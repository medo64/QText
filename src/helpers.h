#ifndef HELPERS_H
#define HELPERS_H

#include <QString>
#include <QWidget>

class Helpers {

    public:
        static QString getFileNameFromTitle(QString fileTitle);
        static QString getFileTitleFromName(QString fileName);
        static QString getFolderNameFromTitle(QString folderTitle);
        static QString getFolderTitleFromName(QString folderName);
        static bool showInFileManager(QString directoryPath, QString filePath);
        static bool openWithDefaultApplication(QString filePath);
        static bool openWithVSCode(QString filePath);
        static bool openWithVSCode(QStringList filePaths);
        static bool openWithVSCodeAvailable();
        static void setReadonlyPalette(QWidget* widget);
        static void replaceDialogIcon(QWidget* dialog);

    private:
        static bool isValidTitleChar(QChar ch);
        static QString getFSNameFromTitle(QString fsTitle, bool isFolder);
        static QString getFSTitleFromName(QString fsName);

};

#endif // HELPERS_H
