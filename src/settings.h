#ifndef SETTINGS_H
#define SETTINGS_H

#include <QKeySequence>
#include <QString>

class Settings {

    public:
        static QString dataPath();
        static void setDataPath(QString newPath);
        static QKeySequence hotkey();
        static void setHotkey(QKeySequence newHotkey);

};

#endif // SETTINGS_H
