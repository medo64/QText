#ifndef FOLDERITEM_H
#define FOLDERITEM_H

#include <QString>
#include "fileitem.h"

class FileItem;

class FolderItem {

    public:
        FolderItem(const int pathIndex, const QString directoryBase, const QString directoryName); //prefix differentiates between DataPath and DataPath2
        QString getKey();
        QString getPath();
        QString getTitle();
        bool isPrimary();
        int fileCount();
        FileItem* getFile(int index);
        FileItem* newFile(QString title);
        bool deleteFile(FileItem* file);
        bool saveAll();
        bool fileExists(QString title);
        bool moveFile(int from, int to);

    private:
        QString _directoryPath;
        QString _directoryName;
        int _pathIndex;
        QVector<FileItem*> _files;

};

#endif // FOLDERITEM_H
