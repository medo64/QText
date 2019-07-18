#include "settings.h"
#include "medo/config.h"

#include <QDir>

QString Settings::dataPath() {
    QString defaultPath = Config::dataDirectory();
    QString path = Config::read("DataPath", Config::read("FilesLocation", defaultPath));
    return (path.length() > 0) ? QDir::cleanPath(path) : defaultPath;
}

void Settings::setDataPath(QString newPath) {
    Config::write("DataPath", QDir::cleanPath(newPath));
}


QKeySequence Settings::hotkey() {
    QKeySequence defaultHotkey { "Ctrl+Shift+Q" };
    QString hotkeyText = Config::read("Hotkey", "");
    if (hotkeyText.length() > 0) {
        QKeySequence hotkeyKeys = QKeySequence(hotkeyText, QKeySequence::PortableText);
        if (hotkeyKeys.count() > 0) { return QKeySequence { hotkeyKeys[0] }; } //return first key found
    }
    return  defaultHotkey;
}

void Settings::setHotkey(QKeySequence newHotkey) {
    QString hotkeyText { newHotkey.toString(QKeySequence::PortableText) };
    Config::write("DataPath", hotkeyText);
}


bool Settings::minimizeToTray() {
    return Config::read("MinimizeToTray", false);
}

void Settings::setMinimizeToTray(bool newMinimizeToTray) {
    Config::write("MinimizeToTray", newMinimizeToTray);
}
