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

FileItem::FileItem(FolderItem* folder, QString fileName)
    : QTextEdit(nullptr) {
    _folder = folder;
    _fileName = fileName;

    this->setLineWrapMode(Settings::wordWrap() ? QTextEdit::WidgetWidth : QTextEdit::NoWrap);
    this->setWordWrapMode(QTextOption::WrapAtWordBoundaryOrAnywhere);
    this->setFrameStyle(QFrame::NoFrame);

    load();

    QFontMetricsF fm (this->font());
    auto tabWidth = Settings::tabWidth() * fm.width(' ');
    this->setTabStopDistance(tabWidth);

    connect(this->document(), &QTextDocument::modificationChanged, this, &FileItem::onModificationChanged);
    this->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(this, &FileItem::customContextMenuRequested, this, &FileItem::onContextMenuRequested);
}

FileItem::~FileItem() {
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
    if (file.suffix() == "md") {
        return FileType::Markdown;
    } else  if (file.suffix() == "html") {
        return FileType::Html;
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
        case FileType::Markdown:
            newFileName = oldFile.completeBaseName() + ".md";
            break;
        case FileType::Html:
            newFileName = oldFile.completeBaseName() + ".html";
            break;
        default:
            newFileName = oldFile.completeBaseName() + ".txt";
            break;
    }
    QString newPath = QDir::cleanPath(oldFile.dir().path() + "/" + newFileName);

    QDir dir;
    if (dir.rename(oldPath, newPath)) {
        _fileName = newFileName;
        save();
        load();
        return true;
    } else {
        return false;
    }
}

QString FileItem::extension() const {
    switch (type()) {
        case FileType::Markdown: return ".md";
        case FileType::Html: return ".html";
        default: return ".txt";
    }
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
            QString contents = in.readAll();
            QTextDocument* document = new QTextDocument(this);
            switch (type()) {
                case FileType::Markdown:
                    document->setMarkdown(contents);
                    this->setAcceptRichText(false);
                    break;
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
        case FileType::Markdown:
            contents = this->document()->toMarkdown();
            break;
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
        } else if (((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_X))
                   || ((e->modifiers() == (Qt::ControlModifier | Qt::ShiftModifier)) && (e->key() == Qt::Key_X))
                   || ((e->modifiers() == Qt::ShiftModifier) && (e->key() == Qt::Key_Delete))) {
            onContextMenuCutPlain();
            return true;
        } else if (((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_C))
                   || ((e->modifiers() == (Qt::ControlModifier | Qt::ShiftModifier)) && (e->key() == Qt::Key_C))
                   || ((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_Insert))) {
            onContextMenuCopyPlain();
            return true;
        } else if (((e->modifiers() == Qt::ControlModifier) && (e->key() == Qt::Key_V))
                   || ((e->modifiers() == (Qt::ControlModifier | Qt::ShiftModifier)) && (e->key() == Qt::Key_V))
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
        int numDegrees = e->delta() / 8;
        int numSteps = numDegrees / 15;
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
    } else {
        QTextEdit::wheelEvent(e);
    }
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
    connect(cutAction, &QAction::triggered, this, &FileItem::onContextMenuCutPlain);
    menu.addAction(cutAction);

    QAction* copyAction = new QAction(Icons::copy(), "&Copy");
    copyAction->setShortcut(QKeySequence("Ctrl+C"));
    copyAction->setShortcutVisibleInContextMenu(true);
    copyAction->setDisabled(!textCursor().hasSelection());
    connect(copyAction, &QAction::triggered, this, &FileItem::onContextMenuCopyPlain);
    menu.addAction(copyAction);

    QAction* pasteAction = new QAction(Icons::paste(), "&Paste");
    pasteAction->setShortcut(QKeySequence("Ctrl+V"));
    pasteAction->setShortcutVisibleInContextMenu(true);
    pasteAction->setDisabled(!Clipboard::hasText());
    connect(pasteAction, &QAction::triggered, this, &FileItem::onContextMenuPastePlain);
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

    menu.exec(this->mapToGlobal(point));
}

void FileItem::onContextMenuUndo() {
    document()->undo();
}

void FileItem::onContextMenuRedo() {
    document()->redo();
}

void FileItem::onContextMenuCutPlain() {
    Clipboard::cutText(textCursor());
}

void FileItem::onContextMenuCopyPlain() {
    Clipboard::copyText(textCursor());
}

void FileItem::onContextMenuPastePlain() {
    Clipboard::pasteText(textCursor());
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
