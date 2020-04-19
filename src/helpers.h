#ifndef HELPERS_H
#define HELPERS_H

#include <QString>
#include <QWidget>
#include "fileitem.h"
#include "folderitem.h"

class Helpers {

    public:
        static QString getFileNameFromTitle(QString fileTitle);
        static QString getFileTitleFromName(QString fileName);
        static QString getFolderNameFromTitle(QString folderTitle);
        static QString getFolderTitleFromName(QString folderName);
        static bool showInFileManager(QString directoryPath, QString filePath);
        static bool openWithDefaultApplication(QString filePath);
        static bool openFileWithVSCode(FileItem* file);
        static bool openFileWithVSCode(QString filePath);
        static bool openDirectoryWithVSCode(FileItem* file);
        static bool openDirectoryWithVSCode(FolderItem* folder);
        static bool openDirectoriesWithVSCode(QStringList directoryPaths);
        static bool openWithVSCodeAvailable();
        static void setReadonlyPalette(QWidget* widget);
        static void setupResizableDialog(QWidget* dialog);
        static void setupFixedSizeDialog(QWidget* dialog);

    private:
        static bool isValidTitleChar(QChar ch);
        static QString getFSNameFromTitle(QString fsTitle, bool isFolder);
        static QString getFSTitleFromName(QString fsName);

};

#endif // HELPERS_H
