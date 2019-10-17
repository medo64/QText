#ifndef FOLDERITEM_H
#define FOLDERITEM_H

#include "fileitem.h"
#include <QString>

class FolderItem {

    public:
        FolderItem(const QString directoryBase, const QString directoryName);
        FolderItem(const QString prefix, const QString directoryBase, const QString directoryName); //prefix differentiates between DataPath and DataPath2
        QString getKey();
        QString getPath();
        QString getTitle();
        size_t fileCount();
        FileItem* getFile(size_t index);
        FileItem* newFile(QString title);
        bool deleteFile(FileItem* file);
        bool saveAll();
        bool fileExists(QString title);

    private:
        QString _directoryPath;
        QString _directoryName;
        QString _prefix;
        std::vector<FileItem*> _files;

};

#endif // FOLDERITEM_H
