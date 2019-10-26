#include "storage.h"
#include "folderitem.h"
#include <QDir>

Storage::Storage(const QString path, QString path2) {
    QDir rootDirectory = path.startsWith("~/") ? QDir::homePath() + path.mid(1) : path;
    if (!rootDirectory.exists()) { rootDirectory.mkpath(path); }

    _path = rootDirectory.path();

    _folders.push_back(std::make_shared<FolderItem>(_path, nullptr));
    QStringList directories = rootDirectory.entryList(QDir::Dirs|QDir::NoDotAndDotDot, QDir::SortFlag::Name);
    for(QString directory : directories) {
        auto folder = std::make_shared<FolderItem>(_path, directory);
        _folders.push_back(folder);
    }

    if (path2.length() > 0) { //append second directory too
        QDir rootDirectory2 = path.startsWith("~/") ? QDir::homePath() + path.mid(1) : path2;
        if (!rootDirectory2.exists()) { rootDirectory2.mkpath(path2); }

        _path2 = rootDirectory2.path();

        _folders.push_back(std::make_shared<FolderItem>("2", _path2, nullptr));
        QStringList directories2 = rootDirectory2.entryList(QDir::Dirs|QDir::NoDotAndDotDot, QDir::SortFlag::Name);
        for(QString directory2 : directories2) {
            auto folder2 = std::make_shared<FolderItem>("2", _path2, directory2);
            _folders.push_back(folder2);
        }
    }
}


int Storage::folderCount() {
    return _folders.size();
}

std::shared_ptr<FolderItem> Storage::getFolder(int index) {
    return _folders.at(index);
}

std::shared_ptr<FolderItem> Storage::getBaseFolder() {
    return _folders.at(0);
}


QString Storage::getPath() {
    return _path;
}
