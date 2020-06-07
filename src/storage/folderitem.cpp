#include <QDir>
#include <QString>
#include "medo/config.h"
#include "storage/fileitem.h"
#include "storage/folderitem.h"
#include "storage/storage.h"
#include "storage/storagemonitorlocker.h"
#include "deletion.h"
#include "helpers.h"

FolderItem::FolderItem(Storage* storage, FolderItem* rootFolder, const int pathIndex, const QString directoryPath, const QString directoryName) {
    _storage = storage;
    _rootFolder = rootFolder;
    _directoryPath = directoryPath;
    _directoryName = directoryName;
    _pathIndex = pathIndex;

    QDir directory = path();

    QStringList files = directory.entryList(Storage::supportedExtensionFilters(), QDir::Files);
    for (QString fileName : files) {
        addItem(new FileItem(this, fileName));
    }

    loadOrdering();
}

FolderItem::~FolderItem() {
    for (auto file : _files) {
        file->deleteLater();
    }
}


FolderItem* FolderItem::rootFolder() const {
    return _rootFolder;
}

QString FolderItem::name() const {
    if (_pathIndex > 0) {
        return QString::number(_pathIndex + 1) + "/" + ((_directoryName == nullptr) ? "" : _directoryName);
    } else {
        return (_directoryName == nullptr) ? "" : _directoryName;
    }
}

int FolderItem::pathIndex() const {
    return _pathIndex;
}

QString FolderItem::title()  const {
    if (_directoryName == nullptr) {
        QFileInfo path = _directoryPath;
        QString dirName = path.fileName();
        int iParStart = dirName.indexOf('(');
        int iParEnd = dirName.lastIndexOf(')');
        if ((iParStart >= 0) && (iParEnd > iParStart)) {
            return dirName.mid(iParStart, iParEnd - iParStart + 1); //use name in parentheses if one exists
        } else {
            return (_pathIndex == 0) ? "(Default)" : "(Default " + QString::number(_pathIndex + 1) + ")";
        }
    } else {
        return Helpers::getFolderTitleFromName(_directoryName);
    }
}

bool FolderItem::rename(QString newTitle) {
    StorageMonitorLocker lockMonitor(_storage->monitor());

    if (newTitle.isEmpty()) { return false; }
    QString newName = Helpers::getFolderNameFromTitle(newTitle);
    if (newName.compare(_directoryName, Qt::CaseSensitive) == 0) { return false; }

    QDir directory = _directoryPath;
    if (directory.rename(_directoryName, newName)) {
        cleanOrdering();
        _directoryName = newName;
        saveOrdering();
        return true;
    }

    return false;
}

bool FolderItem::isRoot() const {
    return (_directoryName == nullptr);
}

bool FolderItem::isPrimary() const {
    return (_pathIndex == 0);
}


int FolderItem::fileCount() const {
    return _files.size();
}

FileItem* FolderItem::fileAt(int index) const {
    return _files.at(index);
}

FileItem* FolderItem::newFile(QString title, FileType type, FileItem* afterItem) {
    StorageMonitorLocker lockMonitor(_storage->monitor());

    FileItem* file;
    switch (type) {
        case FileType::Markdown: file = new FileItem(this, Helpers::getFileNameFromTitle(title) + "." + Storage::markdownSuffix());   break;
        case FileType::Html:     file = new FileItem(this, Helpers::getFileNameFromTitle(title) + "." + Storage::htmlSuffix()); break;
        default:                 file = new FileItem(this, Helpers::getFileNameFromTitle(title) + "." + Storage::plainSuffix());  break;
    }

    if (afterItem == nullptr) {
        addItem(file);
    } else {
        addItemAfter(file, afterItem);
    }
    saveOrdering();

    return file;
}

bool FolderItem::deleteFile(FileItem* file, DeletionStyle deletionStyle) {
    StorageMonitorLocker lockMonitor(_storage->monitor());

    for (auto item = _files.begin(); item != _files.end(); item++) {
        FileItem* iFile = *item;
        if (iFile->path().compare(file->path(), Qt::CaseSensitive) == 0) {
            bool deleteSuccessful;
            switch (deletionStyle) {
                case DeletionStyle::Recycle:   deleteSuccessful = Deletion::recycleFile(file); break;
                case DeletionStyle::Overwrite: deleteSuccessful = Deletion::overwriteFile(file); break;
                default:                       deleteSuccessful = Deletion::deleteFile(file); break;
            }
            if (deleteSuccessful) {
                _files.erase(item);
                saveOrdering();
            }
            return deleteSuccessful;
        }
    }

    return false;
}


bool FolderItem::fileExists(QString title) const {
    for (auto file : _files) {
        if (file->title().compare(title, Qt::CaseInsensitive) == 0) { return true; }
    }
    return false;
}


bool FolderItem::saveAll() const {
    bool allSaved = true;
    for (FileItem* file : _files) {
        if (file->isModified()) {
            allSaved &= file->save();
        }
    }
    return allSaved;
}


QString FolderItem::path() const {
    if (_directoryName == nullptr) {
        return QDir::cleanPath(_directoryPath);
    } else {
        return QDir::cleanPath(_directoryPath + QDir::separator() + _directoryName);
    }
}

bool FolderItem::moveFile(int from, int to) {
    StorageMonitorLocker lockMonitor(_storage->monitor());

    if ((from < 0) || (from >= _files.count())) { //source outside of range
    } else if ((to < 0) || (to >= _files.count())) { //destination outside of range
    } else if (from == to) { //same position
    } else { //all ok
        _files.move(from, to);
        saveOrdering();
        return true;
    }

    return false;
}


void FolderItem::addItem(FileItem* item) {
    _files.append(item);
}

void FolderItem::addItemAfter(FileItem* item, FileItem* afterItem) {
    for (auto i = 0; i < _files.count(); i++) {
        if (_files[i] == afterItem) {
            _files.insert(i + 1, item);
            return;
        }
    }
    addItem(item); //only if matching afterItem is never found
}

bool FolderItem::removeItem(FileItem* item) {
    for (auto i = 0; i < _files.count(); i++) {
        if (_files[i] == item) {
            _files.remove(i);
            return true;
        }
    }
    return false;
}

void FolderItem::removeItemAt(int index) {
    _files.removeAt(index);
}


void FolderItem::cleanOrdering() {
    Config::stateWriteMany("Order!" + this->name(), QStringList());
}

void FolderItem::saveOrdering() {
    QStringList orderList;
    for (auto file : _files) {
        orderList.append(file->name());
    }
    Config::stateWriteMany("Order!" + this->name(), orderList);
}

void FolderItem::loadOrdering() {
    QStringList ordering = Config::stateReadMany("Order!" + this->name());
    std::sort(_files.begin(), _files.end(), [ordering] (FileItem * item1, FileItem * item2) {
        QString key1 = item1->name();
        QString key2 = item2->name();
        int index1 = ordering.indexOf(key1);
        int index2 = ordering.indexOf(key2);
        if ((index1 == -1) && (index2 == -1)) { //both items are new, just compare alphabetically
            return key1.compare(key2, Qt::CaseSensitive) < 0;
        } else if (index1 == -1) { //second item wins because first item is not in list
            return false;
        } else if (index2 == -1) { //first item wins because second item is not in list
            return true;
        } else { //both have previously stored index
            return (index1 < index2);
        }
    });
}
