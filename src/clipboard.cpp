#include <QClipboard>
#include <QMimeData>
#include <QTextDocumentFragment>
#include "clipboard.h"

bool Clipboard::hasText() {
    auto mimeData = QApplication::clipboard()->mimeData();
    return mimeData->hasText();
}

bool Clipboard::setText(QString text) {
    QApplication::clipboard()->clear();
    if (!text.isEmpty()) {
        if (QApplication::clipboard()->supportsSelection()) {
            QApplication::clipboard()->setText(text, QClipboard::Selection); //to support Linux Terminal app
        }
        QApplication::clipboard()->setText(text);
        return true;
    }
    return false;
}

bool Clipboard::cutText(QTextCursor cursor) {
    if (copyText(cursor)) {
        cursor.removeSelectedText();
        return true;
    }
    return false;
}

bool Clipboard::copyText(QTextCursor cursor) {
    if (cursor.hasSelection()) {
        return setText(cursor.selection().toPlainText());
    }
    return false;
}

bool Clipboard::pasteText(QTextCursor cursor) {
    auto data = QApplication::clipboard()->mimeData();
    if (data->hasText()) {
        if (cursor.hasSelection()) { cursor.removeSelectedText(); }
        cursor.insertText(data->text());
        return true;
    }
    return false;
}
