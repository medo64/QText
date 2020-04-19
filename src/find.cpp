#include <QApplication>
#include <QTextDocument>
#include <QFlags>

#include "find.h"


Storage* Find::_storage = nullptr;
QString Find::_findText;
QFlags<QTextDocument::FindFlag> Find::_findFlags;
FileItem* Find::_firstMatchFile = nullptr;
QTextCursor Find::_firstMatchCursor;


void Find::setup(Storage* storage, QString text, bool matchCase) {
    _storage = storage;

    _findText = text;
    _findFlags = QTextDocument::FindFlag();
    if (matchCase) { _findFlags |= QTextDocument::FindCaseSensitively; }

    _firstMatchFile = nullptr;
    _firstMatchCursor = QTextCursor();
}

FileItem* Find::findNext(FileItem* currentFile) {
    bool firstFile = true;
    for(auto file : fileList(currentFile)) {
        QTextDocument* document = file->document();
        QTextCursor cursor = firstFile ? file->textCursor() : QTextCursor(); //only starting search starts from current cursor
        QTextCursor resultCursor = document->find(_findText, cursor, _findFlags);
        if (!resultCursor.isNull()) {
            if (_firstMatchFile == nullptr) { //save first match
                _firstMatchFile = file;
                _firstMatchCursor = resultCursor;
            } else if ((file == _firstMatchFile) && (resultCursor == _firstMatchCursor)) { //check if we wrapped around
                _firstMatchFile = nullptr; //reset search
                QApplication::beep();
                return nullptr;
            }
            file->setTextCursor(resultCursor);
            return file;
        }
        firstFile = false;
    }

    //nothing found
    QApplication::beep();
    return nullptr;
}

QList<FileItem*> Find::fileList(FileItem* pivotFile) {
    QList<FileItem*> items;

    bool foundPivot;
    int insertLocation = 0;
    for (int i = 0; i < _storage->folderCount(); i++) {
        FolderItem* folder = _storage->getFolder(i);
        for (int j = 0; j < folder->fileCount(); j++) {
            FileItem* file = folder->getFile(j);
            if (file == pivotFile) { foundPivot = true; }
            if (foundPivot) { //insert to the front of the list
                items.insert(insertLocation, file);
                insertLocation++;
            } else {
                items.append(file);
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
