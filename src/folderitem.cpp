#include "medo/config.h"
#include "fileitem.h"
#include "folderitem.h"
#include "helpers.h"
#include <QDir>
#include <QString>

FolderItem::FolderItem(FolderItem* rootFolder, const int pathIndex, const QString directoryPath, const QString directoryName) {
    _rootFolder = rootFolder;
    _directoryPath = directoryPath;
    _directoryName = directoryName;
    _pathIndex = pathIndex;

    QString path = getPath();
    QDir directory = path;

    QStringList files = directory.entryList(QStringList() << "*.txt" << "*.html", QDir::Files);
    for (QString fileName : files) {
        _files.push_back(new FileItem(this, fileName));
    }

    loadOrdering();
}


FolderItem* FolderItem::getRootFolder() {
    return _rootFolder;
}

QString FolderItem::getKey() {
    if (_pathIndex > 0) {
        return QString::number(_pathIndex + 1) + "/" + ((_directoryName == nullptr) ? "" : _directoryName);
    } else {
        return (_directoryName == nullptr) ? "" : _directoryName;
    }
}

int FolderItem::getPathIndex() {
    return _pathIndex;
}

QString FolderItem::getTitle() {
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
    if (newTitle.isEmpty()) { return false; }
    QString newName = Helpers::getFolderNameFromTitle(newTitle);
    if (newName.compare(_directoryName, Qt::CaseSensitive) == 0) { return false; }

    QDir directory = _directoryPath;
    if (directory.rename(_directoryName, newName)) {
        cleanOrdering();
        _directoryName = newName;
        saveOrdering();
        return true;
    } else {
        return false;
    }
}

bool FolderItem::isRoot() {
    return (_directoryName == nullptr);
}

bool FolderItem::isPrimary() {
    return (_pathIndex == 0);
}


int FolderItem::fileCount() {
    return _files.size();
}

FileItem* FolderItem::getFile(int index) {
    return _files.at(index);
}

FileItem* FolderItem::newFile(QString title) {
    auto file = new FileItem(this, Helpers::getFileNameFromTitle(title) + ".txt");
    _files.push_back(file);
    saveOrdering();
    return file;
}

bool FolderItem::deleteFile(FileItem* file) {
    for (auto item = _files.begin(); item != _files.end(); item++) {
        FileItem* iFile = *item;
        if (iFile->getPath().compare(file->getPath(), Qt::CaseSensitive) == 0) {
            QFile::remove(iFile->getPath());
            _files.erase(item);
            saveOrdering();
            return true;
        }
    }
    return false;
}


bool FolderItem::fileExists(QString title) {
    for (auto file : _files) {
        if (file->getTitle().compare(title, Qt::CaseInsensitive) == 0) { return true; }
    }
    return false;
}


bool FolderItem::saveAll() {
    bool allSaved = true;
    for (int i = 0; i < fileCount(); i++) {
        auto file = getFile(i);
        if (file->isModified()) {
            allSaved &= file->save();
        }
    }
    return allSaved;
}


QString FolderItem::getPath() {
    if (_directoryName == nullptr) {
        return QDir::cleanPath(_directoryPath);
    } else {
        return QDir::cleanPath(_directoryPath + QDir::separator() + _directoryName);
    }
}

bool FolderItem::moveFile(int from, int to) {
    if ((from < 0) || (from >= _files.count())) { return false; }
    if ((to < 0) || (to >= _files.count())) { return false; }
    if (from == to) { return false; }
    _files.move(from, to);
    saveOrdering();
    return true;
}


void FolderItem::cleanOrdering() {
    Config::stateWriteMany("Order!" + this->getKey(), QStringList());
}

void FolderItem::saveOrdering() {
    QStringList orderList;
    for (auto file : _files) {
        orderList.append(file->getKey());
    }
    Config::stateWriteMany("Order!" + this->getKey(), orderList);
}

void FolderItem::loadOrdering() {
    QStringList ordering = Config::stateReadMany("Order!" + this->getKey());
    std::sort(_files.begin(), _files.end(), [ordering] (FileItem * item1, FileItem * item2) {
        QString key1 = item1->getKey();
        QString key2 = item2->getKey();
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
