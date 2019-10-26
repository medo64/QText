#ifndef STORAGE_H
#define STORAGE_H

#include "folderitem.h"
#include <QString>

class Storage {

    public:
        Storage(const QString path, const QString path2);
        int folderCount();
        std::shared_ptr<FolderItem> getFolder(int index);
        std::shared_ptr<FolderItem> getBaseFolder();
        QString getPath();

    private:
        QString _path;
        QString _path2;
        QVector<std::shared_ptr<FolderItem>> _folders;

};

#endif // STORAGE_H
