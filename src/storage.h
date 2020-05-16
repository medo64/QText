#ifndef STORAGE_H
#define STORAGE_H

#include <QString>
#include "folderitem.h"
#include "storagemonitorthread.h"

class Storage : public QObject {
        Q_OBJECT

    public:
        Storage(const QStringList paths);
        int folderCount();
        FolderItem* getFolder(int index);
        FolderItem* getBaseFolder();
        QString getPath();
        FolderItem* newFolder(QString title);
        bool deleteFolder(FolderItem* folder);
        StorageMonitorThread* monitor();

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

#endif // STORAGE_H
