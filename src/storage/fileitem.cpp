#include <QDebug>
#include <QDir>
#include <QFileInfo>
#include <QMenu>
#include <QTextDocumentFragment>
#include "ui/inserttimedialog.h"
#include "clipboard.h"
#include "fileitem.h"
#include "helpers.h"
#include "icons.h"
#include "settings.h"
#include "storage.h"
#include "storagemonitorlocker.h"

const QStringList urlProtocols({ "ftp://", "http://", "https://", "mailto://", "sftp://", "ssh://" });
const QStringList urlPrefixes({ "www." });
const QStringList urlSuffixes({ ".com", ".org", ".net", ".int", ".edu", ".gov", ".mil", ".edu" });
const QStringList urlInfixes({ ".com/", ".org/", ".net/", ".int/", ".edu/", ".gov/", ".mil/", ".edu/" });


FileItem::FileItem(FolderItem* folder, QString fileName)
    : QTextEdit(nullptr) {
    _folder = folder;
    _fileName = fileName;

    this->setLineWrapMode(Settings::wordWrap() ? QTextEdit::WidgetWidth : QTextEdit::NoWrap);
    this->setWordWrapMode(QTextOption::WrapAtWordBoundaryOrAnywhere);
    this->setFrameStyle(QFrame::NoFrame);

    load();

    QFontMetricsF fm(this->font());
    auto tabWidth = Settings::tabWidth() * fm.width(' ');
    this->setTabStopDistance(tabWidth);

    connect(this->document(), &QTextDocument::modificationChanged, this, &FileItem::onModificationChanged);
    this->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(this, &FileItem::customContextMenuRequested, this, &FileItem::onContextMenuRequested);

    this->installEventFilter(this);  // just to catch Ctrl key
    this->viewport()->installEventFilter(this);
#if QT_VERSION < QT_VERSION_CHECK(5, 14, 0)
    this->viewport()->setMouseTracking(true);
#endif
}

FileItem::~FileItem() {
    QTextEdit::deleteLater();
}


FolderItem* FileItem::folder() const {
    return _folder;
}

QString FileItem::name() const {
    return _fileName;
}

QString FileItem::title() const {
    for (QString extension : Storage::supportedExtensions()) {
        if (_fileName.endsWith(extension, Qt::CaseInsensitive)) {
            auto fileNameWithoutExtension = _fileName.left(_fileName.length() - extension.length());
            return Helpers::getFileTitleFromName(fileNameWithoutExtension);
        }
    }
    return Helpers::getFileTitleFromName(_fileName); //should not happen
}

void FileItem::setTitle(QString newTitle) {
    StorageMonitorLocker lockMonitor(_folder->storage()->monitor());

    save();
    if (newTitle == title()) { return; } //no change

    QString curPath = path();
    QString newFileName = Helpers::getFileNameFromTitle(newTitle) + extension();
    QString newPath = QDir::cleanPath(_folder->path() + QDir::separator() + newFileName);

    QFile curFile(curPath);
    QFile newFile(newPath);

    if (curFile.rename(newPath)) {
        _fileName = newFileName;
        emit titleChanged(this);
    }
}

FileType FileItem::type() const {
    QFileInfo file(path());
    if (file.suffix() == Storage::htmlSuffix()) {
        return FileType::Html;
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
    } else  if (file.suffix() == Storage::markdownSuffix()) {
        return FileType::Markdown;
#endif
    } else {
        return FileType::Plain;
    }
}

bool FileItem::setType(FileType newType) {
    if (type() == newType) { return true; } //already that type
    if (!save()) { return false; } //skip if it cannot be saved

    QString oldPath = path();
    QFileInfo oldFile(oldPath);
    QString newFileName;
    switch (newType) {
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
        case FileType::Markdown:
            newFileName = oldFile.completeBaseName() + "." + Storage::markdownSuffix();
            break;
#endif
        case FileType::Html:
            newFileName = oldFile.completeBaseName() + "." + Storage::htmlSuffix();
            break;
        default:
            newFileName = oldFile.completeBaseName() + "." + Storage::plainSuffix();
            break;
    }
    QString newPath = QDir::cleanPath(oldFile.dir().path() + "/" + newFileName);

    QDir dir;
    if (dir.rename(oldPath, newPath)) {
        _fileName = newFileName;
        save();
        load();
        document()->clearUndoRedoStacks();
        return true;
    } else {
        return false;
    }
}

QString FileItem::extension() const {
    QFileInfo fileInfo(_fileName);
    return "." + fileInfo.suffix();
}

bool FileItem::isModified() const {
    return this->document()->isModified();
}

bool FileItem::isEmpty() const {
    return this->document()->isEmpty();
}


bool FileItem::load() {
    qDebug() << "load()" << path();

    if (_timerSavePending != nullptr) { _timerSavePending->stop(); }
    this->blockSignals(true);

    bool fileValid;
    QFile file(path());
    if (file.exists()) {
        if (file.open(QIODevice::ReadOnly)) {
            QTextStream in(&file);
            in.setCodec(_utf8Codec);
            in.setAutoDetectUnicode(true);
            QString contents = in.readAll();
            QTextDocument* document = new QTextDocument(this);

            switch (type()) {
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
                case FileType::Markdown:
                    document->setMarkdown(contents);
                    this->setAcceptRichText(false);
                    break;
#endif
                case FileType::Html:
                    document->setHtml(contents);
                    this->setAcceptRichText(true);
                    break;
                default:
                    document->setPlainText(contents);
                    this->setAcceptRichText(false);
                    break;
            }
            file.close();
            this->setDocument(document);
            this->document()->setModified(false);

            this->blockSignals(false);
            emit titleChanged(this);
            fileValid = true;
        } else {
            this->setReadOnly(true);
            this->setStyleSheet("QTextEdit { background-color: red; color: white; }");
            this->setText(file.errorString() + "\n" + path());
            qDebug() << "load()" << path() << "error:" << file.errorString();
            fileValid = false;
        }
    } else { //create a new one
        if (file.open(QIODevice::WriteOnly)) {
            file.close();
            this->document()->setModified(false);
            this->blockSignals(false);
            emit titleChanged(this);
            fileValid = true;
        } else {
            this->setReadOnly(true);
            this->setStyleSheet("QTextEdit { background-color: red; color: white; }");
            this->setText(file.errorString() + "\n" + path());
            qDebug() << "new()" << path() << "error:" << file.errorString();
            fileValid = false;
        }
    }

    QFont font = Settings::font();
    this->setFont(font); //for plain text
    this->document()->setDefaultFont(font);

    if (fileValid) {
        QFileInfo info(file);
        _modificationTime = info.lastModified(); //store so that file change can be detected
    } else {
        _modificationTime = QDateTime::currentDateTimeUtc();
    }
    return fileValid;
}

bool FileItem::save() const {
    qDebug() << "save()" << path();

    if (_timerSavePending != nullptr) { _timerSavePending->stop(); }

    QString contents;
    switch (type()) {
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
        case FileType::Markdown:
            contents = this->document()->toMarkdown();
            break;
#endif
        case FileType::Html:
            contents = this->document()->toHtml();
            break;
        default:
            contents = this->document()->toPlainText();
            break;
    }

    QFile file(path());
    if (file.open(QIODevice::WriteOnly)) {
        QTextStream out(&file);
        out.setCodec(_utf8Codec);
        out.setAutoDetectUnicode(true);
        out << contents;
        out.flush();
        file.close();
        this->document()->setModified(false);
        QFileInfo info(file);
        _modificationTime = info.lastModified(); //remember modification time to avoid reload
        return true;
    } else {
        qDebug() << "save()" << path() << "error:" << file.errorString();
        return false;
    }
}

bool FileItem::setFolder(FolderItem* newFolder) {
    if (newFolder == nullptr) { return false; }

    auto fromFile = this->path();
    auto toFile = QDir::cleanPath(newFolder->path() + "/" + this->name());
    StorageMonitorLocker lockMonitor(_folder->storage()->monitor());
    if (QDir().rename(fromFile, toFile)) { //only move if rename was possible
        _folder->removeItem(this);
        _folder = newFolder;
        newFolder->addItem(this);
        return true;
    }
    return false;
}


bool FileItem::isTextUndoAvailable() {
    return document()->isUndoAvailable();
}

void FileItem::textUndo() {
    document()->undo();
}

bool FileItem::isTextRedoAvailable() {
    return document()->isRedoAvailable();
}

void FileItem::textRedo() {
    document()->redo();
}

bool FileItem::canTextCut() {
    return textCursor().hasSelection();
}


void FileItem::textCut(bool forcePlain) {
    if (!Settings::forcePlainCopyPaste() && !forcePlain) {
        Clipboard::cutHtml(textCursor());
    } else {
        Clipboard::cutPlain(textCursor());
    }
}

bool FileItem::canTextCopy() {
    return textCursor().hasSelection();
}

void FileItem::textCopy(bool forcePlain) {
    if (!Settings::forcePlainCopyPaste() && !forcePlain) {
        Clipboard::copyHtml(textCursor());
    } else {
        Clipboard::copyPlain(textCursor());
    }
}

bool FileItem::canTextPaste() {
    return Clipboard::hasPlain();
}

void FileItem::textPaste(bool forcePlain) {
    if (!Settings::forcePlainCopyPaste() && !forcePlain && (type() == FileType::Html)) {
        Clipboard::pasteHtml(textCursor());
    } else {
        Clipboard::pastePlain(textCursor());
    }
}


bool FileItem::isFontBold() {
    return textCursor().charFormat().fontWeight() == QFont::Bold;
}

void FileItem::setFontBold(bool bold) {
    qDebug().noquote().nospace() << "[FileItem] setFontBold(" << bold << ")";
    QTextCursor cursor = textCursor();
    if (cursor.hasSelection()) {
        QTextCharFormat format;
        format.setFontWeight(bold ? QFont::Bold : QFont::Normal);
        cursor.mergeCharFormat(format);
    } else { //change for new text
        QTextCharFormat format = currentCharFormat();
        format.setFontWeight(bold ? QFont::Bold : QFont::Normal);
        setCurrentCharFormat(format);
    }
}

bool FileItem::isFontItalic() {
    return textCursor().charFormat().fontItalic();
}

void FileItem::setFontItalic(bool italic) {
    qDebug().noquote().nospace() << "[FileItem] setFontItalic(" << italic << ")";
    QTextCursor cursor = textCursor();
    if (cursor.hasSelection()) {
        QTextCharFormat format;
        format.setFontItalic(italic);
        cursor.mergeCharFormat(format);
    } else { //change for new text
        QTextCharFormat format = currentCharFormat();
        format.setFontItalic(italic);
        setCurrentCharFormat(format);
    }
}

bool FileItem::isFontUnderline() {
    return textCursor().charFormat().fontUnderline();
}

void FileItem::setFontUnderline(bool underline) {
    qDebug().noquote().nospace() << "[FileItem] setFontUnderline(" << underline << ")";
    QTextCursor cursor = textCursor();
    if (cursor.hasSelection()) {
        QTextCharFormat format;
        format.setFontUnderline(underline);
        cursor.mergeCharFormat(format);
    } else { //change for new text
        QTextCharFormat format = currentCharFormat();
        format.setFontUnderline(underline);
        setCurrentCharFormat(format);
    }
}

bool FileItem::isFontStrikethrough() {
    return textCursor().charFormat().fontStrikeOut();
}

void FileItem::setFontStrikethrough(bool strikethrough) {
    qDebug().noquote().nospace() << "[FileItem] setFontStrikethrough(" << strikethrough << ")";
    QTextCursor cursor = textCursor();
    if (cursor.hasSelection()) {
        QTextCharFormat format;
        format.setFontStrikeOut(strikethrough);
        cursor.mergeCharFormat(format);
    } else { //change for new text
        QTextCharFormat format = currentCharFormat();
        format.setFontStrikeOut(strikethrough);
        setCurrentCharFormat(format);
    }
}


bool FileItem::event(QEvent* event) {
    if (event->type() == QEvent::KeyPress) {
        QKeyEvent* e = static_cast<QKeyEvent*>(event);
        if ((e->key() == Qt::Key_Return) && (e->modifiers() != Qt::NoModifier)) { //allow enter with any modifier
            e->setModifiers(Qt::NoModifier);
            return QTextEdit::event(e);
        }

        if ((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_Z)) {
            onContextMenuUndo();
            return true;
        } else if ((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_Y)) {
            onContextMenuRedo();
            return true;
        } else if ((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_X)) {
            onContextMenuCut();
            return true;
        } else if ((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_C)) {
            onContextMenuCopy();
            return true;
        } else if ((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_V)) {
            onContextMenuPaste();
            return true;
        } else if (((e->modifiers() == (Qt::ControlModifier | Qt::ShiftModifier)) && (e->key() == Qt::Key_X))
                   || ((e->modifiers() == Qt::ShiftModifier) && (e->key() == Qt::Key_Delete))) {
            onContextMenuCutPlain();
            return true;
        } else if (((e->modifiers() == (Qt::ControlModifier | Qt::ShiftModifier)) && (e->key() == Qt::Key_C))
                   || ((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_Insert))) {
            onContextMenuCopyPlain();
            return true;
        } else if (((e->modifiers() == (Qt::ControlModifier | Qt::ShiftModifier)) && (e->key() == Qt::Key_V))
                   || ((e->modifiers() == Qt::ShiftModifier) && (e->key() == Qt::Key_Insert))) {
            onContextMenuPastePlain();
            return true;
        } else if ((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_A)) {
            onContextMenuSelectAll();
            return true;
        } else if ((e->modifiers() == Qt::NoModifier) && (e->key() == Qt::Key_F5)) {
            onContextMenuInsertTime();
            return true;
        }

        if (e->modifiers() == Qt::AltModifier) { return false; } //ignore keys with Alt
        if (e->modifiers() == (Qt::ControlModifier | Qt::AltModifier)) { return false; } //ignore keys with Ctrl+Alt
    }
    return QTextEdit::event(event);
}

bool FileItem::eventFilter(QObject* obj, QEvent* event) {
    bool isViewport = (obj == viewport());
    bool isKeyEvent = (event->type() == QEvent::KeyPress) || (event->type() == QEvent::KeyRelease);

    if (isViewport || isKeyEvent) {
        QMouseEvent* e = nullptr;
        bool isMouseClick = false;
        bool isMouseDblClick = false;
        bool isMouseMove = false;

        switch (event->type()) {
            case QEvent::KeyPress:
            case QEvent::KeyRelease: {
                    QKeyEvent* ek = static_cast<QKeyEvent*>(event);
                    if (ek->key() == Qt::Key_Control) {  // pretend this is a mouse move
                        e = new QMouseEvent(QEvent::MouseMove,
                                            this->mapFromGlobal(QCursor::pos()),
                                            Qt::NoButton,
                                            Qt::NoButton,
                                            ek->modifiers());  // dummy event details
                        isMouseMove = true;
                    }
                } break;

            case QEvent::MouseButtonPress:
                e = static_cast<QMouseEvent*>(event);
                isMouseClick = (e->buttons() == Qt::LeftButton);
                break;

            case QEvent::MouseButtonDblClick:
                e = static_cast<QMouseEvent*>(event);
                isMouseDblClick = (e->buttons() == Qt::LeftButton);
                break;

            case QEvent::MouseMove:
                e = static_cast<QMouseEvent*>(event);
                isMouseMove = true;
                break;

            default: break;
        }

        if (e != nullptr) {
            bool isCtrlPressed = e->modifiers() == Qt::ControlModifier;
            bool useCtrl = Settings::followUrlWithCtrl();
            bool useDblClick = Settings::followUrlWithDoubleClick();
            QString anchor = findAnchorAt(e->pos());

            if ((isMouseClick && useCtrl && isCtrlPressed) || (isMouseDblClick && useDblClick)) {
                if (!anchor.isEmpty()) {
                    qDebug().noquote().nospace() << "[FileItem] URL executing (" << anchor << ")";
                    QApplication::setOverrideCursor(Qt::WaitCursor);
                    Helpers::openUrl(anchor);
                    QApplication::restoreOverrideCursor();
                    return true;
                }
            } else if (isMouseMove) {
                if ((e->buttons() == Qt::NoButton)) {
                    if (!anchor.isEmpty() && ((useCtrl && isCtrlPressed) || useDblClick)) {
                        if (!customCursorSet) {
                            qDebug().noquote().nospace() << "[FileItem] URL detected (" << anchor << ")";
                            viewport()->setCursor(Qt::PointingHandCursor);
                            customCursorSet = true;
                        }
                    } else if (customCursorSet) {
                        qDebug().noquote().nospace() << "[FileItem] URL gone";
                        viewport()->setCursor(Qt::IBeamCursor);
                        customCursorSet = false;
                    }
                }
            }
        }
    }
    return false;
}

void FileItem::focusInEvent(QFocusEvent* e) {
    qDebug().nospace() << "focusInEvent(" << QVariant::fromValue(e->reason()).toString() << ") " << path();
    QFile file(path());
    if (file.exists()) {
        QFileInfo info(file);
        if (_modificationTime != info.lastModified()) { load(); }

        int clearUndoInterval = Settings::clearUndoInterval();
        if ((clearUndoInterval > 0) && (this->document()->isUndoAvailable() || this->document()->isRedoAvailable())) {
            qint64 intervalSinceLastModification = info.lastModified().secsTo(QDateTime::currentDateTime());
            if (intervalSinceLastModification > clearUndoInterval) {
                this->document()->clearUndoRedoStacks();
            }
        }
    } else {
        load(); //just to ensure all fields are filled
    }

    QTextEdit::focusInEvent(e);
    emit activated(this);
}

void FileItem::focusOutEvent(QFocusEvent* e) {
    qDebug().nospace() << "focusOutEvent(" << QVariant::fromValue(e->reason()).toString() << ") " << path();
    QTextEdit::focusOutEvent(e);
    if (this->document()->isModified()) { save(); }
}

void FileItem::wheelEvent(QWheelEvent* e) {
    if (e->modifiers() == Qt::ControlModifier) {
        QPoint angleDelta = e->angleDelta();
        if (!angleDelta.isNull()) {
            int degreesY = angleDelta.y() / 8;
            int numSteps = degreesY / 15;
            int deltaZoomAmount = numSteps * 2; //each wheel turn is 2 zoom steps
            if (numSteps > 0) {
                if (zoomAmount + deltaZoomAmount > 64) { deltaZoomAmount = 20 - zoomAmount; } //don't allow zoom to go over 64 points
                if (deltaZoomAmount > 0) {
                    zoomAmount += deltaZoomAmount;
                    this->zoomIn(deltaZoomAmount);
                }
            } else if (numSteps < 0) {
                if (zoomAmount + deltaZoomAmount < 0) { deltaZoomAmount = -zoomAmount; } //don't allow zoom to go negative
                if (deltaZoomAmount < 0) {
                    zoomAmount += deltaZoomAmount;
                    this->zoomIn(deltaZoomAmount);
                }
            }
            e->accept();
        }
    } else {
        QTextEdit::wheelEvent(e);
    }
}


QString FileItem::findAnchorAt(QPoint pos) {
    QString anchor = this->anchorAt(pos);
    if (!anchor.isEmpty()) { return anchor; }

    // check for links in text
    QTextCursor cursor = cursorForPosition(pos);
    int charPosition = cursor.positionInBlock();

    cursor.movePosition(QTextCursor::StartOfBlock, QTextCursor::MoveAnchor, 1);
    cursor.movePosition(QTextCursor::EndOfBlock, QTextCursor::KeepAnchor, 1);
    QString blockText = cursor.selectedText();
    if (charPosition >= blockText.length()) { return QString(); }  // don't check if on last character - to avoid "whitespace links"

    int startAt = 0;
    int endAt = blockText.length();
    for (int i = 0; i < blockText.length(); i++) {
        QChar ch = blockText.at(i);
        bool isUrlChar = ch.isLetterOrNumber()
                         || (ch == '-')
                         || (ch == '.')
                         || (ch == '_')
                         || (ch == ':')
                         || (ch == '/')
                         || (ch == '?')
                         || (ch == '&')
                         || (ch == '=')
                         || (ch == '~')
                         || (ch == '%');
        if (!isUrlChar) {
            if (i < charPosition) {
                startAt = i + 1;
            } else {
                endAt = i;
                break;
            }
        }
    }

    QString text = blockText.mid(startAt, endAt - startAt);
    QString url;
    for (QString part : urlProtocols) {
        if (text.startsWith(part)) { url = text; }
    }
    if (url.isNull()) {
        for (QString part : urlPrefixes) {
            if (text.startsWith(part)) { url = text; }
        }
    }
    if (url.isNull()) {
        for (QString part : urlSuffixes) {
            if (text.endsWith(part)) { url = text; }
        }
    }
    if (url.isNull()) {
        for (QString part : urlInfixes) {
            if (text.contains(part)) { url = text; }
        }
    }
    if (!url.isNull()) {
        if (url.contains("://")) { return url; }
        if (url.startsWith("ftp.")) { return "ftp://" + url; }
        return "http://" + url;
    }

    return QString();
}


QString FileItem::path() const {
    return QDir::cleanPath(_folder->path() + QDir::separator() + _fileName);
}


void FileItem::printPreview(QPrinter* printer) {
    print(printer);
}


void FileItem::onModificationChanged(bool changed) {
    qDebug().nospace() << "onModificationChanged(" << changed << ")" << path();

    emit modificationChanged(this, changed);

    int interval = Settings::quickSaveInterval();
    if (interval > 0) {
        if (_timerSavePending == nullptr) {
            _timerSavePending = new QTimer(); //set timer to fire 3 seconds after the last key press
            _timerSavePending->setSingleShot(true);
            QObject::connect(_timerSavePending, &QTimer::timeout, this, &FileItem::onSavePendingTimeout);
        }
        _timerSavePending->stop();
        _timerSavePending->setInterval(interval);
        _timerSavePending->start();
    } else if (_timerSavePending != nullptr) { //if interval is 0, remove pending timer
        _timerSavePending->stop();
        delete _timerSavePending;
        _timerSavePending = nullptr;
    }
}

void FileItem::onSavePendingTimeout() {
    qDebug() << "onSavePendingTimeout()" << path();
    if (this->document()->isModified()) { save(); }
}


void FileItem::onContextMenuRequested(const QPoint& point) {
    if (point.isNull()) { return; }

    QMenu menu(this);

    QAction* undoAction = new QAction(Icons::undo(), "&Undo");
    undoAction->setShortcut(QKeySequence("Ctrl+Z"));
    undoAction->setShortcutVisibleInContextMenu(true);
    undoAction->setDisabled(!document()->isUndoAvailable());
    connect(undoAction, &QAction::triggered, this, &FileItem::onContextMenuUndo);
    menu.addAction(undoAction);

    QAction* redoAction = new QAction(Icons::redo(), "&Redo");
    redoAction->setShortcut(QKeySequence("Ctrl+Y"));
    redoAction->setShortcutVisibleInContextMenu(true);
    redoAction->setDisabled(!document()->isRedoAvailable());
    connect(redoAction, &QAction::triggered, this, &FileItem::onContextMenuRedo);
    menu.addAction(redoAction);

    menu.addSeparator();

    QAction* cutAction = new QAction(Icons::cut(), "Cu&t");
    cutAction->setShortcut(QKeySequence("Ctrl+X"));
    cutAction->setShortcutVisibleInContextMenu(true);
    cutAction->setDisabled(!textCursor().hasSelection());
    connect(cutAction, &QAction::triggered, this, &FileItem::onContextMenuCut);
    menu.addAction(cutAction);

    QAction* copyAction = new QAction(Icons::copy(), "&Copy");
    copyAction->setShortcut(QKeySequence("Ctrl+C"));
    copyAction->setShortcutVisibleInContextMenu(true);
    copyAction->setDisabled(!textCursor().hasSelection());
    connect(copyAction, &QAction::triggered, this, &FileItem::onContextMenuCopy);
    menu.addAction(copyAction);

    QAction* pasteAction = new QAction(Icons::paste(), "&Paste");
    pasteAction->setShortcut(QKeySequence("Ctrl+V"));
    pasteAction->setShortcutVisibleInContextMenu(true);
    pasteAction->setDisabled(!Clipboard::hasPlain());
    connect(pasteAction, &QAction::triggered, this, &FileItem::onContextMenuPaste);
    menu.addAction(pasteAction);

    QAction* deleteAction = new QAction("&Delete");
    deleteAction->setShortcut(QKeySequence("Delete"));
    deleteAction->setShortcutVisibleInContextMenu(true);
    deleteAction->setDisabled(!textCursor().hasSelection());
    connect(deleteAction, &QAction::triggered, this, &FileItem::onContextMenuDelete);
    menu.addAction(deleteAction);

    menu.addSeparator();

    QAction* selectAllAction = new QAction("Select &All");
    selectAllAction->setShortcut(QKeySequence("Ctrl+A"));
    selectAllAction->setShortcutVisibleInContextMenu(true);
    connect(selectAllAction, &QAction::triggered, this, &FileItem::onContextMenuSelectAll);
    menu.addAction(selectAllAction);

    menu.addSeparator();

    QAction* insertDateTimeAction = new QAction("Insert &Time/Date");
    insertDateTimeAction->setShortcut(QKeySequence("F5"));
    insertDateTimeAction->setShortcutVisibleInContextMenu(true);
    connect(insertDateTimeAction, &QAction::triggered, this, &FileItem::onContextMenuInsertTime);
    menu.addAction(insertDateTimeAction);

    QString anchor = findAnchorAt(point);
    if (!anchor.isEmpty()) {
        QString anchorText = anchor;
        int questionMark = anchorText.indexOf('?');
        if (questionMark > 0) { anchorText = anchorText.left(questionMark); }
        if (anchorText.length() > 42) { anchorText = anchorText.left(39) + "…"; }

        menu.addSeparator();

        QAction* copyUrlAction = new QAction("Copy " + anchorText);
        copyUrlAction->setData(anchor);
        connect(copyUrlAction, &QAction::triggered, this, &FileItem::onContextCopyUrl);
        menu.addAction(copyUrlAction);

        QAction* goToUrlAction = new QAction("Go to " + anchorText);
        goToUrlAction->setData(anchor);
        connect(goToUrlAction, &QAction::triggered, this, &FileItem::onContextGoToUrl);
        menu.addAction(goToUrlAction);
    }

    menu.addSeparator();

    QAction* resetFontAction = new QAction("&Reset Font");
    resetFontAction->setDisabled(!textCursor().hasSelection());
    connect(resetFontAction, &QAction::triggered, this, &FileItem::onContextMenuResetFont);
    menu.addAction(resetFontAction);

    menu.exec(this->mapToGlobal(point));
}

void FileItem::onContextMenuUndo() {
    textUndo();
}

void FileItem::onContextMenuRedo() {
    textRedo();
}


void FileItem::onContextMenuCut() {
    textCut();
}

void FileItem::onContextMenuCopy() {
    textCopy();
}

void FileItem::onContextMenuPaste() {
    textPaste();
}

void FileItem::onContextMenuCutPlain() {
    textCut(true);
}

void FileItem::onContextMenuCopyPlain() {
    textCopy(true);
}

void FileItem::onContextMenuPastePlain() {
    textPaste(true);
}


void FileItem::onContextMenuDelete() {
    textCursor().removeSelectedText();
}

void FileItem::onContextMenuSelectAll() {
    selectAll();
}

void FileItem::onContextMenuInsertTime() {
    auto dialog = new InsertTimeDialog(this);
    if (dialog->exec() == QDialog::Accepted) {
        textCursor().removeSelectedText();
        textCursor().insertText(dialog->formattedTime() + "\n");
    }
}

void FileItem::onContextMenuResetFont() {
    QTextCursor cursor = textCursor();
    if (cursor.hasSelection()) {
        QTextCharFormat format;
        format.setFont(Settings::font());
        cursor.setCharFormat(format);
    }
}

void FileItem::onContextCopyUrl() {
    auto senderAction = dynamic_cast<QAction*>(sender());
    auto url = senderAction->data().toString();
    Clipboard::setData(url);
}

void FileItem::onContextGoToUrl() {
    auto senderAction = dynamic_cast<QAction*>(sender());
    auto url = senderAction->data().toString();
    QApplication::setOverrideCursor(Qt::WaitCursor);
    Helpers::openUrl(url);
    QApplication::restoreOverrideCursor();
}
