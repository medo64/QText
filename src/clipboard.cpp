#include <QClipboard>
#include <QMimeData>
#include <QTextDocumentFragment>
#include "clipboard.h"

QClipboard* Clipboard::_clipboard = QApplication::clipboard();

bool Clipboard::hasPlain() {
    auto mimeData = _clipboard->mimeData();
    return mimeData->hasText();
}

bool Clipboard::cutPlain(QTextCursor cursor) {
    if (copyPlain(cursor)) {
        cursor.removeSelectedText();
        return true;
    }
    return false;
}

bool Clipboard::copyPlain(QTextCursor cursor) {
    if (cursor.hasSelection()) {
        _clipboard->clear();
        _clipboard->setText(cursor.selection().toPlainText());
        return true;
    }
    return false;
}

bool Clipboard::pastePlain(QTextCursor cursor) {
    auto data = _clipboard->mimeData();
    if (data->hasText()) {
        if (cursor.hasSelection()) { cursor.removeSelectedText(); }
        cursor.insertText(data->text());
        return true;
    }
    return false;
}
