#ifndef FIND_H
#define FIND_H

#include <QList>
#include <QString>
#include "storage.h"

class Find {

    public:
        static void setup(Storage* storage, QString text, bool matchCase, bool wholeWord, bool useRegEx);
        static FileItem* findNext(FileItem* currentFile, bool backward = false);
        static bool hasText();
        static QString lastText();
        static bool lastMatchCase();
        static bool lastWholeWord();
        static bool lastUseRegEx();

    private:
        static QList<FileItem*> fileList(FileItem* pivotFile, bool backward = false);
        static Storage* _storage;
        static QString _findText;
        static QFlags<QTextDocument::FindFlag> _findFlags;
        static bool _findUseRegEx;
        static FileItem* _firstMatchFile;
        static QTextCursor _firstMatchCursor;
        static bool _firstMatchBackward;

};

#endif // FIND_H
