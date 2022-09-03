#pragma once

#include <QApplication>
#include <QClipboard>
#include <QTextCursor>

class Clipboard {

    public:
        static bool hasPlain();
        static bool hasHtml();
        static bool setData(QString plainText, QString htmlText = "");
        static bool cutPlain(QTextCursor cursor);
        static bool copyPlain(QTextCursor cursor);
        static bool pastePlain(QTextCursor cursor);
        static bool cutHtml(QTextCursor cursor);
        static bool copyHtml(QTextCursor cursor);
        static bool pasteHtml(QTextCursor cursor);

};
