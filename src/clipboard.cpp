#include <QClipboard>
#include <QThread>
#include <QMimeData>
#include <QTextDocumentFragment>
#include "clipboard.h"

bool Clipboard::hasPlain() {
    auto mimeData = QApplication::clipboard()->mimeData();
    return mimeData->hasText();
}

bool Clipboard::hasHtml() {
    auto mimeData = QApplication::clipboard()->mimeData();
    return mimeData->hasHtml();
}

bool Clipboard::setData(QString plainText, QString htmlText) {
    if (plainText.isEmpty()) { return false; } //plain text has to be present

    QClipboard* clipboard = QApplication::clipboard();

    QMimeData* data = new QMimeData;
    data->setText(plainText);
    if (!htmlText.isEmpty()) { data->setHtml(htmlText); }
    clipboard->setMimeData(data, QClipboard::Clipboard);

    if (clipboard->supportsSelection()) { //to support Linux Terminal app, only plain text
        clipboard->setText(plainText, QClipboard::Selection);
    }

#if defined(Q_OS_LINUX)
    QThread::msleep(1); //workaround for copied text not being available...
#endif

    return true;
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
        return setData(cursor.selection().toPlainText());
    }
    return false;
}

bool Clipboard::pastePlain(QTextCursor cursor) {
    auto data = QApplication::clipboard()->mimeData();
    if (data->hasText()) {
        if (cursor.hasSelection()) { cursor.removeSelectedText(); }
        cursor.insertText(data->text());
        return true;
    }
    return false;
}


bool Clipboard::cutHtml(QTextCursor cursor) {
    if (copyHtml(cursor)) {
        cursor.removeSelectedText();
        return true;
    }
    return false;
}

bool Clipboard::copyHtml(QTextCursor cursor) {
    if (cursor.hasSelection()) {
        return setData(cursor.selection().toPlainText(), cursor.selection().toHtml());
    }
    return false;
}

bool Clipboard::pasteHtml(QTextCursor cursor) {
    auto data = QApplication::clipboard()->mimeData();
    if (data->hasText()) {
        if (cursor.hasSelection()) { cursor.removeSelectedText(); }
        if (data->hasHtml()) {
            cursor.insertHtml(data->html());
        } else {
            cursor.insertText(data->text());
        }
        return true;
    }
    return false;
}
