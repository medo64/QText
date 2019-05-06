#include "fileitem.h"
#include "folderitem.h"
#include <QDir>
#include <QString>

FolderItem::FolderItem(const QString directoryPath, const QString directoryName) {
    _directoryPath = directoryPath;
    _directoryName = directoryName;

    QString path = getPath();
    QDir directory = path;

    _files = std::make_shared<std::vector<std::shared_ptr<FileItem>>>();
    QStringList files = directory.entryList(QStringList() << "*.txt" << "*.html", QDir::Files);
    for(QString fileName : files) {
        _files->push_back(std::make_shared<FileItem>(path, fileName));
    }
}


QString FolderItem::getTitle() {
    return (_directoryName == nullptr) ? "(Default)" : _directoryName;
}

size_t FolderItem::fileCount() {
    return _files->size();
}

std::shared_ptr<FileItem> FolderItem::getFile(size_t index) {
    return _files->at(index);
}


QString FolderItem::getPath() {
    if (_directoryName == nullptr) {
        return QDir::cleanPath(_directoryPath);
    } else {
        return QDir::cleanPath(_directoryPath + QDir::separator() + _directoryName);
    }
}
