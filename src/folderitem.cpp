#include "fileitem.h"
#include "folderitem.h"
#include "helpers.h"
#include <QDir>
#include <QString>

FolderItem::FolderItem(const int pathIndex, const QString directoryPath, const QString directoryName) {
    _directoryPath = directoryPath;
    _directoryName = directoryName;
    _pathIndex = pathIndex;

    QString path = getPath();
    QDir directory = path;

    QStringList files = directory.entryList(QStringList() << "*.txt" << "*.html", QDir::Files);
    for(QString fileName : files) {
        _files.push_back(new FileItem(this, fileName));
    }
}


QString FolderItem::getKey() {
    if (_pathIndex > 0) {
        return QString::number(_pathIndex + 1) + "/" + ((_directoryName == nullptr) ? "" : _directoryName);
    } else {
        return (_directoryName == nullptr) ? "" : _directoryName;
    }
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
    return true;
}
