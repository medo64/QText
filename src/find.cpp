#include <QApplication>
#include <QTextDocument>

#include "find.h"


Storage* Find::_storage = nullptr;
QString Find::_findText;
QFlags<QTextDocument::FindFlag> Find::_findFlags;
bool Find::_findUseRegEx;
FileItem* Find::_firstMatchFile = nullptr;
QTextCursor Find::_firstMatchCursor;
bool Find::_firstMatchBackward;
Find::SearchScope Find::_findScope = Find::SearchScope::AllFolders;


void Find::setup(Storage* storage, QString text, bool matchCase, bool wholeWord, bool useRegEx, Find::SearchScope searchScope) {
    _storage = storage;

    _findText = text;
    _findFlags = QTextDocument::FindFlag();
    if (matchCase) { _findFlags |= QTextDocument::FindCaseSensitively; }
    if (wholeWord) { _findFlags |= QTextDocument::FindWholeWords; }
    _findUseRegEx = useRegEx;

    _firstMatchFile = nullptr;
    _firstMatchCursor = QTextCursor();
    _firstMatchBackward = false;

    _findScope = searchScope;
}

FileItem* Find::findNext(FileItem* currentFile, bool backward) {
    bool firstFile = true;
    for (auto file : fileList(currentFile, backward)) {
        auto cursor = file->textCursor();
        if (!firstFile) { cursor.movePosition(backward ? QTextCursor::End : QTextCursor::Start); }
        auto flags = backward ? _findFlags | QTextDocument::FindBackward : _findFlags;

        auto document = file->document();
        QTextCursor resultCursor;
        if (_findUseRegEx) {
            auto regEx = QRegExp(_findText, _findFlags.testFlag(QTextDocument::FindCaseSensitively) ? Qt::CaseSensitive : Qt::CaseInsensitive);
            resultCursor = document->find(regEx, cursor, flags);
        } else {
            resultCursor = document->find(_findText, cursor, flags);
        }

        if (!resultCursor.isNull()) {
            if ((_firstMatchFile == nullptr) || (_firstMatchBackward != backward)) { //save first match
                _firstMatchFile = file;
                _firstMatchCursor = resultCursor;
                _firstMatchBackward = backward;
            } else if ((file == _firstMatchFile) && (resultCursor == _firstMatchCursor)) { //check if we wrapped around
                QApplication::beep();
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

QList<FileItem*> Find::fileList(FileItem* pivotFile, bool backward) {
    QList<FileItem*> items;

    if (_findScope == Find::SearchScope::CurrentFile) {
        items.append(pivotFile);
        items.append(pivotFile);
    } else {
        bool foundPivot = false;
        int insertLocation = 0;
        for (FolderItem* folder : *_storage) {
            if ((_findScope == Find::SearchScope::CurrentFolder) && (folder != pivotFile->folder())) { continue; }

            for (FileItem* file : *folder) {
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

bool Find::lastUseRegEx() {
    return _findUseRegEx;
}

Find::SearchScope Find::lastSearchScope() {
    return _findScope;
}
