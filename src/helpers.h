#ifndef HELPERS_H
#define HELPERS_H

#include <QString>

class Helpers {

    public:
        static QString getFileNameFromTitle(QString fileTitle);
        static QString getFileTitleFromName(QString fileName);
        static QString getFolderNameFromTitle(QString folderTitle);
        static QString getFolderTitleFromName(QString folderName);

    private:
        static bool isValidTitleChar(QChar ch);
        static QString getFSNameFromTitle(QString fsTitle, bool isFolder);
        static QString getFSTitleFromName(QString fsName);

};

#endif // HELPERS_H
