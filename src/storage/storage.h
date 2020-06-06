#pragma once

#include <QString>
#include "folderitem.h"
#include "storagemonitorthread.h"

const QString _plainSuffix = "txt";
const QString _htmlSuffix = "html";
const QString _markdownSuffix = "md";
const QStringList _supportedExtensions = QStringList({"." + _plainSuffix, "." + _htmlSuffix, "." + _markdownSuffix});
const QStringList _supportedExtensionFilters = QStringList({"*." + _plainSuffix, "*." + _htmlSuffix, "*." + _markdownSuffix});

class StorageInternal : public QObject { //just to keep private variables not visible to friends
        friend class Storage;

    private:
        QVector<FolderItem*> _folders;
        StorageMonitorThread* _monitor;

};

class Storage : public StorageInternal {
        Q_OBJECT
        friend class FileItem;
        friend class FolderItem;
        friend class StorageMonitorThread;

    public:
        Storage(const QStringList paths);
        int folderCount() const;
        FolderItem* folderAt(int index) const;
        FolderItem* folderFromKey(QUuid key) const;
        FolderItem* baseFolder() const;
        FolderItem* newFolder(QString title);
        bool deleteFolder(FolderItem* folder, DeletionStyle deletionStyle);
        StorageMonitorThread* monitor() const;

    public:
        inline QVector<FolderItem*>::const_iterator begin() const { return _folders.constBegin(); }
        inline QVector<FolderItem*>::const_iterator end() const { return _folders.constEnd(); }

    public:
        Storage(const Storage&) = delete;
        void operator=(const Storage&) = delete;

    private:
        static QString plainSuffix() { return _plainSuffix; };
        static QString htmlSuffix() { return _htmlSuffix; };
        static QString markdownSuffix() { return _markdownSuffix; };
        static QStringList supportedExtensions() { return _supportedExtensions; };
        static QStringList supportedExtensionFilters() { return _supportedExtensionFilters; };
        void addItem(FolderItem* item);
        void removeItemAt(int index);

    private:
        void sortFolders();

    private slots:
        void onDirectoryAdded(QString folderPath);
        void onDirectoryRemoved(QString folderPath);
        void onFileAdded(QString folderPath, QString fileName);
        void onFileRemoved(QString folderPath, QString fileName);

    signals:
        void updatedFolder(FolderItem* folder);

};
