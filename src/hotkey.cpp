#include <QCoreApplication>
#include <QDebug>
#include "hotkey.h"

Hotkey::Hotkey(QObject *parent)
    : QObject(parent) {

    nativeInit();
    qApp->installNativeEventFilter(this);
}

Hotkey::~Hotkey() {
    if (_isRegistered) { unregisterHotkey(); }
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

    bool successful = nativeRegisterHotkey(key, modifiers);
    if (successful) { _isRegistered = true; }
    return successful;
}

/*!
 * \brief Disables currently registered hotkey.
 */
bool Hotkey::unregisterHotkey() {
    if (!_isRegistered) {
        qDebug() << "Hotkey not registered!";
        return false;
    }

    bool successful = nativeUnregisterHotkey();
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

#elif defined(Q_OS_LINUX)

void Hotkey::nativeInit() {
}

bool Hotkey::nativeRegisterHotkey(Qt::Key key, Qt::KeyboardModifiers modifiers) {
    uint16_t modValue = 0;
    if (modifiers & Qt::AltModifier) { modValue |= (1<<3); }
    if (modifiers & Qt::ControlModifier) { modValue |= (1<<2); }
    if (modifiers & Qt::ShiftModifier) { modValue |= (1<<0); }
    if (modifiers & ~(Qt::AltModifier | Qt::ControlModifier | Qt::ShiftModifier | Qt::MetaModifier)) {
        qDebug().noquote().nospace() << "Unrecognized modifiers (" << modifiers << ")!";
        return false;
    }

    KeySym keySymbol;
    if (((key >= Qt::Key_A) && (key <= Qt::Key_Z)) || ((key >= Qt::Key_0) && (key <= Qt::Key_9))) {
        keySymbol = key;
    } else if ((key >= Qt::Key_F1) && (key <= Qt::Key_F35)) {
        keySymbol = XK_F1 + (key - Qt::Key_F1);
    } else {
        qDebug().noquote().nospace() << "Unrecognized key (" << key << " in " << ")!";
        return false;
    }
    xcb_keycode_t keyValue = XKeysymToKeycode(QX11Info::display(), keySymbol);

    xcb_connection_t* connection = QX11Info::connection();
    auto cookie = xcb_grab_key_checked(connection, 1, static_cast<xcb_window_t>(QX11Info::appRootWindow()), modValue, keyValue, XCB_GRAB_MODE_ASYNC, XCB_GRAB_MODE_ASYNC);
    auto cookieError = xcb_request_check(connection, cookie);
    if (cookieError == nullptr) {
        _hotkeyMods = modValue;
        _hotkeyKey = keyValue;
        return true;
    } else {
        free(cookieError);
        return false;
    }
}

bool Hotkey::nativeUnregisterHotkey() {
    xcb_connection_t* connection = QX11Info::connection(); // xcb_connect(nullptr, nullptr); // QX11Info::connection();
    auto cookie = xcb_ungrab_key(connection, _hotkeyKey, static_cast<xcb_window_t>(QX11Info::appRootWindow()), _hotkeyMods);
    auto cookieError = xcb_request_check(connection, cookie);
    if (cookieError == nullptr) {
        return true;
    } else {
        free(cookieError);
        return false;
    }
}

bool Hotkey::nativeEventFilter(const QByteArray&, void* message, long*) {
    xcb_generic_event_t* e = static_cast<xcb_generic_event_t*>(message);
    if ((e->response_type & ~0x80) == XCB_KEY_PRESS) {
        xcb_key_press_event_t* ke = reinterpret_cast<xcb_key_press_event_t*>(e);
        xcb_keycode_t keyValue = ke->detail;
        uint16_t modValue = ke->state & (XCB_MOD_MASK_SHIFT|XCB_MOD_MASK_CONTROL|XCB_MOD_MASK_1); //xmodmap -pm (to see masks: default 1 Alt, 4 Super)
        if ((_hotkeyMods == modValue) && (_hotkeyKey == keyValue)) {
            emit activated();
            return true;
        }
    }
    return false;
}

#endif
