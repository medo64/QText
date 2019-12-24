#ifndef SETTINGS_H
#define SETTINGS_H

#include <QKeySequence>
#include <QString>

class Settings {

    public:

        static bool alwaysOnTop();
        static bool defaultAlwaysOnTop();
        static void setAlwaysOnTop(bool newAlwaysOnTop);

        static int clearUndoInterval();
        static int defaultClearUndoInterval();
        static void setClearUndoInterval(int newClearUndoInterval);

        static QStringList dataPaths();
        static QStringList defaultDataPaths();
        static void setDataPaths(QStringList newPath);

        static QString dataPath2();
        static QString defaultDataPath2();
        static void setDataPath2(QString newPath);

        static QKeySequence hotkey();
        static QKeySequence defaultHotkey();
        static void setHotkey(QKeySequence newHotkey);

        static bool minimizeToTray();
        static bool defaultMinimizeToTray();
        static void setMinimizeToTray(bool newMinimizeToTray);

        static int quickSaveInterval();
        static int defaultQuickSaveInterval();
        static void setQuickSaveInterval(int newQuickSaveInterval);

        static bool showInTaskbar();
        static bool defaultShowInTaskbar();
        static void setShowInTaskbar(bool newShowInTaskbar);

        static int tabWidth();
        static int defaultTabWidth();
        static void setTabWidth(int newTabWidth);

        static bool wordWrap();
        static bool defaultWordWrap();
        static void setWordWrap(bool newWordWrap);


        static QString lastFile(QString folder);
        static void setLastFile(QString folder, QString file);

        static QString lastFolder();
        static void setLastFolder(QString folder);

        static bool setupCompleted();
        static void setSetupCompleted(bool newSetupCompleted);

};

#endif // SETTINGS_H
