#include "dconfHotkey.h"
#include <QCoreApplication>
#include <QDebug>
#include <QProcess>

DConfHotkey::DConfHotkey(QString name, QObject* parent)
    : QObject(parent) {
    _name = name;
}


QString getKeyString(QKeySequence key) {
    auto keyboardKey = Qt::Key(key[0] & static_cast<int>(~Qt::KeyboardModifierMask));
    auto keyboardModifiers = Qt::KeyboardModifiers(key[0] & static_cast<int>(Qt::KeyboardModifierMask));

    QString keySequence;
    if (keyboardModifiers & Qt::ShiftModifier)   { keySequence += "<Shift>";   keyboardModifiers ^= Qt::ShiftModifier; }
    if (keyboardModifiers & Qt::ControlModifier) { keySequence += "<Control>"; keyboardModifiers ^= Qt::ControlModifier; }
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

// clazy:skip(clazy-non-pod-global-static)
static const QString dconfRootPath = QStringLiteral("/org/gnome/settings-daemon/plugins/media-keys/custom-keybindings");

QStringList getSubkeys() {
    QProcess dconfList;
    dconfList.start("/usr/bin/dconf",
                    QStringList({
                        "list",
                        dconfRootPath + "/"
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
                                    dconfRootPath + "/" + pathSubkey + "name",
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

bool applyKeybindings() {  // otherwise custom keys are ignored
    QStringList subkeys = getSubkeys();
    QString newBindings;
    newBindings += "[";
    bool hadOne = false;
    for (const QString& subKey : qAsConst(subkeys)) {
        if (hadOne) { newBindings +=", "; } else { hadOne = true; }
        newBindings += "'" + dconfRootPath + "/" + subKey + "'";
    }
    newBindings += "]";

    bool isOk = true;

    QProcess dconfReadBindings;
    dconfReadBindings.start("/usr/bin/dconf",
                            QStringList {
                                "read",
                                dconfRootPath,
                            });
    dconfReadBindings.waitForFinished();
    QString currBindings;
    if (dconfReadBindings.exitCode() == 0) {
        currBindings = dconfReadBindings.readAllStandardOutput().trimmed();
    }

    if (newBindings.compare(currBindings) != 0) {  // update only if different
        if (hadOne) {
            QProcess dconfWriteBindings;
            dconfWriteBindings.start("/usr/bin/dconf",
                                     QStringList {
                                         "write",
                                         dconfRootPath,
                                         newBindings,
                                     });
            dconfWriteBindings.waitForFinished();
            isOk = isOk && (dconfWriteBindings.exitCode() == 0);
        } else {
            QProcess dconfResetBindings;
            dconfResetBindings.start("/usr/bin/dconf",
                                     QStringList {
                                         "reset",
                                         dconfRootPath,
                                     });
            dconfResetBindings.waitForFinished();
            isOk = isOk && (dconfResetBindings.exitCode() == 0);
        }
    }

    return isOk;
}


bool DConfHotkey::hasRegisteredHotkey() {
    QStringList dconfListLines = getSubkeys();
    QString selectedSubkey = getSubkey(_name, dconfListLines);
    return !selectedSubkey.isEmpty();
}

bool DConfHotkey::registerHotkey(QKeySequence key) {
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
    QString selectedSubkey = getSubkey(_name, subkeys);

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
    QString dconfPath = dconfRootPath + "/" + selectedSubkey;

    QProcess dconfReadName;
    dconfReadName.start("/usr/bin/dconf",
                        QStringList {
                            "read",
                            dconfPath + "name",
                        });
    dconfReadName.waitForFinished();
    QString currName;
    if (dconfReadName.exitCode() == 0) {
        currName = dconfReadName.readAllStandardOutput().trimmed();
    }

    QString newName = "'" + _name + "'";
    if (newName.compare(currName) != 0) {  // update only if different
        QProcess dconfWriteName;
        dconfWriteName.start("/usr/bin/dconf",
                             QStringList {
                                 "write",
                                 dconfPath + "name",
                                 newName,
                             });
        dconfWriteName.waitForFinished();
        isOk = isOk && (dconfWriteName.exitCode() == 0);
    }

    QProcess dconfReadCommand;
    dconfReadCommand.start("/usr/bin/dconf",
                        QStringList {
                            "read",
                            dconfPath + "command",
                        });
    dconfReadCommand.waitForFinished();
    QString currCommand;
    if (dconfReadCommand.exitCode() == 0) {
        currCommand = dconfReadCommand.readAllStandardOutput().trimmed();
    }

    QString newCommand = "'" + QCoreApplication::applicationFilePath() + "'";
    if (newCommand.compare(currCommand) != 0) {  // update only if different
        QProcess dconfWriteCommand;
        dconfWriteCommand.start("/usr/bin/dconf",
                                QStringList {
                                    "write",
                                    dconfPath + "command",
                                    newCommand,
                                });
        dconfWriteCommand.waitForFinished();
        isOk = isOk && (dconfWriteCommand.exitCode() == 0);
    }

    QProcess dconfReadBinding;
    dconfReadBinding.start("/usr/bin/dconf",
                           QStringList {
                               "read",
                               dconfPath + "binding",
                           });
    dconfReadBinding.waitForFinished();
    QString currBinding;
    if (dconfReadBinding.exitCode() == 0) {
        currBinding = dconfReadBinding.readAllStandardOutput().trimmed();
    }

    QString newBinding = "'" + keySequence + "'";
    if (newBinding.compare(currBinding) != 0) {  // update only if different
        QProcess dconfWriteBinding;
        dconfWriteBinding.start("/usr/bin/dconf",
                                QStringList {
                                    "write",
                                    dconfPath + "binding",
                                    newBinding,
                                });
        dconfWriteBinding.waitForFinished();
        isOk = isOk && (dconfWriteBinding.exitCode() == 0);
    }

    if (isOk) { isOk = isOk && applyKeybindings(); }
    if (!isOk) { qDebug().noquote() << "[DConfHotkey]" << "Registration failed!"; }
    return isOk;
}

bool DConfHotkey::unregisterHotkey() {
    QStringList subkeys = getSubkeys();
    QString selectedSubkey = getSubkey(_name, subkeys);
    if (selectedSubkey.isEmpty()) {
        qDebug().noquote() << "[DConfHotkey]" << "No registration found!";
        return false;
    }

    QProcess dconfReset;
    QString dconfPath = dconfRootPath + "/" + selectedSubkey;
    dconfReset.start("/usr/bin/dconf",
                     QStringList({
                         "reset",
                         "-f",
                         dconfPath
                     }));
    dconfReset.waitForFinished();
    bool isOk = (dconfReset.exitCode() == 0);

    if (isOk) { isOk = isOk && applyKeybindings(); }
    if (!isOk) { qDebug().noquote() << "[DConfHotkey]" << "Unregistration failed!"; }
    return isOk;
}
