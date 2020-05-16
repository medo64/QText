#include <QDebug>
#include <QDir>
#include "fileitem.h"
#include "folderitem.h"
#include "helpers.h"
#include "storage.h"
#include "storagemonitorthread.h"

Storage::Storage(const QStringList paths) {
    _monitor = new StorageMonitorThread(paths);

    int index = 0;
    for (QString path : paths) {
        QDir rootDirectory = path.startsWith("~/") ? QDir::homePath() + path.mid(1) : path;
        if (!rootDirectory.exists()) { rootDirectory.mkpath(path); }
        QString cleanedPath = rootDirectory.path();

        auto rootFolder = new FolderItem(this, nullptr, index, cleanedPath, nullptr);
        _folders.push_back(rootFolder);
        QStringList directories = rootDirectory.entryList(QDir::Dirs | QDir::NoDotAndDotDot, QDir::SortFlag::Name);
        for (QString directory : directories) {
            auto folder = new FolderItem(this, rootFolder, index, cleanedPath, directory);
            _folders.push_back(folder);
        }

        index++;
    }
    sortFolders();

    connect(_monitor, &StorageMonitorThread::directoryAdded, this, &Storage::onDirectoryAdded);
    connect(_monitor, &StorageMonitorThread::fileAdded, this, &Storage::onFileAdded);
    connect(_monitor, &StorageMonitorThread::directoryRemoved, this, &Storage::onDirectoryRemoved);
    connect(_monitor, &StorageMonitorThread::fileRemoved, this, &Storage::onFileRemoved);
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
    _monitor->stopMonitoring();

    FolderItem* newFolder = nullptr;
    FolderItem* rootFolder = getBaseFolder();
    QString rootPath = rootFolder->getPath();
    QString title = proposedTitle;
    for (int i = 2; i < 100; i++) { //repeat until 100 is reached
        QString dirName = Helpers::getFileNameFromTitle(title);
        QDir dir = QDir::cleanPath(rootPath + "/" + dirName);
        if (!dir.exists()) { //found one that doesn't exist - use this
            if (dir.mkpath(".")) {
                FolderItem* folder = new FolderItem(this, rootFolder, 0, rootPath, dirName);
                _folders.push_back(folder);
                sortFolders();
                newFolder = folder; //set only if directory can be created
            }
            break;
        }
        title = proposedTitle + " (" + QString::number(i) + ")"; //add number before trying again
    }

    _monitor->continueMonitoring();
    return newFolder;
}

bool Storage::deleteFolder(FolderItem* folder) {
    _monitor->stopMonitoring();

    bool result = false;
    for (int i = 0; i < _folders.count(); i++) {
        FolderItem* iFolder = _folders[i];
        if (iFolder->isRoot()) { continue; } //skip root folders
        if (iFolder == folder) {
            QDir directory(iFolder->getPath());
            if (directory.removeRecursively()) {
                _folders.removeAt(i);
                result = true;
                break;
            }
        }
    }

    _monitor->continueMonitoring();
    return result;
}

StorageMonitorThread* Storage::monitor() {
    return _monitor;
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


void Storage::onDirectoryAdded(QString folderPath) {
    for (FolderItem* folder : _folders) { //find duplicates
        if (folder->getPath() == folderPath) { return; } //already there
    }

    FolderItem* rootFolder = nullptr;
    QFileInfo folderInfo = folderPath;
    auto rootPath = folderInfo.dir().path();
    for (FolderItem* folder : _folders) { //find root folder
        if (!folder->isRoot()) { continue; }
        if (folder->getPath() == rootPath) {
            rootFolder = folder;
            break;
        }
    }

    if (rootFolder != nullptr) { //create new item
        qDebug().noquote().nospace() << "[StorageMonitorThread] onDirectoryAdded(\"" << rootFolder->getPath() << "\", \"" << folderInfo.fileName() << "\")";
        auto folder = new FolderItem(this, rootFolder, rootFolder->getPathIndex(), rootFolder->getPath(), folderInfo.fileName());
        _folders.append(folder);
        sortFolders();

        emit updatedFolder(nullptr);
    }
}

void Storage::onDirectoryRemoved(QString folderPath) {
    for (int i = 0; i < _folders.count(); i++) { //find item
        FolderItem* iFolder = _folders[i];
        QString iFolderPath = iFolder->getPath();
        if (iFolderPath == folderPath) { //remove item
            _folders.removeAt(i);

            QFileInfo iFolderInfo = iFolderPath;
            qDebug().noquote().nospace() << "[StorageMonitorThread] onDirectoryRemoved(\"" << iFolderInfo.dir().path() << "\", \"" << iFolderInfo.fileName() << "\")";
            emit updatedFolder(nullptr);
            break;
        }
    }
}

void Storage::onFileAdded(QString folderPath, QString fileName) {
    FolderItem* folder = nullptr;
    for (FolderItem* iFolder : _folders) { //find owner folder
        if (iFolder->getPath() == folderPath) {
            folder = iFolder;
            break;
        }
    }

    if (folder != nullptr) {
        for (int i = 0; i < folder->fileCount(); i++ ) { //find duplicates
            FileItem* file = folder->getFile(i);
            if (file->getKey() == fileName) { return; } //already there
        }

        folder->addItem(new FileItem(folder, fileName)); //add item

        qDebug().noquote().nospace() << "[StorageMonitorThread] onFileAdded(\"" << folder->getPath() << "\", \"" << fileName << "\")";
        emit updatedFolder(folder);
    }
}

void Storage::onFileRemoved(QString folderPath, QString fileName) {
    FolderItem* folder = nullptr;
    for (FolderItem* iFolder : _folders) { //find owner folder
        if (iFolder->getPath() == folderPath) {
            folder = iFolder;
            break;
        }
    }

    if (folder != nullptr) {
        for (int i = 0; i < folder->fileCount(); i++) { //find item
            FileItem* iFile = folder->getFile(i);
            QString iFileName = iFile->getKey();
            if (iFileName == fileName) { //remove item
                folder->removeItem(i);

                qDebug().noquote().nospace() << "[StorageMonitorThread] onFileRemoved(\"" << folderPath << "\", \"" << fileName << "\")";
                emit updatedFolder(folder);
                break;
            }
        }
    }
}
