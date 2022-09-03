#include "dconfHotkey.h"
#include <QCoreApplication>
#include <QDebug>
#include <QProcess>

DConfHotkey::DConfHotkey(QString name, QObject* parent)
    : QObject(parent) {
#if defined(Q_OS_WIN)  // not applicable for Windows
    return;
#endif

    _name = name;
}


QString getKeyString(QKeySequence key) {
    auto keyboardKey = Qt::Key(key[0] & static_cast<int>(~Qt::KeyboardModifierMask));
    auto keyboardModifiers = Qt::KeyboardModifiers(key[0] & static_cast<int>(Qt::KeyboardModifierMask));

    QString keySequence;
    if (keyboardModifiers & Qt::ShiftModifier)   { keySequence += "<Shift>";   keyboardModifiers ^= Qt::ShiftModifier; }
    if (keyboardModifiers & Qt::ControlModifier) { keySequence += "<Primary>"; keyboardModifiers ^= Qt::ControlModifier; }
    if (keyboardModifiers & Qt::AltModifier)     { keySequence += "<Alt>";     keyboardModifiers ^= Qt::AltModifier; }
    if (keyboardModifiers & Qt::MetaModifier)    { keySequence += "<Super>";   keyboardModifiers ^= Qt::MetaModifier; }

    if (keyboardModifiers == 0) { //all modifiers have been processed
        QKeySequence keySeq = QKeySequence(keyboardKey);
        QString keySeqText = keySeq.toString(QKeySequence::NativeText);
        if (keySeqText.length() == 1) {
            keySequence += keySeqText.toLower();
        } else {
            keySequence += keySeqText;
        }
        return keySequence;
    } else {
        return QString();  // cannot figure how to process all modifiers
    }
}

QStringList getSubkeys() {
    QProcess dconfList;
    dconfList.start("/usr/bin/dconf",
                    QStringList({
                        "list",
                        "/org/gnome/settings-daemon/plugins/media-keys/custom-keybindings/"
                    }));
    dconfList.waitForFinished();
    QString dconfListText(dconfList.readAllStandardOutput());

    QStringList dconfListLines = dconfListText.split('\n', Qt::SkipEmptyParts);
    return dconfListLines;
}

QString getSubkey(QString name, QStringList subkeys) {  // returns which subkey has data (e.g. "custom0/")
    for (const QString& pathSubkey : qAsConst(subkeys)) {
        if (!pathSubkey.isEmpty()) {
            QProcess dconfReadName;
            dconfReadName.start("/usr/bin/dconf",
                                QStringList({
                                     "read",
                                     "/org/gnome/settings-daemon/plugins/media-keys/custom-keybindings/" + pathSubkey + "name"
                                }));
            dconfReadName.waitForFinished();
            QString dconfReadNameText(dconfReadName.readAllStandardOutput());
            dconfReadNameText = dconfReadNameText.trimmed();
            if (dconfReadNameText.compare("'" + name + "'") == 0) {
                return pathSubkey;
            }
        }
    }
    return QString();  // not found
}


bool DConfHotkey::hasRegisteredHotkey() {
    QStringList dconfListLines = getSubkeys();
    QString selectedSubkey = getSubkey(_name, dconfListLines);
    return !selectedSubkey.isEmpty();
}

bool DConfHotkey::registerHotkey(QKeySequence key) {
#if defined(Q_OS_WIN)  // not applicable for Windows
    return false;
#endif

    if (key.count() != 1) {
        qDebug().noquote() << "[DConfHotkey]" << "Must have only one key combination!";
        return false;
    }

    QString keySequence = getKeyString(key);
    if (keySequence.isEmpty()) {
        qDebug().noquote() << "[DConfHotkey]" << "Cannot parse key sequence!";
        return false;
    }

    QStringList subkeys = getSubkeys();
    QString selectedSubkey = getSubkey(_name, subkeys );

    if (selectedSubkey.isEmpty()) {  // find a free spot
        int maxCustom = -1;
        for (const QString& pathSubkey : qAsConst(subkeys )) {
            if (pathSubkey.startsWith("custom") && pathSubkey.endsWith("/")) {
                bool nOk;
                int customNumber = pathSubkey.midRef(6, pathSubkey.length() - 7).toInt(&nOk);
                if (nOk && customNumber > maxCustom) {
                    maxCustom = customNumber;
                }
            }
        }
        selectedSubkey = "custom" + QString::number(maxCustom +1) + "/";
    }

    bool isOk = true;
    QString dconfPath = "/org/gnome/settings-daemon/plugins/media-keys/custom-keybindings/" + selectedSubkey;

    QProcess dconfWriteName;
    dconfWriteName.start("/usr/bin/dconf write " + dconfPath + "name \"'" + _name + "'\"", QStringList());
    dconfWriteName.waitForFinished();
    isOk = isOk && (dconfWriteName.exitCode() == 0);

    QProcess dconfWriteCommand;
    dconfWriteCommand.start("/usr/bin/dconf write " + dconfPath + "command \"'" + QCoreApplication::applicationFilePath() + "'\"", QStringList());
    dconfWriteCommand.waitForFinished();
    isOk = isOk && (dconfWriteCommand.exitCode() == 0);

    QProcess dconfWriteBinding;
    dconfWriteBinding.start("/usr/bin/dconf write " + dconfPath + "binding \"'" + keySequence + "'\"", QStringList());
    dconfWriteBinding.waitForFinished();
    isOk = isOk && (dconfWriteBinding.exitCode() == 0);

    if (!isOk){
        qDebug().noquote() << "[DConfHotkey]" << "Registration failed!";
    }
    return isOk;
}

bool DConfHotkey::unregisterHotkey() {
#if defined(Q_OS_WIN)  // not applicable for Windows
    return false;
#endif

    QStringList subkeys = getSubkeys();
    QString selectedSubkey = getSubkey(_name, subkeys);
    if (selectedSubkey.isEmpty()) {
        qDebug().noquote() << "[DConfHotkey]" << "No registration found!";
        return false;
    }

    QProcess dconfReset;
    QString dconfPath = "/org/gnome/settings-daemon/plugins/media-keys/custom-keybindings/" + selectedSubkey;
    dconfReset.start("/usr/bin/dconf reset -f " + dconfPath, QStringList());
    dconfReset.waitForFinished();
    bool isOk = (dconfReset.exitCode() == 0);

    if (!isOk){
        qDebug().noquote() << "[DConfHotkey]" << "Unregistration failed!";
    }
    return isOk;
}

