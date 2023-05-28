#include "winHotkey.h"
#include <QCoreApplication>
#include <QDebug>
#include <windows.h>

WinHotkey::WinHotkey(QObject* parent)
    : QObject(parent) {

    nativeInit();
    qApp->installNativeEventFilter(this);
}

WinHotkey::~WinHotkey() {
    if (_isRegistered) { unregisterHotkey(); }
    qApp->removeNativeEventFilter(this);
}


bool WinHotkey::registerHotkey(QKeySequence key) {
    if (_isRegistered) {
        qDebug().noquote() << "[WinHotkey]" << "Hotkey already registered!";
        return false;
    }

    if (key.count() != 1) {
        qDebug().noquote() << "[WinHotkey]" << "Must have only one key combination!";
        return false;
    }

    auto keyboardKey = Qt::Key(key[0] & static_cast<int>(~Qt::KeyboardModifierMask));
    auto keyboardModifiers = Qt::KeyboardModifiers(key[0] & static_cast<int>(Qt::KeyboardModifierMask));

    bool successful = nativeRegisterHotkey(keyboardKey, keyboardModifiers);
    if (successful) {
        _key = key;
        _isRegistered = true;
    } else {
        qDebug().noquote().nospace() << "[WinHotkey] Failed to register hotkey " << key.toString(QKeySequence::PortableText) << "!";
    }
    return successful;
}

bool WinHotkey::unregisterHotkey() {
    if (!_isRegistered) {
        qDebug().noquote() << "[WinHotkey]" << "Hotkey not registered!";
        return false;
    }

    bool successful = nativeUnregisterHotkey();
    if (successful) {
        _isRegistered = false;
    } else {
        qDebug().noquote() << "[WinHotkey]" << "Failed to deregister hotkey" << _key.toString(QKeySequence::PortableText) << "!";
    }
    return successful;
}

void WinHotkey::suspend() {
    ++suspensionLevel;
}

void WinHotkey::resume() {
    --suspensionLevel;
}


std::atomic<uint64_t> WinHotkey::_globalHotkeyCounter(0);

void WinHotkey::nativeInit() {
    _hotkeyId = reinterpret_cast<WPARAM>(_globalHotkeyCounter++);
}

bool WinHotkey::nativeRegisterHotkey(Qt::Key key, Qt::KeyboardModifiers modifiers) {
    if (_hotkeyId > 0xBFFF) {
        qDebug().noquote() << "[WinHotkey]" << "No more Hotkey IDs!"; //cannot be bothered to track IDs as this is unlikely
        return false;
    }

    uint modValue = 0;
    if (modifiers & Qt::AltModifier)     { modValue += MOD_ALT; }
    if (modifiers & Qt::ControlModifier) { modValue += MOD_CONTROL; }
    if (modifiers & Qt::ShiftModifier)   { modValue += MOD_SHIFT; }
    if (modifiers & Qt::MetaModifier)    { modValue += MOD_WIN; }

    if (modifiers & ~(Qt::AltModifier | Qt::ControlModifier | Qt::ShiftModifier | Qt::MetaModifier)) {
        qDebug().noquote().nospace() << "[Hotkey] " << "Unrecognized modifiers (" << modifiers << ")!";
        return false;
    }

    uint keyValue;
    if (((key >= Qt::Key_A) && (key <= Qt::Key_Z)) || ((key >= Qt::Key_0) && (key <= Qt::Key_9))) {
        keyValue = key;
    } else if ((key >= Qt::Key_F1) && (key <= Qt::Key_F24)) {
        keyValue = VK_F1 + (key - Qt::Key_F1);
    } else {
        qDebug().noquote().nospace() << "[WinHotkey] " << "Unrecognized key (" << key << ")!";
        return false;
    }

    return RegisterHotKey(nullptr, static_cast<int>(_hotkeyId), modValue, keyValue);
}

bool WinHotkey::nativeUnregisterHotkey() {
    return UnregisterHotKey(nullptr, static_cast<int>(_hotkeyId));
}


bool WinHotkey::nativeEventFilter(const QByteArray&, void* message, long*) {
    MSG* msg = static_cast<MSG*>(message);
    if (msg->message == WM_HOTKEY) {
        if (msg->wParam == static_cast<WPARAM>(_hotkeyId)) {
            if (suspensionLevel <= 0) {
                qDebug().noquote() << "[WinHotkey]" << "Hotkey" << _key.toString(QKeySequence::PortableText) << "detected";
                emit activated();
                return true;
            } else {
                qDebug().noquote() << "[WinHotkey]" << "Hotkey" << _key.toString(QKeySequence::PortableText) << "ignored";
                return false;
            }
        }
    }
    return false;
}
