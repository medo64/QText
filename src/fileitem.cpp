#include "fileitem.h"
#include <QColor>
#include <QDir>
#include <QEvent>
#include <QTextStream>

FileItem::FileItem(QString directoryPath, QString fileName) {
    _directoryPath = directoryPath;
    _fileName = fileName;
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

QTextEdit* FileItem::getEditor() {
    if (_editor == nullptr) {
        _editor = new QTextEdit();
        _editor->setLineWrapMode(QTextEdit::WidgetWidth);
        _editor->setWordWrapMode(QTextOption::WrapAtWordBoundaryOrAnywhere);

        QString path = getPath();
        QFile file(path);
        if(file.open(QIODevice::ReadOnly)) {
            QTextStream in(&file);
            QString content = in.readAll();
            QTextDocument *document = new QTextDocument(_editor);
            if (isHtml()) {
                document->setHtml(content);
            } else {
                document->setPlainText(content);
            }
            _editor->setDocument(document);
        } else {
            _editor->setText(file.errorString() + "\n" + path);
            _editor->setStyleSheet("QTextEdit { background-color: red; color: white; }");
        }
    }
    return _editor;
}


QString FileItem::getPath() {
    return QDir::cleanPath(_directoryPath + QDir::separator() + _fileName);
}
