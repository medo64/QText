#ifndef FOLDERITEM_H
#define FOLDERITEM_H

#include <QString>
#include "fileitem.h"

class FileItem;

class FolderItem {

    public:
        FolderItem(FolderItem* rootFolder, const int pathIndex, const QString directoryBase, const QString directoryName); //prefix differentiates between DataPath and DataPath2
        FolderItem* getRootFolder();
        QString getKey();
        QString getPath();
        int getPathIndex();
        QString getTitle();
        bool rename(QString newTitle);
        bool isRoot();
        bool isPrimary();
        int fileCount();
        FileItem* getFile(int index);
        FileItem* newFile(QString title);
        bool deleteFile(FileItem* file);
        bool saveAll();
        bool fileExists(QString title);
        bool moveFile(int from, int to);

    private:
        FolderItem* _rootFolder;
        QString _directoryPath;
        QString _directoryName;
        int _pathIndex;
        QVector<FileItem*> _files;
        void cleanOrdering();
        void saveOrdering();
        void loadOrdering();

};

#endif // FOLDERITEM_H
