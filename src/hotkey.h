#ifndef HOTKEY_H
#define HOTKEY_H

#include <QAbstractNativeEventFilter>
#include <QObject>
#include <QKeySequence>

#if defined(Q_OS_WIN)
    #include <windows.h>
#elif defined(Q_OS_LINUX)
    #include <QX11Info>
    #include <xcb/xcb.h>
    #include <X11/keysym.h>
    #include <X11/Xlib.h>
    //workaround for compiler errors
    #undef Bool
    #undef CursorShape
    #undef Expose
    #undef KeyPress
    #undef KeyRelease
    #undef FocusIn
    #undef FocusOut
    #undef FontChange
    #undef None
    #undef Status
    #undef Unsorted
#endif

class Hotkey : public QObject, QAbstractNativeEventFilter {
    Q_OBJECT

    public:
        explicit Hotkey(QObject* parent = nullptr);
        ~Hotkey() override;
        bool registerHotkey(QKeySequence key);
        bool unregisterHotkey();

    protected:
        bool nativeEventFilter(const QByteArray& eventType, void* message, long* result) override;

    private:
        bool _isRegistered = false;
        void nativeInit();
        bool nativeRegisterHotkey(Qt::Key key, Qt::KeyboardModifiers modifiers);
        bool nativeUnregisterHotkey();
#if defined(Q_OS_WIN)
        static std::atomic<WPARAM> _globalHotkeyCounter;
        int _hotkeyId = 0; //0x0000 through 0xBFFF
#elif defined(Q_OS_LINUX)
        uint16_t _hotkeyMods;
        xcb_keycode_t _hotkeyKey;
#endif

    signals:
        void activated();

    public slots:
};

#endif // HOTKEY_H
