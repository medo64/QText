#include <QDebug>
#include <QDir>
#include "deletion.h"
#include "fileitem.h"
#include "folderitem.h"
#include "helpers.h"
#include "storage.h"
#include "storagemonitorlocker.h"
#include "storagemonitorthread.h"

Storage::Storage(const QStringList paths) {
    _monitor = new StorageMonitorThread(paths);

    int index = 0;
    for (QString path : paths) {
        QDir rootDirectory = path.startsWith("~/") ? QDir::homePath() + path.mid(1) : path;
        if (!rootDirectory.exists()) { rootDirectory.mkpath(path); }
        QString cleanedPath = rootDirectory.path();

        auto rootFolder = new FolderItem(this, nullptr, index, cleanedPath, nullptr);
        addItem(rootFolder);
        QStringList directories = rootDirectory.entryList(QDir::Dirs | QDir::NoDotAndDotDot, QDir::SortFlag::Name);
        for (QString directory : directories) {
            auto folder = new FolderItem(this, rootFolder, index, cleanedPath, directory);
            addItem(folder);
        }

        index++;
    }
    sortFolders();

    connect(_monitor, &StorageMonitorThread::directoryAdded, this, &Storage::onDirectoryAdded);
    connect(_monitor, &StorageMonitorThread::fileAdded, this, &Storage::onFileAdded);
    connect(_monitor, &StorageMonitorThread::directoryRemoved, this, &Storage::onDirectoryRemoved);
    connect(_monitor, &StorageMonitorThread::fileRemoved, this, &Storage::onFileRemoved);
}


int Storage::folderCount() const {
    return _folders.size();
}

FolderItem* Storage::folderAt(int index) const {
    return _folders.at(index);
}

FolderItem* Storage::folderFromKey(QUuid key) const {
    for (auto folder : this->_folders) {
        if (folder->key() == key) { return folder; }
    }
    return nullptr;
}

FolderItem* Storage::baseFolder() const {
    return _folders.at(0);
}


FolderItem* Storage::newFolder(QString proposedTitle) {
    StorageMonitorLocker lockMonitor(monitor());

    FolderItem* newFolder = nullptr;
    FolderItem* rootFolder = baseFolder();
    QString rootPath = rootFolder->path();
    QString title = proposedTitle;
    for (int i = 2; i < 100; i++) { //repeat until 100 is reached
        QString dirName = Helpers::getFileNameFromTitle(title);
        QDir dir = QDir::cleanPath(rootPath + "/" + dirName);
        if (!dir.exists()) { //found one that doesn't exist - use this
            if (dir.mkpath(".")) {
                FolderItem* folder = new FolderItem(this, rootFolder, 0, rootPath, dirName);
                addItem(folder);
                sortFolders();
                newFolder = folder; //set only if directory can be created
            }
            break;
        }
        title = proposedTitle + " (" + QString::number(i) + ")"; //add number before trying again
    }

    return newFolder;
}

bool Storage::deleteFolder(FolderItem* folder, DeletionStyle deletionStyle) {
    StorageMonitorLocker lockMonitor(monitor());

    for (int i = 0; i < _folders.count(); i++) {
        FolderItem* iFolder = _folders[i];
        if (iFolder == folder) {
            bool deleteSuccessful;
            switch (deletionStyle) {
                case DeletionStyle::Recycle:   deleteSuccessful = Deletion::recycleFolder(folder); break;
                case DeletionStyle::Overwrite: deleteSuccessful = Deletion::overwriteFolder(folder); break;
                default:                       deleteSuccessful = Deletion::deleteFolder(folder); break;
            }
            if (deleteSuccessful) {
                removeItemAt(i);
            }
            return deleteSuccessful;
        }
    }

    return false;
}

StorageMonitorThread* Storage::monitor() const {
    return _monitor;
}


void Storage::addItem(FolderItem* item) {
    _folders.append(item);
}

void Storage::removeItemAt(int index) {
    _folders.removeAt(index);
}


void Storage::sortFolders() {
    std::sort(_folders.begin(), _folders.end(), [] (FolderItem * item1, FolderItem * item2) {
        int index1 = item1->pathIndex();
        int index2 = item2->pathIndex();
        if (index1 == index2) { //both items are same level, just compare alphabetically
            return item1->title().compare(item2->title(), Qt::CaseInsensitive) < 0;
        } else { //different levels
            return (index1 < index2);
        }
    });
}


void Storage::onDirectoryAdded(QString folderPath) {
    for (FolderItem* folder : _folders) { //find duplicates
        if (folder->path() == folderPath) { return; } //already there
    }

    FolderItem* rootFolder = nullptr;
    QFileInfo folderInfo = folderPath;
    auto rootPath = folderInfo.dir().path();
    for (FolderItem* folder : _folders) { //find root folder
        if (!folder->isRoot()) { continue; }
        if (folder->path() == rootPath) {
            rootFolder = folder;
            break;
        }
    }

    if (rootFolder != nullptr) { //create new item
        qDebug().noquote().nospace() << "[StorageMonitorThread] onDirectoryAdded(\"" << rootFolder->path() << "\", \"" << folderInfo.fileName() << "\") #" << QThread::currentThreadId();
        auto folder = new FolderItem(this, rootFolder, rootFolder->pathIndex(), rootFolder->path(), folderInfo.fileName());
        _folders.append(folder);
        sortFolders();

        emit updatedFolder(nullptr);
    }
}

void Storage::onDirectoryRemoved(QString folderPath) {
    for (int i = 0; i < _folders.count(); i++) { //find item
        FolderItem* iFolder = _folders[i];
        QString iFolderPath = iFolder->path();
        if (iFolderPath == folderPath) { //remove item
            removeItemAt(i);

            QFileInfo iFolderInfo = iFolderPath;
            qDebug().noquote().nospace() << "[StorageMonitorThread] onDirectoryRemoved(\"" << iFolderInfo.dir().path() << "\", \"" << iFolderInfo.fileName() << "\") #" << QThread::currentThreadId();
            emit updatedFolder(nullptr);
            break;
        }
    }
}

void Storage::onFileAdded(QString folderPath, QString fileName) {
    FolderItem* folder = nullptr;
    for (FolderItem* iFolder : _folders) { //find owner folder
        if (iFolder->path() == folderPath) {
            folder = iFolder;
            break;
        }
    }

    if (folder != nullptr) {
        for (FileItem* file : *folder) { //find duplicates
            if (file->name() == fileName) { return; } //already there
        }

        folder->addItem(new FileItem(folder, fileName)); //add item

        qDebug().noquote().nospace() << "[StorageMonitorThread] onFileAdded(\"" << folder->path() << "\", \"" << fileName << "\") #" << QThread::currentThreadId();
        emit updatedFolder(folder);
    }
}

void Storage::onFileRemoved(QString folderPath, QString fileName) {
    FolderItem* folder = nullptr;
    for (FolderItem* iFolder : _folders) { //find owner folder
        if (iFolder->path() == folderPath) {
            folder = iFolder;
            break;
        }
    }

    if (folder != nullptr) {
        for (int i = 0; i < folder->fileCount(); i++) { //find item
            FileItem* iFile = folder->fileAt(i);
            QString iFileName = iFile->name();
            if (iFileName == fileName) { //remove item
                folder->removeItemAt(i);

                qDebug().noquote().nospace() << "[StorageMonitorThread] onFileRemoved(\"" << folderPath << "\", \"" << fileName << "\") #" << QThread::currentThreadId();
                emit updatedFolder(folder);
                break;
            }
        }
    }
}
