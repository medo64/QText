#pragma once

#include <QKeySequence>
#include <QString>
#include "storage/filetype.h"

class Settings {

    public:
        typedef enum {
            Delete    = 1,
            Overwrite = 2,
        } DeletionStyle;

    public:

        static bool alwaysOnTop();
        static bool defaultAlwaysOnTop();
        static void setAlwaysOnTop(bool newAlwaysOnTop);

        static int clearUndoInterval();
        static int defaultClearUndoInterval();
        static void setClearUndoInterval(int newClearUndoInterval);

        static bool colorTrayIcon();
        static bool defaultColorTrayIcon();
        static void setColorTrayIcon(bool newColorTrayIcon);

        static QStringList dataPaths();
        static QStringList defaultDataPaths();
        static void setDataPaths(QStringList newPath);

        static QString dataPath2();
        static QString defaultDataPath2();
        static void setDataPath2(QString newPath);

        static FileType defaultFileType();
        static FileType defaultDefaultFileType();
        static void setDefaultFileType(FileType newDefaultFileType);

        static DeletionStyle deletionSyle();
        static DeletionStyle defaultDeletionStyle();
        static void setDeletionStyle(DeletionStyle newDeletionStyle);

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

        static bool tabTextColorPerType();
        static bool defaultTabTextColorPerType();
        static void setTabTextColorPerType(bool newTabTextColorPerType);

        static int tabWidth();
        static int defaultTabWidth();
        static void setTabWidth(int newTabWidth);

        static QString timeFormat();
        static QString defaultTimeFormat();
        static void setTimeFormat(QString newTimeFormat);

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
