#include "helpers.h"
#include "storage.h"
#include "folderitem.h"
#include <QDir>

Storage::Storage(const QStringList paths) {
    int index = 0;
    for (QString path : paths) {
        QDir rootDirectory = path.startsWith("~/") ? QDir::homePath() + path.mid(1) : path;
        if (!rootDirectory.exists()) { rootDirectory.mkpath(path); }
        QString cleanedPath = rootDirectory.path();

        auto rootFolder = new FolderItem(nullptr, index, cleanedPath, nullptr);
        _folders.push_back(rootFolder);
        QStringList directories = rootDirectory.entryList(QDir::Dirs | QDir::NoDotAndDotDot, QDir::SortFlag::Name);
        for (QString directory : directories) {
            auto folder = new FolderItem(rootFolder, index, cleanedPath, directory);
            _folders.push_back(folder);
        }

        index++;
    }
    sortFolders();
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


FolderItem* Storage::newFolder(QString proposedTitle) {
    FolderItem* rootFolder = getBaseFolder();
    QString rootPath = rootFolder->getPath();
    QString title = proposedTitle;
    for (int i = 2; i < 100; i++) { //repeat until 100 is reached
        QString dirName = Helpers::getFileNameFromTitle(title);
        QDir dir = QDir::cleanPath(rootPath + "/" + dirName);
        if (!dir.exists()) { //found one that doesn't exist - use this
            if (dir.mkpath(".")) {
                FolderItem* folder = new FolderItem(rootFolder, 0, rootPath, dirName);
                _folders.push_back(folder);
                sortFolders();
                return folder;
            } else {
                return nullptr; //cannot create directory
            }
        }
        title = proposedTitle + " (" + QString::number(i) + ")"; //add number before trying again
    }
    return nullptr; //give up
}

bool Storage::deleteFolder(FolderItem* folder) {
    for (int i = 0; i < _folders.count(); i++) {
        FolderItem* iFolder = _folders[i];
        if (iFolder->isRoot()) { continue; } //skip root folders
        if (iFolder == folder) {
            QDir directory(iFolder->getPath());
            if (directory.removeRecursively()) {
                _folders.removeAt(i);
                return true;
            }
        }
    }
    return false;
}


void Storage::sortFolders() {
    std::sort(_folders.begin(), _folders.end(), [] (FolderItem * item1, FolderItem * item2) {
        int index1 = item1->getPathIndex();
        int index2 = item2->getPathIndex();
        if (index1 == index2) { //both items are same level, just compare alphabetically
            return item1->getTitle().compare(item2->getTitle(), Qt::CaseInsensitive) < 0;
        } else { //different levels
            return (index1 < index2);
        }
    });
}
