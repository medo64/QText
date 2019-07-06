#include "hotkey.h"
#include <QCoreApplication>
#include <QDebug>

Hotkey::Hotkey(QObject *parent)
    : QObject(parent) {

#if defined(Q_OS_WIN)
    nativeInit();
#endif

    qApp->installNativeEventFilter(this);
}

Hotkey::~Hotkey() {
    if (_isRegistered) {
        unregisterHotkey();
    }
    qApp->removeNativeEventFilter(this);
}


/*!
 * \brief Registers hotkey.
 * \param keySequence Hotkey.
 */
bool Hotkey::registerHotkey(QKeySequence keySequence) {
    if (_isRegistered) {
        qDebug() << "Hotkey already registered!";
        return false;
    }

    if (keySequence.count() != 1) {
        qDebug() << "Must have only one key combination!";
        return false;
    }

    auto key = Qt::Key(keySequence[0] & static_cast<int>(~Qt::KeyboardModifierMask));
    auto modifiers = Qt::KeyboardModifiers(keySequence[0] & static_cast<int>(Qt::KeyboardModifierMask));

#if defined(Q_OS_WIN)
    bool successful = nativeRegisterHotkey(key, modifiers);
#else
    return false;
#endif

    if (successful) { _isRegistered = true; }
    return successful;
}

/*!
 * \brief Disables currently registered hotkey.
 */
bool Hotkey::unregisterHotkey() {
#if defined(Q_OS_WIN)
    bool successful = nativeUnregisterHotkey();
#else
    return false;
#endif

    if (successful) { _isRegistered = false; }
    return successful;
}


#if defined(Q_OS_WIN)

std::atomic<WPARAM> Hotkey::_globalHotkeyCounter(0);

void Hotkey::nativeInit() {
    _hotkeyId = static_cast<int>(_globalHotkeyCounter++);
}

bool Hotkey::nativeRegisterHotkey(Qt::Key key, Qt::KeyboardModifiers modifiers) {
    if (_hotkeyId > 0xBFFF) {
        qDebug() << "No more Hotkey IDs!"; //cannot be bothered to track IDs as this is unlikely
        return false;
    }

    uint modValue = 0;
    if (modifiers &  Qt::AltModifier) { modValue += MOD_ALT; }
    if (modifiers &  Qt::ControlModifier) { modValue += MOD_CONTROL; }
    if (modifiers &  Qt::ShiftModifier) { modValue += MOD_SHIFT; }

    if (modifiers & ~(Qt::AltModifier | Qt::ControlModifier | Qt::ShiftModifier)) {
        qDebug().noquote().nospace() << "Unrecognized modifiers (" << modifiers << ")!";
        return false;
    }

    uint keyValue;
    if (((key >= Qt::Key_A) && (key <= Qt::Key_Z)) || ((key >= Qt::Key_0) && (key <= Qt::Key_9))) {
        keyValue = key;
    } else if ((key >= Qt::Key_F1) && (key <= Qt::Key_F24)) {
        keyValue = VK_F1 + (key - Qt::Key_F1);
    } else {
        qDebug().noquote().nospace() << "Unrecognized key (" << key << " in " << ")!";
        return false;
    }

    return RegisterHotKey(nullptr, _hotkeyId, modValue, keyValue);
}

bool Hotkey::nativeUnregisterHotkey() {
    return UnregisterHotKey(nullptr, _hotkeyId);
}


bool Hotkey::nativeEventFilter(const QByteArray&, void* message, long*) {
    MSG* msg = static_cast<MSG*>(message);
    if (msg->message == WM_HOTKEY) {
        if (msg->wParam == static_cast<WPARAM>(_hotkeyId)) {
            emit activated();
            return true;
        } else {
            return false;
        }
    } else {
        return false;
    }
}

#endif
