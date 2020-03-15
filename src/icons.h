#ifndef ICONS_H
#define ICONS_H

#include <QIcon>

class Icons {

    public:
        static QIcon app();
        static QIcon appMono();
        static QIcon tray();

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

        static QIcon gotoIcon();

        static QIcon settings();

};

#endif // ICONS_H
