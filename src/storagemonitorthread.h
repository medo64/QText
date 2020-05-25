#pragma once

#include <QMutex>
#include <QString>
#include <QStringList>
#include <QThread>

class StorageMonitorThread : public QThread {
        Q_OBJECT

    public:
        explicit StorageMonitorThread(QStringList paths);
        ~StorageMonitorThread();
        void continueMonitoring();
        void stopMonitoring();

    public:
        StorageMonitorThread(const StorageMonitorThread&) = delete;
        void operator=(const StorageMonitorThread&) = delete;

    private:
        QMutex _mutex;
        bool _isMonitoring = true;
        QStringList _paths;
        QStringList _prevPaths;
        void run();
        QStringList findPaths(QString rootPath, int depth = 0);

    signals:
        void directoryAdded(QString folderPath);
        void directoryRemoved(QString folderPath);
        void fileAdded(QString folderPath, QString fileName);
        void fileRemoved(QString folderPath, QString fileName);

};
