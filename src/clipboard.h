#pragma once

#include <QApplication>
#include <QClipboard>
#include <QTextCursor>

class Clipboard {

    public:
        static bool hasText();
        static bool setText(QString text);
        static bool cutText(QTextCursor cursor);
        static bool copyText(QTextCursor cursor);
        static bool pasteText(QTextCursor cursor);

};
