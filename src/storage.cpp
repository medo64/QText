#include "storage.h"
#include "folderitem.h"
#include <QDir>

Storage::Storage(const QStringList paths) {
    int index = 0;
    for (QString path : paths) {
        QDir rootDirectory = path.startsWith("~/") ? QDir::homePath() + path.mid(1) : path;
        if (!rootDirectory.exists()) { rootDirectory.mkpath(path); }
        QString cleanedPath = rootDirectory.path();

        _folders.push_back(new FolderItem(index, cleanedPath, nullptr));
        QStringList directories = rootDirectory.entryList(QDir::Dirs|QDir::NoDotAndDotDot, QDir::SortFlag::Name);
        for(QString directory : directories) {
            auto folder = new FolderItem(index, cleanedPath, directory);
            _folders.push_back(folder);
        }

        index++;
    }
}


int Storage::folderCount() {
    return _folders.size();
}

FolderItem* Storage::getFolder(int index) {
    return _folders.at(index);
}

FolderItem* Storage::getBaseFolder() {
    return _folders.at(0);
}
