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


QString Settings::dataPath() {
    QString path = Config::read("DataPath", Config::read("FilesLocation", defaultDataPath()));
    return (path.length() > 0) ? QDir::cleanPath(path) : defaultDataPath();
}

QString Settings::defaultDataPath() {
    return Config::dataDirectory();
}

void Settings::setDataPath(QString newPath) {
    Config::write("DataPath", QDir::cleanPath(newPath));
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
    return false;
}

void Settings::setMinimizeToTray(bool newMinimizeToTray) {
    Config::write("MinimizeToTray", newMinimizeToTray);
}


int Settings::quickSaveInterval() {
    int value = Config::read("quickSaveInterval", defaultQuickSaveInterval());
    if (value == 0) { return 0; } //quick save disabled
    if (value < 1000) { return 1000;  } //minimum is 1 seconds
    if (value > 60000) { return 60000;  } //maximum is 60 seconds
    return value;
}

int Settings::defaultQuickSaveInterval() {
    return 2500;
}

void Settings::setQuickSaveInterval(int newQuickSaveInterval) {
    Config::write("MinimizeToTray", newQuickSaveInterval);
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


// state settings


QString Settings::lastFile(QString folder) {
    if (folder.length() > 0) {
        return Config::read("LastFile!" + folder, "");
    } else {
        return Config::read("LastFile", "");
    }
}

void Settings::setLastFile(QString folder, QString file) {
    if (folder.length() > 0) {
        Config::write("LastFile!" + folder, file);
    } else {
        Config::write("LastFile", file);
    }
}


QString Settings::lastFolder() {
    return Config::read("LastFolder", "");
}

void Settings::setLastFolder(QString folder) {
    Config::write("LastFolder", folder);
}


bool Settings::setupCompleted() {
    return Config::read("SetupCompleted", false);
}

void Settings::setSetupCompleted(bool newSetupCompleted) {
    Config::write("SetupCompleted", newSetupCompleted);
}
