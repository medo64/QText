#include "hotkey.h"
#include <QCoreApplication>
#include <QDebug>
#include <QGuiApplication>
#include <QProcess>

Hotkey::Hotkey(QString name, bool forceDConf, bool forceXcb, QObject* parent)
    : QObject(parent) {
#if defined(Q_OS_WIN)
    Q_UNUSED(name);
    Q_UNUSED(forceDConf);
    Q_UNUSED(forceXcb);
    _winHotkey = new WinHotkey(this);
    connect(_winHotkey, &WinHotkey::activated, this, &Hotkey::onActivated);
#elif defined(Q_OS_LINUX)
    bool useDConf = false;
    bool useXcb = false;
    if (forceDConf) {
        useDConf = true;
    } else if (forceXcb) {
        useXcb = true;
    } else {
        QString platformName = QGuiApplication::platformName();
        QString sessionType = getenv("XDG_SESSION_TYPE");
        auto isWayland = (platformName.compare("wayland") == 0) || (sessionType.compare("wayland") == 0);
        if (isWayland) {
            useDConf = true;
        } else {
            useXcb = true;
        }
    }
    if (useDConf) {
        _dconfHotkey = new DConfHotkey(name, this);
    } else if (useXcb) {
        _xcbHotkey = new XcbHotkey(this);
        connect(_xcbHotkey, &XcbHotkey::activated, this, &Hotkey::onActivated);
    }
#endif
}

Hotkey::~Hotkey() {
#if defined(Q_OS_WIN)
    if (_winHotkey != nullptr) {
        delete  _winHotkey;
        _winHotkey = nullptr;
    }
#elif defined(Q_OS_LINUX)
    if (_dconfHotkey != nullptr) {
        delete  _dconfHotkey;
        _dconfHotkey = nullptr;
    }
    if (_xcbHotkey != nullptr) {
        delete  _xcbHotkey;
        _xcbHotkey = nullptr;
    }
#endif
}

bool Hotkey::registerHotkey(QKeySequence key) {
#if defined(Q_OS_WIN)
    if (_winHotkey != nullptr) {
        return _winHotkey->registerHotkey(key);
    }
#elif defined(Q_OS_LINUX)
    if (_dconfHotkey != nullptr) {
        return _dconfHotkey->registerHotkey(key);
    }
    if (_xcbHotkey != nullptr) {
        return _xcbHotkey->registerHotkey(key);
    }
#endif
    return false;
}

bool Hotkey::unregisterHotkey() {
#if defined(Q_OS_WIN)
    if (_winHotkey != nullptr) {
        return _winHotkey->unregisterHotkey();
    }
#elif defined(Q_OS_LINUX)
    if (_dconfHotkey != nullptr) {
        return _dconfHotkey->unregisterHotkey();
    }
    if (_xcbHotkey != nullptr) {
        return _xcbHotkey->unregisterHotkey();
    }
#endif
    return false;
}

void Hotkey::suspend() {
#if defined(Q_OS_WIN)
    if (_winHotkey != nullptr) {
        _winHotkey->suspend();
    }
#elif defined(Q_OS_LINUX)
    if (_dconfHotkey != nullptr) {
        // no suspend
    }
    if (_xcbHotkey != nullptr) {
        _xcbHotkey->suspend();
    }
#endif
}

void Hotkey::resume() {
#if defined(Q_OS_WIN)
    if (_winHotkey != nullptr) {
        _winHotkey->resume();
    }
#elif defined(Q_OS_LINUX)
    if (_dconfHotkey != nullptr) {
        // no resume
    }
    if (_xcbHotkey != nullptr) {
        _xcbHotkey->resume();
    }
#endif
}


void Hotkey::onActivated() {
    emit activated();
}
