#include <algorithm>
#include "settings.h"
#include "medo/config.h"

#include <QDir>

bool Settings::alwaysOnTop() {
    return Config::read("AlwaysOnTop", defaultAlwaysOnTop());
}

void Settings::setAlwaysOnTop(bool newAlwaysOnTop) {
    Config::write("AlwaysOnTop", newAlwaysOnTop);
}


int Settings::clearUndoInterval() {
    int value = Config::read("ClearUndoInterval", defaultClearUndoInterval());
    if (value == 0) { return 0; } //disabled
    if (value < 60) { return 60;  } //minimum is 1 minute
    if (value > 24 * 60 * 60) { return 24 * 60 * 60;  } //maximum is 1 day
    return value;
}

void Settings::setClearUndoInterval(int newClearUndoInterval) {
    Config::write("ClearUndoInterval", newClearUndoInterval);
}


bool Settings::colorTrayIcon() {
    return Config::read("ColorTrayIcon", defaultColorTrayIcon());
}

void Settings::setColorTrayIcon(bool newColorTrayIcon) {
    Config::write("ColorTrayIcon", newColorTrayIcon);
}


QString Settings::dataPath() { //return only the first directory
    auto paths = dataPaths();
    return paths[0];
}

void Settings::setDataPath(QString newPath) { //set only the first directory, leaving other entries unchanged
    auto paths = dataPaths();
    paths[0] = newPath;
    setDataPaths(paths);
}

QStringList Settings::dataPaths() {
    QStringList paths = Config::readMany("DataPath", defaultDataPaths());
    QStringList cleanedPaths;
    for (QString path : paths) {
        if (path.length() > 0) { cleanedPaths.append(QDir::cleanPath(path)); }
    }
    return (cleanedPaths.length() > 0) ? cleanedPaths : defaultDataPaths();
}

void Settings::setDataPaths(QStringList newPaths) {
    QStringList cleanedPaths;
    for (QString newPath : newPaths) {
        if (newPath.length() > 0) { cleanedPaths.append(QDir::cleanPath(newPath)); }
    }
    Config::writeMany("DataPath", cleanedPaths);
}



FileType Settings::defaultFileType() {
    QString value = Config::read("DefaultFileType", "Plain");
    if ((value.compare("Html", Qt::CaseInsensitive) == 0) || (value.contains("html", Qt::CaseInsensitive))) {
        return FileType::Html;
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
    } else if ((value.compare("Markdown", Qt::CaseInsensitive) == 0) || (value.contains("md", Qt::CaseInsensitive))) {
        return FileType::Markdown;
#endif
    } else {
        return FileType::Plain;
    }
}

void Settings::setDefaultFileType(FileType newDefaultFileType) {
    switch (newDefaultFileType) {
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
        case FileType::Markdown: Config::write("DefaultFileType", "Markdown"); break;
#endif
        case FileType::Html:     Config::write("DefaultFileType", "Html");     break;
        default:                 Config::write("DefaultFileType", "Plain");    break;
    }
}


DeletionStyle Settings::deletionSyle() {
    QString value = Config::read("DeletionStyle", "Delete");
    if (value.compare("Recycle", Qt::CaseInsensitive) == 0) {
        return DeletionStyle::Recycle;
    } else if (value.compare("Overwrite", Qt::CaseInsensitive) == 0) {
        return DeletionStyle::Overwrite;
    } else {
        return DeletionStyle::Delete;
    }
}

void Settings::setDeletionStyle(DeletionStyle newDeletionStyle) {
    switch (newDeletionStyle) {
        case DeletionStyle::Recycle:   Config::write("DeletionStyle", "Recycle"); break;
        case DeletionStyle::Overwrite: Config::write("DeletionStyle", "Overwrite"); break;
        default:                       Config::write("DeletionStyle", "Delete");    break;
    }
}


bool Settings::forceDarkMode() {
    return Config::read("ForceDarkMode", defaultForceDarkMode());
}

void Settings::setForceDarkMode(bool newForceDarkMode) {
    Config::write("ForceDarkMode", newForceDarkMode);
}


bool Settings::forcePlainCopyPaste() {
    return Config::read("ForcePlainCopyPaste", defaultForcePlainCopyPaste());
}

void Settings::setForcePlainCopyPaste(bool newForcePlainCopyPaste) {
    Config::write("ForcePlainCopyPaste", newForcePlainCopyPaste);
}


QKeySequence Settings::hotkey() {
    QString hotkeyText = Config::read("Hotkey", "");
    if (hotkeyText.length() > 0) {
        QKeySequence hotkeyKeys = QKeySequence(hotkeyText, QKeySequence::PortableText);
        if (hotkeyKeys.count() > 0) { return QKeySequence { hotkeyKeys[0] }; } //return first key found
    }
    return  defaultHotkey();
}

void Settings::setHotkey(QKeySequence newHotkey) {
    QString hotkeyText { newHotkey.toString(QKeySequence::PortableText) };
    Config::write("Hotkey", hotkeyText);
}


bool Settings::minimizeToTray() {
    return Config::read("MinimizeToTray", defaultMinimizeToTray());
}

void Settings::setMinimizeToTray(bool newMinimizeToTray) {
    Config::write("MinimizeToTray", newMinimizeToTray);
}


int Settings::quickSaveInterval() {
    int value = Config::read("QuickSaveInterval", defaultQuickSaveInterval());
    if (value == 0) { return 0; } //quick save disabled
    if (value < 1000) { return 1000;  } //minimum is 1 seconds
    if (value > 60000) { return 60000;  } //maximum is 60 seconds
    return value;
}

void Settings::setQuickSaveInterval(int newQuickSaveInterval) {
    Config::write("QuickSaveInterval", newQuickSaveInterval);
}


bool Settings::showInTaskbar() {
    return Config::read("ShowInTaskbar", defaultShowInTaskbar());
}

void Settings::setShowInTaskbar(bool newShowInTaskbar) {
    Config::write("ShowInTaskbar", newShowInTaskbar);
}


#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)

bool Settings::showMarkdown() {
    return Config::read("ShowMarkdown", defaultShowMarkdown());
}

void Settings::setShowMarkdown(bool newShowMarkdown) {
    Config::write("ShowMarkdown", newShowMarkdown);
}

#endif

bool Settings::tabTextColorPerType() {
    return Config::read("TabTextColorPerType", defaultTabTextColorPerType());
}

void Settings::setTabTextColorPerType(bool newTabTextColorPerType) {
    Config::write("TabTextColorPerType", newTabTextColorPerType);
}


int Settings::tabWidth() {
    return std::min(std::max(Config::read("TabWidth", defaultTabWidth()), 2), 16);
}

void Settings::setTabWidth(int newTabWidth) {
    Config::write("TabWidth", std::min(std::max(newTabWidth, 2), 16));
}


QString Settings::timeFormat() {
    return Config::read("TimeFormat", defaultTimeFormat());
}

void Settings::setTimeFormat(QString newTimeFormat) {
    Config::write("TimeFormat", newTimeFormat);
}


bool Settings::wordWrap() {
    return Config::read("WordWrap", defaultWordWrap());
}

void Settings::setWordWrap(bool newWordWrap) {
    Config::write("WordWrap", newWordWrap);
}


// state settings


QString Settings::lastFile(QString folder) {
    if (folder.length() > 0) {
        return Config::stateRead("LastFile!" + folder, "");
    } else {
        return Config::stateRead("LastFile", "");
    }
}

void Settings::setLastFile(QString folder, QString file) {
    if (folder.length() > 0) {
        Config::stateWrite("LastFile!" + folder, file);
    } else {
        Config::stateWrite("LastFile", file);
    }
}


QString Settings::lastFolder() {
    return Config::stateRead("LastFolder", "");
}

void Settings::setLastFolder(QString folder) {
    Config::stateWrite("LastFolder", folder);
}


bool Settings::setupCompleted() {
    return Config::stateRead("SetupCompleted", false);
}

void Settings::setSetupCompleted(bool newSetupCompleted) {
    Config::stateWrite("SetupCompleted", newSetupCompleted);
}
