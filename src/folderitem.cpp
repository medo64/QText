#include "fileitem.h"
#include "folderitem.h"
#include "helpers.h"
#include <QDir>
#include <QString>

FolderItem::FolderItem(const QString prefix, const QString directoryPath, const QString directoryName) {
    _directoryPath = directoryPath;
    _directoryName = directoryName;
    _prefix = prefix;

    QString path = getPath();
    QDir directory = path;

    QStringList files = directory.entryList(QStringList() << "*.txt" << "*.html", QDir::Files);
    for(QString fileName : files) {
        _files.push_back(new FileItem(this, fileName));
    }
}

FolderItem::FolderItem(const QString directoryPath, const QString directoryName) {
    _directoryPath = directoryPath;
    _directoryName = directoryName;

    QString path = getPath();
    QDir directory = path;

    QStringList files = directory.entryList(QStringList() << "*.txt" << "*.html", QDir::Files);
    for(QString fileName : files) {
        _files.push_back(new FileItem(this, fileName));
    }
}


QString FolderItem::getKey() {
    if (_prefix.length() > 0) {
        return _prefix + "/" + ((_directoryName == nullptr) ? "" : _directoryName);
    } else {
        return (_directoryName == nullptr) ? "" : _directoryName;
    }
}

QString FolderItem::getTitle() {
    return (_directoryName == nullptr) ? "(Default" + _prefix + ")" : Helpers::getFolderTitleFromName(_directoryName);
}

size_t FolderItem::fileCount() {
    return _files.size();
}

FileItem* FolderItem::getFile(size_t index) {
    return _files.at(index);
}

FileItem* FolderItem::newFile(QString title) {
    auto file = new FileItem(this, Helpers::getFileNameFromTitle(title) + ".txt");
    _files.push_back(file);
    return file;
}

bool FolderItem::deleteFile(FileItem* file) {
    for(auto item = _files.begin(); item != _files.end(); item++) {
        FileItem* iFile = *item;
        if (iFile->getPath().compare(file->getPath(), Qt::CaseSensitive) == 0) {
            QFile::remove(iFile->getPath());
            _files.erase(item);
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
    for (size_t i = 0; i < fileCount(); i++) {
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
