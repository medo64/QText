#include <QDebug>
#include <QDir>
#include "storage.h"
#include "storagemonitorthread.h"

StorageMonitorThread::StorageMonitorThread(QStringList paths) {
    _paths = paths;
    this->setObjectName("StorageMonitor");
    this->start(LowPriority);
}

StorageMonitorThread::~StorageMonitorThread() {
    this->requestInterruption();
}


void StorageMonitorThread::continueMonitoring() {
    qDebug().noquote().nospace() << "[StorageMonitorThread] continueMonitoring() #" << QThread::currentThreadId();
    _mutex.lock();
    _prevPaths.clear();
    if (_isMonitoring) { qWarning().noquote().nospace() << "W: [StorageMonitorThread] Already monitoring #" << QThread::currentThreadId(); }
    _isMonitoring = true;
    _mutex.unlock();
}

void StorageMonitorThread::stopMonitoring() {
    qDebug().noquote().nospace() << "[StorageMonitorThread] stopMonitoring() #" << QThread::currentThreadId();
    _mutex.lock();
    if (!_isMonitoring) { qWarning().noquote().nospace() << "W: [StorageMonitorThread] Already not monitoring #" << QThread::currentThreadId(); }
    _isMonitoring = false;
    _mutex.unlock();
}


void StorageMonitorThread::run() {
    while (!this->isInterruptionRequested()) {
        _mutex.lock(); //just check monitoring
        bool isMonitoring = _isMonitoring;
        _mutex.unlock();

        QStringList currPaths;
        if (isMonitoring) { //collect only when monitoring is not stopped
            for (QString path : _paths) {
                currPaths.append(findPaths(path));
            }
        }

        _mutex.lock(); //lock raising events
        if (_isMonitoring && (currPaths.count() > 0) && (_prevPaths.count() > 0)) { //analyze add/remove
            QStringList addedDirs, addedFiles, removedDirs, removedFiles;

            //additions
            for (QString path : currPaths) {
                if (!_prevPaths.contains(path)) {
                    if (path.endsWith("/")) {
                        addedDirs.append(QDir::cleanPath(path));
                    } else {
                        addedFiles.append(path);
                    }
                } else {
                    _prevPaths.removeOne(path);
                }
            }

            //removals
            for (QString path : _prevPaths) {
                if (path.endsWith("/")) {
                    removedDirs.append(QDir::cleanPath(path));
                } else {
                    removedFiles.append(path);
                }
            }

            //emit signals
            for (QString path : removedDirs) {
                qDebug().noquote().nospace() << "[StorageMonitorThread] directoryRemoved(\"" << path << "\") #" << QThread::currentThreadId();
                emit directoryRemoved(path);
            }
            for (QString path : removedFiles) {
                QFileInfo file(path);
                QString dirPath = QDir::cleanPath(file.dir().path());
                QString fileName = file.fileName();
                if (!removedDirs.contains(dirPath)) { //filter out files where folder was removed
                    qDebug().noquote().nospace() << "[StorageMonitorThread] fileRemoved(\"" << dirPath << "\", \"" << fileName << "\") #" << QThread::currentThreadId();
                    emit fileRemoved(dirPath, fileName);
                }
            }
            for (QString path : addedDirs) {
                qDebug().noquote().nospace() << "[StorageMonitorThread] directoryAdded(\"" << path << "\") #" << QThread::currentThreadId();
                emit directoryAdded(path);
            }
            for (QString path : addedFiles) {
                QFileInfo file(path);
                QString dirPath = QDir::cleanPath(file.dir().path());
                QString fileName = file.fileName();
                if (!addedDirs.contains(dirPath)) { //filter out files where folder was added
                    qDebug().noquote().nospace() << "[StorageMonitorThread] fileAdded(\"" << dirPath << "\", \"" << fileName << "\") #" << QThread::currentThreadId();
                    emit fileAdded(dirPath, fileName);
                }
            }
        }
        _prevPaths = currPaths;
        _mutex.unlock(); //done changing paths

        this->msleep(250);
    }
}


QStringList StorageMonitorThread::findPaths(QString rootPath, int depth) {
    QStringList list;

    QDir rootDirectory = QDir::cleanPath(rootPath.startsWith("~/") ? QDir::homePath() + rootPath.mid(1) : rootPath);
    if (rootDirectory.exists()) {
        QString rootDirectoryPath = rootDirectory.path();
        if (!rootDirectoryPath.endsWith("/")) { rootDirectoryPath += "/"; }

        list.append(rootDirectoryPath);

        for (QString fileName : rootDirectory.entryList(Storage::supportedExtensionFilters(), QDir::Files, QDir::SortFlag::Name)) {
            list.append(rootDirectoryPath + fileName);
        }

        if (depth < 1) { //don't go to deep into directories
            for (QString directoryName : rootDirectory.entryList(QDir::Dirs | QDir::NoDotAndDotDot, QDir::SortFlag::Name)) {
                list.append(findPaths(rootDirectoryPath + directoryName, depth + 1));
            }
        }
    }

    return list;
}
