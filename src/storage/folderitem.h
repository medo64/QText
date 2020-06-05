#pragma once

#include <QString>
#include <QUuid>
#include "fileitem.h"
#include "filetype.h"
#include "settings.h"
#include "storagemonitorthread.h"

class FileItem;
class Storage;

class FolderItem {

    public:
        FolderItem(Storage* storage, FolderItem* rootFolder, const int pathIndex, const QString directoryBase, const QString directoryName);
        FolderItem* rootFolder() const;
        QUuid key() const { return _key; }
        QString name() const;
        QString path() const;
        int pathIndex() const;
        QString title() const;
        bool rename(QString newTitle);
        bool isRoot() const;
        bool isPrimary() const;
        int fileCount() const;
        FileItem* fileAt(int index) const;
        FileItem* newFile(QString title, FileType type, FileItem* afterItem);
        bool deleteFile(FileItem* file, Settings::DeletionStyle deletionStyle);
        bool saveAll() const;
        bool fileExists(QString title) const;
        bool moveFile(int from, int to);

    public:
        inline QVector<FileItem*>::const_iterator begin() const { return _files.constBegin(); }
        inline QVector<FileItem*>::const_iterator end() const { return _files.constEnd(); }

    public:
        FolderItem(const FolderItem&) = delete;
        void operator=(const FolderItem&) = delete;

    private:
        friend class FileItem;
        friend class Storage;
        void addItem(FileItem* item);
        void addItemAfter(FileItem* item, FileItem* afterItem);
        bool removeItem(FileItem* item);
        void removeItemAt(int index);
        Storage* storage() const { return _storage; }

    private:
        QUuid _key = QUuid::createUuid();
        Storage* _storage;
        FolderItem* _rootFolder;
        QString _directoryPath;
        QString _directoryName;
        int _pathIndex;
        QVector<FileItem*> _files;
        void cleanOrdering();
        void saveOrdering();
        void loadOrdering();

};
