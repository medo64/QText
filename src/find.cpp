#include <QApplication>
#include <QTextDocument>

#include "find.h"


Storage* Find::_storage = nullptr;
QString Find::_findText;
QFlags<QTextDocument::FindFlag> Find::_findFlags;
FileItem* Find::_firstMatchFile = nullptr;
QTextCursor Find::_firstMatchCursor;
bool Find::_lastMatchBackward;


void Find::setup(Storage* storage, QString text, bool matchCase, bool wholeWord) {
    _storage = storage;

    _findText = text;
    _findFlags = QTextDocument::FindFlag();
    if (matchCase) { _findFlags |= QTextDocument::FindCaseSensitively; }
    if (wholeWord) { _findFlags |= QTextDocument::FindWholeWords; }

    _firstMatchFile = nullptr;
    _firstMatchCursor = QTextCursor();
    _lastMatchBackward = false;
}

FileItem* Find::findNext(FileItem* currentFile, bool backward) {
    bool firstFile = true;
    for(auto file : fileList(currentFile, backward)) {
        auto cursor = file->textCursor();
        if (!firstFile) { cursor.movePosition(backward ? QTextCursor::End : QTextCursor::Start); }
        auto flags = backward ? _findFlags | QTextDocument::FindBackward : _findFlags;

        auto document = file->document();
        auto resultCursor = document->find(_findText, cursor, flags);

        if (!resultCursor.isNull()) {
            if (_firstMatchFile == nullptr) { //save first match
                _firstMatchFile = file;
                _firstMatchCursor = resultCursor;
            } else if ((file == _firstMatchFile) && (resultCursor == _firstMatchCursor)) { //check if we wrapped around
                if (backward == _lastMatchBackward) { //beep only when going in the same direction for two calls
                    _firstMatchFile = nullptr; //reset search
                    QApplication::beep();
                    return nullptr;
                }
            }
            _lastMatchBackward = backward;
            file->setTextCursor(resultCursor);
            return file;
        }
        firstFile = false;
    }

    //nothing found
    QApplication::beep();
    return nullptr;
}

QList<FileItem*> Find::fileList(FileItem* pivotFile, bool backward) {
    QList<FileItem*> items;

    bool foundPivot;
    int insertLocation = 0;
    for (int i = 0; i < _storage->folderCount(); i++) {
        FolderItem* folder = _storage->getFolder(i);
        for (int j = 0; j < folder->fileCount(); j++) {
            FileItem* file = folder->getFile(j);
            if (backward) { //if we're ordering them backward
                if (foundPivot) {
                    items.insert(insertLocation, file);
                } else {
                    items.insert(0, file);
                    insertLocation++;
                }
                if (file == pivotFile) { foundPivot = true; }
            } else {
                if (file == pivotFile) { foundPivot = true; }
                if (foundPivot) { //insert to the front of the list
                    items.insert(insertLocation, file);
                    insertLocation++;
                } else {
                    items.append(file);
                }
            }
        }
    }

    if (foundPivot) { //add pivot file again at the end to allow checking before the first cursor
        items.append(items[0]);
    }

    return items;
}


bool Find::hasText() {
    return _findText.length() > 0;
}

QString Find::lastText() {
    return _findText;
}

bool Find::lastMatchCase() {
    return _findFlags.testFlag(QTextDocument::FindCaseSensitively);
}

bool Find::lastWholeWord() {
    return _findFlags.testFlag(QTextDocument::FindWholeWords);
}
