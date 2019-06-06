#include "storage.h"
#include "folderitem.h"
#include <QDir>
#include <QString>

Storage::Storage(const QString path) {
    QDir rootDirectory = path;
    if (!rootDirectory.exists()) {
        rootDirectory.mkpath(path);
    }

    _path = rootDirectory.path();

    _folders.push_back(std::make_shared<FolderItem>(path, nullptr));
    QStringList directories = rootDirectory.entryList(QDir::Dirs|QDir::NoDotAndDotDot, QDir::SortFlag::Name);
    for(QString directory : directories) {
        auto folder = std::make_shared<FolderItem>(path, directory);
        _folders.push_back(folder);
    }
}


size_t Storage::folderCount() {
    return _folders.size();
}

std::shared_ptr<FolderItem> Storage::getFolder(size_t index) {
    return _folders.at(index);
}

std::shared_ptr<FolderItem> Storage::getBaseFolder() {
    return _folders.at(0);
}


QString Storage::getPath() {
    return _path;
}
