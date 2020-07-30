#pragma once

#include <QIcon>
#include <QPixmap>

class Icons {

    public:
        static QIcon app();
        static QIcon appMono();
        static QIcon appWhite();

        static QIcon newFile();
        static QIcon saveFile();
        static QIcon renameFile();
        static QIcon deleteFile();
        static QIcon printFile();
        static QIcon printPreviewFile();
        static QIcon printToPdfFile();

        static QIcon cut();
        static QIcon copy();
        static QIcon paste();

        static QIcon undo();
        static QIcon redo();

        static QIcon fontBold();
        static QIcon fontItalic();
        static QIcon fontUnderline();
        static QIcon fontStrikethrough();

        static QIcon find();
        static QIcon findNext();

        static QIcon gotoIcon();

        static QIcon settings();

        static QIcon gotoFile();
        static QIcon gotoFolder();

        static void setDarkMode(bool darkMode);

    private:
        static bool _darkMode;
        static QPixmap getPixmap(QString baseName, int size);

};
