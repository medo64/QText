#include <QApplication>

#include "find.h"

QString Find::_text = QString();


void Find::setup(QString text) {
    _text = text;
}

bool Find::findNext(FileItem* selectedFile) {
    QTextDocument* document = selectedFile->document();

    QTextCursor cursor = document->find(_text, selectedFile->textCursor());
    if (!cursor.isNull()) {
        selectedFile->setTextCursor(cursor);
        return true;
    } else {
        QApplication::beep();
        return false;
    }
}

QString Find::lastText() {
    return _text;
}

bool Find::hasText() {
    return _text.length() > 0;
}
