#include "fileitem.h"
#include <QColor>
#include <QDir>
#include <QEvent>
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
    } else {
        this->setText(file.errorString() + "\n" + path);
        this->setStyleSheet("QTextEdit { background-color: red; color: white; }");
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


QString FileItem::getPath() {
    return QDir::cleanPath(_directoryPath + QDir::separator() + _fileName);
}
