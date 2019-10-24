#ifndef CLIPBOARD_H
#define CLIPBOARD_H

#include <QApplication>
#include <QClipboard>
#include <QTextCursor>

class Clipboard {

    public:
        static bool hasPlain();
        static bool setText(QString text);
        static bool cutPlain(QTextCursor cursor);
        static bool copyPlain(QTextCursor cursor);
        static bool pastePlain(QTextCursor cursor);

    private:
        static QClipboard* _clipboard;

};

#endif // CLIPBOARD_H
