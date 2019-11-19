#ifndef STORAGE_H
#define STORAGE_H

#include "folderitem.h"
#include <QString>

class Storage {

    public:
        Storage(const QStringList paths);
        int folderCount();
        FolderItem* getFolder(int index);
        FolderItem* getBaseFolder();
        QString getPath();
        FolderItem* newFolder(QString title);
        bool deleteFolder(FolderItem* folder);

    private:
        QVector<FolderItem*> _folders;
        void sortFolders();

};

#endif // STORAGE_H
