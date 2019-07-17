/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

// 2019-07-05: Initial version

#ifndef HOTKEY_H
#define HOTKEY_H

#include <QAbstractNativeEventFilter>
#include <QByteArray>
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
#else
    #error "Only Linux and Windows are supported!"
#endif

namespace Medo { class Hotkey; }

class Hotkey : public QObject, QAbstractNativeEventFilter {
    Q_OBJECT

    public:

        /*! Creates a new instance */
        explicit Hotkey(QObject* parent = nullptr);

        /*! Destroys the instance */
        ~Hotkey() override;

        /*! Registers hotkey.
         * \param keySequence Hotkey. */
        bool registerHotkey(QKeySequence key);

        /*! Disables currently registered hotkey. */
        bool unregisterHotkey();

    signals:

        /*! Signals hotkey has been activated */
        void activated();


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

    public slots:
};

#endif // HOTKEY_H
