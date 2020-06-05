#pragma once

#include <QString>
#include "folderitem.h"
#include "storagemonitorthread.h"

class Storage : public QObject {
        Q_OBJECT

    public:
        Storage(const QStringList paths);
        int folderCount() const;
        FolderItem* folderAt(int index) const;
        FolderItem* folderFromKey(QUuid key) const;
        FolderItem* baseFolder() const;
        FolderItem* newFolder(QString title);
        bool deleteFolder(FolderItem* folder);
        StorageMonitorThread* monitor() const;

    public:
        inline QVector<FolderItem*>::const_iterator begin() const { return _folders.constBegin(); }
        inline QVector<FolderItem*>::const_iterator end() const { return _folders.constEnd(); }

    public:
        Storage(const Storage&) = delete;
        void operator=(const Storage&) = delete;

    private:
        friend class FileItem;
        friend class FolderItem;
        friend class StorageMonitorThread;
        static QStringList supportedExtensions();
        static QStringList supportedExtensionFilters();
        void addItem(FolderItem* item);
        void removeItemAt(int index);

    private:
        QVector<FolderItem*> _folders;
        StorageMonitorThread* _monitor;
        void sortFolders();

    private slots:
        void onDirectoryAdded(QString folderPath);
        void onDirectoryRemoved(QString folderPath);
        void onFileAdded(QString folderPath, QString fileName);
        void onFileRemoved(QString folderPath, QString fileName);

    signals:
        void updatedFolder(FolderItem* folder);

};
