#pragma once

#include <QString>
#include "fileitem.h"
#include "settings.h"
#include "storagemonitorthread.h"

class FileItem;
class Storage;

class FolderItem {

    public:
        FolderItem(Storage* storage, FolderItem* rootFolder, const int pathIndex, const QString directoryBase, const QString directoryName);
        FolderItem* rootFolder();
        QString name();
        QString path();
        int pathIndex();
        QString title();
        bool rename(QString newTitle);
        bool isRoot();
        bool isPrimary();
        int fileCount();
        FileItem* fileAt(int index);
        FileItem* newFile(QString title, FileItem* afterItem);
        bool deleteFile(FileItem* file, Settings::DeletionStyle deletionStyle);
        bool saveAll();
        bool fileExists(QString title);
        bool moveFile(int from, int to);
        StorageMonitorThread* monitor();

    public:
        void addItem(FileItem* item);
        void addItemAfter(FileItem* item, FileItem* afterItem);
        void removeItemAt(int index);

    public:
        inline QVector<FileItem*>::const_iterator begin() const { return _files.constBegin(); }
        inline QVector<FileItem*>::const_iterator end() const { return _files.constEnd(); }

    private:
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
