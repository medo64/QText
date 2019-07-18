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
        static bool minimizeToTray();
        static void setMinimizeToTray(bool newMinimizeToTray);

};

#endif // SETTINGS_H
