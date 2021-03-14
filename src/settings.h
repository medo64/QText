#pragma once

#include <QKeySequence>
#include <QString>
#include "medo/config.h"
#include "storage/deletionstyle.h"
#include "storage/filetype.h"

class Settings {

    public:

        static bool alwaysOnTop();
        static void setAlwaysOnTop(bool newAlwaysOnTop);
        static bool defaultAlwaysOnTop() { return false; }

        static int clearUndoInterval();
        static void setClearUndoInterval(int newClearUndoInterval);
        static int defaultClearUndoInterval() { return 15 * 60; } //15 minutes

        static bool colorTrayIcon();
        static void setColorTrayIcon(bool newColorTrayIcon);
        static bool defaultColorTrayIcon() {
            if ((QSysInfo::kernelType() == "winnt") && (QSysInfo::productVersion() == "10")) {
                return false; //white icon
            } else {
                return true; //color icon
            }
        }

        static QString dataPath();
        static void setDataPath(QString newPath);
        static QString defaultDataPath() { return defaultDataPaths()[0]; }
        static QStringList dataPaths();
        static void setDataPaths(QStringList newPaths);
        static QStringList defaultDataPaths() { return QStringList(Config::dataDirectory()); }

        static FileType defaultFileType();
        static void setDefaultFileType(FileType newDefaultFileType);
        static FileType defaultDefaultFileType() { return FileType::Plain; }

        static DeletionStyle deletionSyle();
        static void setDeletionStyle(DeletionStyle newDeletionStyle);
        static DeletionStyle defaultDeletionStyle() { return DeletionStyle::Delete; }

        static bool followUrls();
        static void setFollowUrls(bool newFollowUrls);
        static bool defaultFollowUrls() { return true; }

        static QFont font();
        static void setFont(QFont newFont);

        static QString fontName();
        static void setFontName(QString newFontName);
        static QString defaultFontName() {
#if defined(Q_OS_WIN)
            return "Calibri";
#else
            return "DejaVu Sans";
#endif
        }

        static int fontSize();
        static void setFontSize(int newFontSize);
        static int defaultFontSize() { return 11; }

        static bool forceDarkMode();
        static void setForceDarkMode(bool newForceDarkMode);
        static bool defaultForceDarkMode() { return false; }

        static bool forcePlainCopyPaste();
        static void setForcePlainCopyPaste(bool newForcePlainCopyPaste);
        static bool defaultForcePlainCopyPaste() { return false; }

        static QKeySequence hotkey();
        static void setHotkey(QKeySequence newHotkey);
        static QKeySequence defaultHotkey() {
            QKeySequence defaultHotkey { "Ctrl+Shift+Q" };
            return  defaultHotkey;
        }

        static bool hotkeyTogglesVisibility();
        static void setHotkeyTogglesVisibility(bool newHotkeyTogglesVisibility);
        static bool defaultHotkeyTogglesVisibility() { return false; }

        static bool minimizeToTray();
        static void setMinimizeToTray(bool newMinimizeToTray);
        static bool defaultMinimizeToTray() { return true; }

        static int quickSaveInterval();
        static void setQuickSaveInterval(int newQuickSaveInterval);
        static int defaultQuickSaveInterval() { return 2500; }

        static bool showInTaskbar();
        static void setShowInTaskbar(bool newShowInTaskbar);
        static bool defaultShowInTaskbar() { return true; }

#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
        static bool showMarkdown();
        static void setShowMarkdown(bool newShowMarkdown);
        static bool defaultShowMarkdown() { return false; }
#endif

        static bool tabTextColorPerType();
        static void setTabTextColorPerType(bool newTabTextColorPerType);
        static bool defaultTabTextColorPerType() { return false; }

        static int tabWidth();
        static void setTabWidth(int newTabWidth);
        static int defaultTabWidth() { return 4; }

        static QString timeFormat();
        static void setTimeFormat(QString newTimeFormat);
        static QString defaultTimeFormat() { return QString(); }

        static bool wordWrap();
        static void setWordWrap(bool newWordWrap);
        static bool defaultWordWrap() { return true; }


    public:

        static QString lastFile(QString folder);
        static void setLastFile(QString folder, QString file);

        static QString lastFolder();
        static void setLastFolder(QString folder);

        static bool setupCompleted();
        static void setSetupCompleted(bool newSetupCompleted);

};
