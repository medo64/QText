#ifndef FILEITEM_H
#define FILEITEM_H

#include <memory>
#include <QString>
#include <QTextEdit>

class FileItem {

    public:
        FileItem(QString directoryPath, QString fileName);
        QString getTitle();
        QTextEdit* getEditor();
        bool isPlain();
        bool isHtml();

    private:
        QString _directoryPath;
        QString _fileName;
        QString getPath();
        QTextEdit* _editor = nullptr;

};

#endif // FILEITEM_H
