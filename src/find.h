#pragma once

#include <QList>
#include <QString>
#include "storage/storage.h"

class Find {

    public:
        typedef enum {
            CurrentFile   = 0,
            CurrentFolder = 1,
            AllFolders    = 2,
        } SearchScope;

    public:
        static void setup(Storage* storage, QString text, bool matchCase, bool wholeWord, bool useRegEx, Find::SearchScope searchScope);
        static FileItem* findNext(FileItem* currentFile, bool backward = false);
        static bool hasText();
        static QString lastText();
        static bool lastMatchCase();
        static bool lastWholeWord();
        static bool lastUseRegEx();
        static SearchScope lastSearchScope();

    private:
        static QList<FileItem*> fileList(FileItem* pivotFile, bool backward = false);
        static Storage* _storage;
        static QString _findText;
        static QFlags<QTextDocument::FindFlag> _findFlags;
        static bool _findUseRegEx;
        static FileItem* _firstMatchFile;
        static QTextCursor _firstMatchCursor;
        static bool _firstMatchBackward;
        static SearchScope _findScope;

};
