#include "hotkey.h"
#include <QCoreApplication>
#include <QDebug>
#if defined(Q_OS_WIN)
    #include <windows.h>
#elif defined(Q_OS_LINUX)
    #include <QX11Info>
    #include <xcb/xcb.h>
    #include <X11/keysym.h>
    #include <X11/Xlib.h>
#else
    #error "Only Linux and Windows are supported!"
#endif

Hotkey::Hotkey(QObject* parent)
    : QObject(parent) {

    nativeInit();
    qApp->installNativeEventFilter(this);
}

Hotkey::~Hotkey() {
    if (_isRegistered) { unregisterHotkey(); }
    qApp->removeNativeEventFilter(this);
}


bool Hotkey::registerHotkey(QKeySequence key) {
    if (_isRegistered) {
        qDebug().noquote() << "[Hotkey]" << "Hotkey already registered!";
        return false;
    }

    if (key.count() != 1) {
        qDebug().noquote() << "[Hotkey]" << "Must have only one key combination!";
        return false;
    }

    auto keyboardKey = Qt::Key(key[0] & static_cast<int>(~Qt::KeyboardModifierMask));
    auto keyboardModifiers = Qt::KeyboardModifiers(key[0] & static_cast<int>(Qt::KeyboardModifierMask));

    bool successful = nativeRegisterHotkey(keyboardKey, keyboardModifiers);
    if (successful) {
        _key = key;
        _isRegistered = true;
    } else {
        qDebug().noquote().nospace() << "[Hotkey] Failed to register hotkey " << key.toString(QKeySequence::PortableText) << "!";
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
        qDebug().noquote() << "[Hotkey]" << "Failed to deregister hotkey" << _key.toString(QKeySequence::PortableText) << "!";
    }
    return successful;
}

void Hotkey::suspend() {
    ++suspensionLevel;
}

void Hotkey::resume() {
    --suspensionLevel;
}


#if defined(Q_OS_WIN)

std::atomic<uint64_t> Hotkey::_globalHotkeyCounter(0);

void Hotkey::nativeInit() {
    _hotkeyId = reinterpret_cast<WPARAM>(_globalHotkeyCounter++);
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

    return RegisterHotKey(nullptr, static_cast<int>(_hotkeyId), modValue, keyValue);
}

bool Hotkey::nativeUnregisterHotkey() {
    return UnregisterHotKey(nullptr, static_cast<int>(_hotkeyId));
}


bool Hotkey::nativeEventFilter(const QByteArray&, void* message, long*) {
    MSG* msg = static_cast<MSG*>(message);
    if (msg->message == WM_HOTKEY) {
        if (msg->wParam == static_cast<WPARAM>(_hotkeyId)) {
            if (suspensionLevel <= 0) {
                qDebug().noquote() << "[Hotkey]" << "Hotkey" << _key.toString(QKeySequence::PortableText) << "detected";
                emit activated();
                return true;
            } else {
                qDebug().noquote() << "[Hotkey]" << "Hotkey" << _key.toString(QKeySequence::PortableText) << "ignored";
                return false;
            }
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
    auto hotkeyKey = reinterpret_cast<xcb_keycode_t>(_hotkeyKey);
    xcb_connection_t* connection = QX11Info::connection(); // xcb_connect(nullptr, nullptr); // QX11Info::connection();
    auto cookie = xcb_ungrab_key(connection, hotkeyKey, static_cast<xcb_window_t>(QX11Info::appRootWindow()), _hotkeyMods);
    auto cookieError = xcb_request_check(connection, cookie);
    if (cookieError == nullptr) {
        //Workaround for caps/num lock
        xcb_ungrab_key(connection, hotkeyKey, static_cast<xcb_window_t>(QX11Info::appRootWindow()), _hotkeyMods | XCB_MOD_MASK_LOCK);
        xcb_ungrab_key(connection, hotkeyKey, static_cast<xcb_window_t>(QX11Info::appRootWindow()), _hotkeyMods | XCB_MOD_MASK_2);
        xcb_ungrab_key(connection, hotkeyKey, static_cast<xcb_window_t>(QX11Info::appRootWindow()), _hotkeyMods | XCB_MOD_MASK_LOCK | XCB_MOD_MASK_2);

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
            if (suspensionLevel <= 0) {
                qDebug().noquote() << "[Hotkey]" << "Hotkey" << _key.toString(QKeySequence::PortableText) << "detected";
                emit activated();
                return true;
            } else {
                qDebug().noquote() << "[Hotkey]" << "Hotkey" << _key.toString(QKeySequence::PortableText) << "ignored";
                return false;
            }
        }
    }
    return false;
}

#endif
