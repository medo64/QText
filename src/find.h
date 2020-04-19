#ifndef FIND_H
#define FIND_H

#include <QList>
#include <QString>
#include "storage.h"

class Find {

    public:
        static void setup(QString text, Storage* storage);
        static FileItem* findNext(FileItem* currentFile);
        static QString lastText();
        static bool hasText();

    private:
        static QList<FileItem*> fileList(FileItem* pivotFile);
        static QString _text;
        static Storage* _storage;
        static FileItem* _firstMatchFile;
        static QTextCursor _firstMatchCursor;

};

#endif // FIND_H
