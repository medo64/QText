#ifndef ICONS_H
#define ICONS_H

#include <QIcon>

class Icons {

    public:
        static QIcon application();
        static QIcon tray();

        static QIcon newFile();
        static QIcon saveFile();
        static QIcon renameFile();
        static QIcon deleteFile();

        static QIcon cut();
        static QIcon copy();
        static QIcon paste();

        static QIcon undo();
        static QIcon redo();

        static QIcon gotoDialog();

        static QIcon appMenu();

};

#endif // ICONS_H
