#ifndef FIND_H
#define FIND_H

#include <QList>
#include <QString>
#include "storage.h"

class Find {

    public:
        static void setup(Storage* storage, QString text, bool matchCase, bool wholeWord);
        static FileItem* findNext(FileItem* currentFile);
        static bool hasText();
        static QString lastText();
        static bool lastMatchCase();
        static bool lastWholeWord();

    private:
        static QList<FileItem*> fileList(FileItem* pivotFile);
        static Storage* _storage;
        static QString _findText;
        static QFlags<QTextDocument::FindFlag> _findFlags;
        static FileItem* _firstMatchFile;
        static QTextCursor _firstMatchCursor;

};

#endif // FIND_H
