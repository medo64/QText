#include <QClipboard>
#include <QThread>
#include <QMimeData>
#include <QTextDocumentFragment>
#include "clipboard.h"

bool Clipboard::hasText() {
    auto mimeData = QApplication::clipboard()->mimeData();
    return mimeData->hasText();
}

bool Clipboard::setText(QString text) {
    if (text.isEmpty()) { return false; }

    QClipboard* clipboard = QApplication::clipboard();

    QMimeData* data = new QMimeData;
    data->setText(text);
    clipboard->setMimeData(data, QClipboard::Clipboard);

    if (clipboard->supportsSelection()) { //to support Linux Terminal app, only plain text
        clipboard->setText(text, QClipboard::Selection);
    }

#if defined(Q_OS_LINUX)
    QThread::msleep(1); //workaround for copied text not being available...
#endif

    return true;
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
