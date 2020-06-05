#include <QDebug>
#include <QDir>
#include <QFile>
#include <QRandomGenerator>
#include <QStandardPaths>
#include "deletion.h"

#if defined(Q_OS_WIN)
    #include <windows.h>
#elif defined(Q_OS_LINUX) //no need for imports
#else
    #error "Only Linux and Windows are supported!"
#endif

bool Deletion::deleteFile(QString path) {
    QFile osFile(path);
    return osFile.remove();
}

bool Deletion::overwriteFile(QString path) {
    QFile osFile(path);
    int length = osFile.size();
    osFile.open(QIODevice::WriteOnly | QIODevice::Unbuffered);
    int size = ((length + 4095) / 4096) * 4096; //round size up to the nearest 4K
    char buffer[size];
    QRandomGenerator rnd;
    for (auto i = 0; i < static_cast<int>(sizeof(buffer)); i += 4) {
        qint32 n = rnd.generate();
        memcpy(&buffer[i], &n, 4);
    }
    osFile.seek(0);
    osFile.write(buffer, length);
    osFile.flush();
    return osFile.remove();
}

bool Deletion::recycleFile(QString path) {
    QFile osFile(path);

#if defined(Q_OS_WIN)

    WCHAR fromPathArray[4096];
    memset(&fromPathArray, 0, sizeof(fromPathArray));
    int fromPathLength = path.toWCharArray(fromPathArray);
    if (fromPathLength < 4096) {
        fromPathArray[fromPathLength] = '\0'; //terminate with null char
        SHFILEOPSTRUCTW fileOp;
        memset(&fileOp, 0, sizeof(fileOp));
        fileOp.wFunc = FO_DELETE;
        fileOp.pFrom =  fromPathArray;
        fileOp.fFlags = FOF_NO_UI | FOF_ALLOWUNDO;
        if (SHFileOperation(&fileOp) == 0) {
            return true;
        } else { //just delete if recycle fails
            qDebug().noquote() << "[Deletion] Recycle failed for '" << osFile.fileName() << "'";
            return osFile.remove();
        }
    } else { //don't bother deleting if 4096 characters or longer
        return osFile.remove();
    }

#elif defined(Q_OS_LINUX) //no need for imports

    QString home = QStandardPaths::writableLocation(QStandardPaths::HomeLocation);
    QDir localTrashDir(QDir::cleanPath(home + "/.local/share/Trash"));
    QDir userTrashDir(QDir::cleanPath(home + "/.trash"));
    QDir trashDir;
    if (localTrashDir.exists()) {
        trashDir = localTrashDir;
    } else if (userTrashDir.exists()) {
        trashDir = userTrashDir;
    } else if (userTrashDir.mkdir(".")) { //try creating it if it doesn't exist
        trashDir = userTrashDir;
    } else { //just delete it if recycle is not feasible
        return osFile.remove();
    }

    QFileInfo file(path);
    QFileInfo trashBaseFile(QDir::cleanPath(trashDir.path() + "/" + file.fileName()));
    QString trashPath = trashBaseFile.path();
    for (int i = 2; i < 100; i++) { //check for duplicates up to 100 times, then give up and overwrite
        if (!QFile::exists(trashPath)) { break; }
        trashPath = QDir::cleanPath(trashDir.path() + "/"
                                    + trashBaseFile.completeBaseName() + "~" + QString::number(i)
                                    + "." + trashBaseFile.suffix());
    }

    if (QFile::rename(path, trashPath)) {
        return true;
    } else { //give up and delete
        qDebug().noquote() << "[Deletion] Recycle failed for '" << osFile.fileName() << "' due to '" << osFile.errorString() << "'";
        return osFile.remove();
    }

#else //just delete on unknown platform
    return osFile.remove();
#endif
}
