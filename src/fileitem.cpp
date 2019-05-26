#include "fileitem.h"
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

    QString path = getPath();
    QFile file(path);
    if(file.open(QIODevice::ReadOnly)) {
        QTextStream in(&file);
        QString content = in.readAll();
        QTextDocument *document = new QTextDocument(this);
        if (isHtml()) {
            document->setHtml(content);
        } else {
            document->setPlainText(content);
        }
        this->setDocument(document);

        QObject::connect(document, SIGNAL(contentsChanged()), this, SLOT(onContentsChanged()));
    } else {
        this->setReadOnly(true);
        this->setStyleSheet("QTextEdit { background-color: red; color: white; }");
        this->setText(file.errorString() + "\n" + path);
    }
}

FileItem::~FileItem() {
}


QString FileItem::getTitle() {
    return _fileName;
}

bool FileItem::isHtml() {
    QString path = getPath();
    return path.endsWith(".html", Qt::CaseInsensitive);
}

bool FileItem::isPlain() {
    return !isHtml();
}

bool FileItem::hasChanged() {
    return _hasChanged;
}


void FileItem::focusOutEvent(QFocusEvent* e) {
    qDebug().nospace() << "focusOutEvent(" << QVariant::fromValue(e->reason()).toString() << ") " << getPath();
}


QString FileItem::getPath() {
    return QDir::cleanPath(_directoryPath + QDir::separator() + _fileName);
}


void FileItem::onContentsChanged() {
    qDebug() << "onContentsChanged()" << getPath();
    _hasChanged = true;

    if (_timer == nullptr) {
        _timer = new QTimer(); //set timer to fire 3 seconds after the last key press
        _timer->setInterval(3000);
        _timer->setSingleShot(true);
        QObject::connect(_timer, SIGNAL(timeout()), this, SLOT(onAfterChangeTimeout()));
    }
    _timer->stop();
    _timer->start();
}

void FileItem::onAfterChangeTimeout() {
    qDebug() << "onAfterChangeTimeout()" << getPath();
}
