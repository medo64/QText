#include <QDebug>
#include <QDir>
#include <QFileInfo>
#include <QMenu>
#include "clipboard.h"
#include "helpers.h"
#include "icons.h"
#include "settings.h"
#include "fileitem.h"

FileItem::FileItem(FolderItem* folder, QString fileName)
    : QTextEdit(nullptr) {
    _folder = folder;
    _fileName = fileName;

    this->setLineWrapMode(Settings::wordWrap() ? QTextEdit::WidgetWidth : QTextEdit::NoWrap);
    this->setWordWrapMode(QTextOption::WrapAtWordBoundaryOrAnywhere);
    this->setAcceptRichText(false);

    load();

    QFontMetricsF fm (this->font());
    auto tabWidth = Settings::tabWidth() * fm.width(' ');
    this->setTabStopDistance(tabWidth);

    connect(this->document(), SIGNAL(modificationChanged(bool)), this, SLOT(onModificationChanged(bool)));
    this->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(this, SIGNAL(customContextMenuRequested(const QPoint&)), SLOT(onContextMenuRequested(const QPoint&)));
}

FileItem::~FileItem() {
}


FolderItem* FileItem::getFolder() {
    return _folder;
}

QString FileItem::getKey() {
    return _fileName;
}

QString FileItem::getTitle() {
    QString extensions[] { ".txt", ".html" };
    for (int i = 0; i < extensions->length(); i++) {
        auto extension = extensions[i];
        if (_fileName.endsWith(extension, Qt::CaseInsensitive)) {
            auto fileNameWithoutExtension = _fileName.left(_fileName.length() - extension.length());
            return Helpers::getFileTitleFromName(fileNameWithoutExtension);
        }
    }
    return Helpers::getFileTitleFromName(_fileName); //should not happen
}

void FileItem::setTitle(QString newTitle) {
    save();
    if (newTitle == getTitle()) { return; } //no change

    QString curPath = getPath();
    QString newFileName = Helpers::getFileNameFromTitle(newTitle) + (isHtml() ? ".html" : ".txt");
    QString newPath = QDir::cleanPath(_folder->getPath() + QDir::separator() + newFileName);

    QFile curFile(curPath);
    QFile newFile(newPath);

    if (curFile.rename(newPath)) {
        _fileName = newFileName;
        emit titleChanged(this);
    }
}

bool FileItem::isHtml() {
    QString path = getPath();
    return path.endsWith(".html", Qt::CaseInsensitive);
}

bool FileItem::isPlain() {
    QString path = getPath();
    return path.endsWith(".txt", Qt::CaseInsensitive);
}

bool FileItem::isModified() {
    return this->document()->isModified();
}

bool FileItem::isEmpty() {
    return this->document()->isEmpty();
}


bool FileItem::load() {
    qDebug() << "load()" << getPath();

    if (_timerSavePending != nullptr) { _timerSavePending->stop(); }
    this->blockSignals(true);

    bool fileValid;
    QString path = getPath();
    QFile file(path);
    if (file.exists()) {
        if(file.open(QIODevice::ReadOnly)) {
            QTextStream in(&file);
            QString contents = in.readAll();
            QTextDocument *document = new QTextDocument(this);
            if (isHtml()) {
                document->setHtml(contents);
            } else {
                document->setPlainText(contents);
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
            this->setText(file.errorString() + "\n" + path);
            qDebug() << "load()" << getPath() << "error:" << file.errorString();
            fileValid = false;
        }
    } else { //create a new one
        if(file.open(QIODevice::WriteOnly)) {
            file.close();
            this->document()->setModified(false);
            emit titleChanged(this);
            fileValid = true;
        } else {
            this->setReadOnly(true);
            this->setStyleSheet("QTextEdit { background-color: red; color: white; }");
            this->setText(file.errorString() + "\n" + path);
            qDebug() << "new()" << getPath() << "error:" << file.errorString();
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

bool FileItem::save() {
    qDebug() << "save()" << getPath();

    if (_timerSavePending != nullptr) { _timerSavePending->stop(); }

    QString contents;
    if (isHtml()) {
        contents = this->document()->toHtml();
    } else {
        contents = this->document()->toPlainText();
    }

    QString path = getPath();
    QFile file(path);
    if(file.open(QIODevice::WriteOnly)) {
        QTextStream out(&file);
        out << contents;
        file.close();
        this->document()->setModified(false);
        QFileInfo info(file);
        _modificationTime = info.lastModified(); //remember modification time to avoid reload
        return true;
    } else {
        qDebug() << "save()" << getPath() << "error:" << file.errorString();
        return false;
    }
}


bool FileItem::event(QEvent *event) {
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
        }

        if (e->modifiers() == Qt::AltModifier) { return false; } //ignore keys with Alt
        if (e->modifiers() == (Qt::ControlModifier | Qt::AltModifier)) { return false; } //ignore keys with Ctrl+Alt
    }
    return QTextEdit::event(event);
}

void FileItem::focusInEvent(QFocusEvent* e) {
    qDebug().nospace() << "focusInEvent(" << QVariant::fromValue(e->reason()).toString() << ") " << getPath();
    QString path = getPath();
    QFile file(path);
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
    qDebug().nospace() << "focusOutEvent(" << QVariant::fromValue(e->reason()).toString() << ") " << getPath();
    QTextEdit::focusOutEvent(e);
    if (this->document()->isModified()) { save(); }
}


QString FileItem::getPath() {
    return QDir::cleanPath(_folder->getPath() + QDir::separator() + _fileName);
}


void FileItem::onModificationChanged(bool changed) {
    qDebug().nospace() << "onModificationChanged(" << changed << ")" << getPath();

    emit modificationChanged(this, changed);

    int interval = Settings::quickSaveInterval();
    if (interval > 0) {
        if (_timerSavePending == nullptr) {
            _timerSavePending = new QTimer(); //set timer to fire 3 seconds after the last key press
            _timerSavePending->setSingleShot(true);
            QObject::connect(_timerSavePending, SIGNAL(timeout()), this, SLOT(onSavePendingTimeout()));
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
    qDebug() << "onSavePendingTimeout()" << getPath();
    if (this->document()->isModified()) { save(); }
}


void FileItem::onContextMenuRequested(const QPoint& point) {
    if (point.isNull()) { return; }

    QMenu menu(this);

    QAction* undoAction = new QAction(Icons::undo(), "&Undo");
    undoAction->setShortcut(QKeySequence("Ctrl+Z"));
    undoAction->setShortcutVisibleInContextMenu(true);
    undoAction->setDisabled(!document()->isUndoAvailable());
    connect(undoAction, SIGNAL(triggered()), this, SLOT(onContextMenuUndo()));
    menu.addAction(undoAction);

    QAction* redoAction = new QAction(Icons::redo(), "&Redo");
    redoAction->setShortcut(QKeySequence("Ctrl+Y"));
    redoAction->setShortcutVisibleInContextMenu(true);
    redoAction->setDisabled(!document()->isRedoAvailable());
    connect(redoAction, SIGNAL(triggered()), this, SLOT(onContextMenuRedo()));
    menu.addAction(redoAction);

    menu.addSeparator();

    QAction* cutAction = new QAction(Icons::cut(), "Cu&t");
    cutAction->setShortcut(QKeySequence("Ctrl+X"));
    cutAction->setShortcutVisibleInContextMenu(true);
    cutAction->setDisabled(!textCursor().hasSelection());
    connect(cutAction, SIGNAL(triggered()), this, SLOT(onContextMenuCutPlain()));
    menu.addAction(cutAction);

    QAction* copyAction = new QAction(Icons::copy(), "&Copy");
    copyAction->setShortcut(QKeySequence("Ctrl+C"));
    copyAction->setShortcutVisibleInContextMenu(true);
    copyAction->setDisabled(!textCursor().hasSelection());
    connect(copyAction, SIGNAL(triggered()), this, SLOT(onContextMenuCopyPlain()));
    menu.addAction(copyAction);

    QAction* pasteAction = new QAction(Icons::paste(), "&Paste");
    pasteAction->setShortcut(QKeySequence("Ctrl+V"));
    pasteAction->setShortcutVisibleInContextMenu(true);
    pasteAction->setDisabled(!Clipboard::hasText());
    connect(pasteAction, SIGNAL(triggered()), this, SLOT(onContextMenuPastePlain()));
    menu.addAction(pasteAction);

    QAction* deleteAction = new QAction("&Delete");
    deleteAction->setShortcut(QKeySequence("Delete"));
    deleteAction->setShortcutVisibleInContextMenu(true);
    deleteAction->setDisabled(!textCursor().hasSelection());
    connect(deleteAction, SIGNAL(triggered()), this, SLOT(onContextMenuDelete()));
    menu.addAction(deleteAction);

    menu.addSeparator();

    QAction* selectAllAction = new QAction("Select &All");
    selectAllAction->setShortcut(QKeySequence("Ctrl+A"));
    selectAllAction->setShortcutVisibleInContextMenu(true);
    connect(selectAllAction, SIGNAL(triggered()), this, SLOT(onContextMenuSelectAll()));
    menu.addAction(selectAllAction);

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
