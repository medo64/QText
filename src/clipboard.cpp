#include <QClipboard>
#include <QMimeData>
#include <QTextDocumentFragment>
#include "clipboard.h"

QClipboard* Clipboard::_clipboard = QApplication::clipboard();

bool Clipboard::hasText() {
    auto mimeData = _clipboard->mimeData();
    return mimeData->hasText();
}

bool Clipboard::setText(QString text) {
    _clipboard->clear();
    if (!text.isEmpty()) {
        _clipboard->setText(text);
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
        _clipboard->clear();
        _clipboard->setText(cursor.selection().toPlainText());
        return true;
    }
    return false;
}

bool Clipboard::pasteText(QTextCursor cursor) {
    auto data = _clipboard->mimeData();
    if (data->hasText()) {
        if (cursor.hasSelection()) { cursor.removeSelectedText(); }
        cursor.insertText(data->text());
        return true;
    }
    return false;
}
