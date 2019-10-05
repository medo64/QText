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
        static QString lastFile(QString folder);
        static void setLastFile(QString folder, QString file);
        static QString lastFolder();
        static void setLastFolder(QString folder);
        static bool minimizeToTray();
        static void setMinimizeToTray(bool newMinimizeToTray);
        static int quickSaveInterval();
        static void setQuickSaveInterval(int newQuickSaveInterval);
        static bool setupCompleted();
        static void setSetupCompleted(bool newSetupCompleted);
        static bool showInTaskbar();
        static void setShowInTaskbar(bool newShowInTaskbar);

};

#endif // SETTINGS_H
