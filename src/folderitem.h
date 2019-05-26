#ifndef FOLDERITEM_H
#define FOLDERITEM_H

#include "fileitem.h"
#include <memory>
#include <vector>
#include <QString>

class FolderItem {

    public:
        FolderItem(const QString directoryBase, const QString directoryName);
        QString getTitle();
        size_t fileCount();
        FileItem* getFile(size_t index);

    private:
        QString _directoryPath;
        QString _directoryName;
        QString getPath();
        std::shared_ptr<std::vector<FileItem*>> _files = nullptr;

};

#endif // FOLDERITEM_H
