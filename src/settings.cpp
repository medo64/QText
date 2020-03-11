#include <algorithm>
#include "settings.h"
#include "medo/config.h"

#include <QDir>

bool Settings::alwaysOnTop() {
    return Config::read("AlwaysOnTop", defaultAlwaysOnTop());
}

bool Settings::defaultAlwaysOnTop() {
    return false;
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

int Settings::defaultClearUndoInterval() {
    return 15 * 60; //15 minutes
}

void Settings::setClearUndoInterval(int newClearUndoInterval) {
    Config::write("ClearUndoInterval", newClearUndoInterval);
}


QStringList Settings::dataPaths() {
    QStringList paths = Config::readMany("DataPath", defaultDataPaths());
    QStringList cleanedPaths;
    for (QString path : paths) {
        if (path.length() > 0) { cleanedPaths.append(QDir::cleanPath(path)); }
    }
    return (cleanedPaths.length() > 0) ? cleanedPaths : defaultDataPaths();
}

QStringList Settings::defaultDataPaths() {
    return QStringList(Config::dataDirectory());
}

void Settings::setDataPaths(QStringList newPaths) {
    QStringList cleanedPaths;
    for (QString newPath : newPaths) {
        if (newPath.length() > 0) { cleanedPaths.append(QDir::cleanPath(newPath)); }
    }
    Config::writeMany("DataPath", cleanedPaths);
}


QKeySequence Settings::hotkey() {
    QString hotkeyText = Config::read("Hotkey", "");
    if (hotkeyText.length() > 0) {
        QKeySequence hotkeyKeys = QKeySequence(hotkeyText, QKeySequence::PortableText);
        if (hotkeyKeys.count() > 0) { return QKeySequence { hotkeyKeys[0] }; } //return first key found
    }
    return  defaultHotkey();
}

QKeySequence Settings::defaultHotkey() {
    QKeySequence defaultHotkey { "Ctrl+Shift+Q" };
    return  defaultHotkey;
}

void Settings::setHotkey(QKeySequence newHotkey) {
    QString hotkeyText { newHotkey.toString(QKeySequence::PortableText) };
    Config::write("DataPath", hotkeyText);
}


bool Settings::minimizeToTray() {
    return Config::read("MinimizeToTray", defaultMinimizeToTray());
}

bool Settings::defaultMinimizeToTray() {
    return true;
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

int Settings::defaultQuickSaveInterval() {
    return 2500;
}

void Settings::setQuickSaveInterval(int newQuickSaveInterval) {
    Config::write("QuickSaveInterval", newQuickSaveInterval);
}


double Settings::scaleFactor() {
    double value = Config::read("ScaleFactor", defaultScaleFactor());
    return (value == 0) ? 0.00 : std::min(std::max(value, 0.25), 4.00);
}

double Settings::defaultScaleFactor() {
    return 0.00;
}

void Settings::setScaleFactor(double newScaleFactor) {
    double value = (newScaleFactor == 0) ? 0.00 : std::min(std::max(newScaleFactor, 0.25), 4.00);
    Config::write("ScaleFactor", value);
}


bool Settings::showInTaskbar() {
    return Config::read("ShowInTaskbar", defaultShowInTaskbar());
}

bool Settings::defaultShowInTaskbar() {
    return true;
}

void Settings::setShowInTaskbar(bool newShowInTaskbar) {
    Config::write("ShowInTaskbar", newShowInTaskbar);
}


int Settings::tabWidth() {
    return std::min(std::max(Config::read("TabWidth", defaultTabWidth()), 2), 16);
}

int Settings::defaultTabWidth() {
    return 4;
}

void Settings::setTabWidth(int newTabWidth) {
    Config::write("TabWidth", std::min(std::max(newTabWidth, 2), 16));
}


QString Settings::timeFormat() {
    return Config::read("TimeFormat", defaultTimeFormat());
}

QString Settings::defaultTimeFormat() {
    return QString();
}

void Settings::setTimeFormat(QString newTimeFormat) {
    Config::write("TimeFormat", newTimeFormat);
}


bool Settings::wordWrap() {
    return Config::read("WordWrap", defaultWordWrap());
}

bool Settings::defaultWordWrap() {
    return true;
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
