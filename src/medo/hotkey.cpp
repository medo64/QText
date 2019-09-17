#include "hotkey.h"
#include <QCoreApplication>
#include <QDebug>

Hotkey::Hotkey(QObject* parent)
    : QObject(parent) {

    nativeInit();
    qApp->installNativeEventFilter(this);
}

Hotkey::~Hotkey() {
    if (_isRegistered) { unregisterHotkey(); }
    qApp->removeNativeEventFilter(this);
}


bool Hotkey::registerHotkey(QKeySequence keySequence) {
    if (_isRegistered) {
        qDebug().noquote() << "[Hotkey]" << "Hotkey already registered!";
        return false;
    }

    if (keySequence.count() != 1) {
        qDebug().noquote() << "[Hotkey]" << "Must have only one key combination!";
        return false;
    }

    auto key = Qt::Key(keySequence[0] & static_cast<int>(~Qt::KeyboardModifierMask));
    auto modifiers = Qt::KeyboardModifiers(keySequence[0] & static_cast<int>(Qt::KeyboardModifierMask));

    bool successful = nativeRegisterHotkey(key, modifiers);
    if (successful) {
        _isRegistered = true;
    } else {
        qDebug().noquote() << "[Hotkey]" << "Failed to register hotkey (" + keySequence.toString() + ")!";
    }
    return successful;
}

bool Hotkey::unregisterHotkey() {
    if (!_isRegistered) {
        qDebug().noquote() << "[Hotkey]" << "Hotkey not registered!";
        return false;
    }

    bool successful = nativeUnregisterHotkey();
    if (successful) {
        _isRegistered = false;
    } else {
        qDebug().noquote() << "[Hotkey]" << "Failed to deregister hotkey!";
    }
    return successful;
}


#if defined(Q_OS_WIN)

std::atomic<WPARAM> Hotkey::_globalHotkeyCounter(0);

void Hotkey::nativeInit() {
    _hotkeyId = static_cast<int>(_globalHotkeyCounter++);
}

bool Hotkey::nativeRegisterHotkey(Qt::Key key, Qt::KeyboardModifiers modifiers) {
    if (_hotkeyId > 0xBFFF) {
        qDebug().noquote() << "[Hotkey]" << "No more Hotkey IDs!"; //cannot be bothered to track IDs as this is unlikely
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
        qDebug().noquote().nospace() << "[Hotkey] " << "Unrecognized key (" << key << ")!";
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
        }
    }
    return false;
}

#elif defined(Q_OS_LINUX)

void Hotkey::nativeInit() {
}

bool Hotkey::nativeRegisterHotkey(Qt::Key key, Qt::KeyboardModifiers modifiers) {
    uint16_t modValue = 0;
    if (modifiers & Qt::AltModifier)     { modValue |= XCB_MOD_MASK_1; }
    if (modifiers & Qt::ControlModifier) { modValue |= XCB_MOD_MASK_CONTROL; }
    if (modifiers & Qt::ShiftModifier)   { modValue |= XCB_MOD_MASK_SHIFT; }
    if (modifiers & Qt::MetaModifier)    { modValue |= XCB_MOD_MASK_4; }
    if (modifiers & ~(Qt::AltModifier | Qt::ControlModifier | Qt::ShiftModifier | Qt::MetaModifier)) {
        qDebug().noquote().nospace() << "[Hotkey] " << "Unrecognized modifiers (" << modifiers << ")!";
        return false;
    }

    KeySym keySymbol;
    if (((key >= Qt::Key_A) && (key <= Qt::Key_Z)) || ((key >= Qt::Key_0) && (key <= Qt::Key_9))) {
        keySymbol = key;
    } else if ((key >= Qt::Key_F1) && (key <= Qt::Key_F35)) {
        keySymbol = XK_F1 + (key - Qt::Key_F1);
    } else {
        qDebug().noquote().nospace() << "[Hotkey] " << "Unrecognized key (" << key << " in " << ")!";
        return false;
    }
    xcb_keycode_t keyValue = XKeysymToKeycode(QX11Info::display(), keySymbol);

    xcb_connection_t* connection = QX11Info::connection();
    auto cookie = xcb_grab_key_checked(connection, 1, static_cast<xcb_window_t>(QX11Info::appRootWindow()), modValue, keyValue, XCB_GRAB_MODE_ASYNC, XCB_GRAB_MODE_ASYNC);
    auto cookieError = xcb_request_check(connection, cookie);
    if (cookieError == nullptr) {
        _hotkeyMods = modValue;
        _hotkeyKey = keyValue;

        //Workaround for caps/num lock
        xcb_grab_key_checked(connection, 1, static_cast<xcb_window_t>(QX11Info::appRootWindow()), modValue | XCB_MOD_MASK_LOCK,                  keyValue, XCB_GRAB_MODE_ASYNC, XCB_GRAB_MODE_ASYNC);
        xcb_grab_key_checked(connection, 1, static_cast<xcb_window_t>(QX11Info::appRootWindow()), modValue | XCB_MOD_MASK_2,                     keyValue, XCB_GRAB_MODE_ASYNC, XCB_GRAB_MODE_ASYNC);
        xcb_grab_key_checked(connection, 1, static_cast<xcb_window_t>(QX11Info::appRootWindow()), modValue | XCB_MOD_MASK_LOCK | XCB_MOD_MASK_2, keyValue, XCB_GRAB_MODE_ASYNC, XCB_GRAB_MODE_ASYNC);

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
        //Workaround for caps/num lock
        xcb_ungrab_key(connection, _hotkeyKey, static_cast<xcb_window_t>(QX11Info::appRootWindow()), _hotkeyMods | XCB_MOD_MASK_LOCK);
        xcb_ungrab_key(connection, _hotkeyKey, static_cast<xcb_window_t>(QX11Info::appRootWindow()), _hotkeyMods | XCB_MOD_MASK_2);
        xcb_ungrab_key(connection, _hotkeyKey, static_cast<xcb_window_t>(QX11Info::appRootWindow()), _hotkeyMods | XCB_MOD_MASK_LOCK | XCB_MOD_MASK_2);

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
        uint16_t modValue = ke->state & (XCB_MOD_MASK_SHIFT | XCB_MOD_MASK_CONTROL | XCB_MOD_MASK_1 | XCB_MOD_MASK_4); //xmodmap -pm (to see masks: default 1 Alt, 4 Super)
        if ((_hotkeyMods == modValue) && (_hotkeyKey == keyValue)) {
            qDebug().noquote() << "[Hotkey]" << "Hotkey detected";
            emit activated();
            return true;
        }
    }
    return false;
}

#endif
