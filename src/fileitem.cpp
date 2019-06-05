#include "fileitem.h"
#include "helpers.h"
#include <QColor>
#include <QDir>
#include <QEvent>
#include <QObject>
#include <QtDebug>
#include <QTextStream>

FileItem::FileItem(QString directoryPath, QString fileName)
    : QTextEdit(nullptr) {
    _directoryPath = directoryPath;
    _fileName = fileName;

    this->setLineWrapMode(QTextEdit::WidgetWidth);
    this->setWordWrapMode(QTextOption::WrapAtWordBoundaryOrAnywhere);

    load();
    connect(this->document(), SIGNAL(modificationChanged(bool)), this, SLOT(onModificationChanged(bool)));
}

FileItem::~FileItem() {
}


QString FileItem::getTitle() {
    QString extensions[] { ".txt", ".html" };
    for (int i = 0; i < extensions->length(); i++) {
        auto extension = extensions[i];
        if (_fileName.endsWith(extension, Qt::CaseInsensitive)) {
            auto fileNameWithoutExtension = _fileName.left(_fileName.length() - extension.length());
            return Helpers::getTitleFromFSName(fileNameWithoutExtension);
        }
    }
    return Helpers::getTitleFromFSName(_fileName); //should not happen
}

void FileItem::setTitle(QString newTitle) {
    save();
    if (newTitle == getTitle()) { return; } //no change

    QString curPath = getPath();
    QString newFileName = Helpers::getFSNameFromTitle(newTitle) + (isHtml() ? ".html" : ".txt");
    QString newPath = QDir::cleanPath(_directoryPath + QDir::separator() + newFileName);

    QFile curFile(curPath);
    QFile newFile(newPath);
    if (!newFile.exists()) {
        curFile.rename(newPath);
        _fileName = newFileName;
        emit titleChanged(this);
    }
}

bool FileItem::isHtml() {
    QString path = getPath();
    return path.endsWith(".html", Qt::CaseInsensitive);
}

bool FileItem::isPlain() {
    return !isHtml();
}

bool FileItem::isModified() {
    return this->document()->isModified();
}

bool FileItem::load() {
    qDebug() << "load()" << getPath();

    if (_timerSavePending != nullptr) { _timerSavePending->stop(); }
    this->blockSignals(true);

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
            return true;
        } else {
            this->setReadOnly(true);
            this->setStyleSheet("QTextEdit { background-color: red; color: white; }");
            this->setText(file.errorString() + "\n" + path);
            qDebug() << "load()" << getPath() << "error:" << file.errorString();
            return false;
        }
    } else { //create a new one
        if(file.open(QIODevice::WriteOnly)) {
            file.close();
            this->document()->setModified(false);
            emit titleChanged(this);
            return true;
        } else {
            this->setReadOnly(true);
            this->setStyleSheet("QTextEdit { background-color: red; color: white; }");
            this->setText(file.errorString() + "\n" + path);
            qDebug() << "new()" << getPath() << "error:" << file.errorString();
            return false;
        }
    }
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
        return true;
    } else {
        qDebug() << "save()" << getPath() << "error:" << file.errorString();
        return false;
    }
}


void FileItem::focusInEvent(QFocusEvent* e) {
    qDebug().nospace() << "focusInEvent(" << QVariant::fromValue(e->reason()).toString() << ") " << getPath();
    QTextEdit::focusInEvent(e);
    emit activated(this);
}

void FileItem::focusOutEvent(QFocusEvent* e) {
    qDebug().nospace() << "focusOutEvent(" << QVariant::fromValue(e->reason()).toString() << ") " << getPath();
    QTextEdit::focusOutEvent(e);
    if (this->document()->isModified()) { save(); }
}


QString FileItem::getPath() {
    return QDir::cleanPath(_directoryPath + QDir::separator() + _fileName);
}


void FileItem::onModificationChanged(bool changed) {
    qDebug().nospace() << "onModificationChanged(" << changed << ")" << getPath();

    emit modificationChanged(this, changed);

    if (_timerSavePending == nullptr) {
        _timerSavePending = new QTimer(); //set timer to fire 3 seconds after the last key press
        _timerSavePending->setInterval(3000);
        _timerSavePending->setSingleShot(true);
        QObject::connect(_timerSavePending, SIGNAL(timeout()), this, SLOT(onSavePendingTimeout()));
    }
    _timerSavePending->stop();
    _timerSavePending->start();
}

void FileItem::onSavePendingTimeout() {
    qDebug() << "onSavePendingTimeout()" << getPath();
    if (this->document()->isModified()) { save(); }
}
