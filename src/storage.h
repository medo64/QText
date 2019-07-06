#ifndef STORAGE_H
#define STORAGE_H

#include "folderitem.h"
#include <QString>

class Storage {

    public:
        Storage(const QString path);
        size_t folderCount();
        std::shared_ptr<FolderItem> getFolder(size_t index);
        std::shared_ptr<FolderItem> getBaseFolder();
        QString getPath();

    private:
        QString _path;
        std::vector<std::shared_ptr<FolderItem>> _folders;

};

#endif // STORAGE_H
