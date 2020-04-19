#include <QApplication>

#include "find.h"


QString Find::_text = QString();
Storage* Find::_storage = nullptr;


void Find::setup(QString text, Storage* storage, FileItem* firstFile) {
    _text = text;
    _storage = storage;
}

FileItem* Find::findNext(FileItem* currentFile) {
    bool firstFile = true;
    for(auto file : fileList(currentFile)) {
        QTextDocument* document = file->document();
        QTextCursor cursor = firstFile ? file->textCursor() : QTextCursor(); //only starting search starts from current cursor
        QTextCursor resultCursor = document->find(_text, cursor);
        if (!resultCursor.isNull()) {
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


QString Find::lastText() {
    return _text;
}

bool Find::hasText() {
    return _text.length() > 0;
}
