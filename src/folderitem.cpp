#include "fileitem.h"
#include "folderitem.h"
#include "helpers.h"
#include <QDir>
#include <QString>

FolderItem::FolderItem(const QString directoryPath, const QString directoryName) {
    _directoryPath = directoryPath;
    _directoryName = directoryName;

    QString path = getPath();
    QDir directory = path;

    _files = std::make_shared<std::vector<FileItem*>>();
    QStringList files = directory.entryList(QStringList() << "*.txt" << "*.html", QDir::Files);
    for(QString fileName : files) {
        _files->push_back(new FileItem(path, fileName));
    }
}


QString FolderItem::getTitle() {
    return (_directoryName == nullptr) ? "(Default)" : Helpers::getTitleFromFSName(_directoryName);
}

size_t FolderItem::fileCount() {
    return _files->size();
}

FileItem* FolderItem::getFile(size_t index) {
    return _files->at(index);
}

FileItem* FolderItem::newFile(QString title) {
    auto file = new FileItem(this->getPath(), Helpers::getFSNameFromTitle(title) + ".txt");
    _files->push_back(file);
    return file;
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
