/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */
// 2023-05-28: Split into separate files for Dconf/XCB/Wind
// 2020-06-06: Added support for suspend and resume
// 2019-10-04: Reorganizing includes to minimize conflicts
// 2019-09-16: Allowing Meta/Win key as a hotkey modifier
// 2019-07-05: Initial version

#pragma once
#include <QAbstractNativeEventFilter>
#include <QByteArray>
#include <QObject>
#include <QKeySequence>

namespace Medo { class Medo; }

class XcbHotkey : public QObject, QAbstractNativeEventFilter {
        Q_OBJECT

    public:

        /*! Creates a new instance */
        explicit XcbHotkey(QObject* parent = nullptr);

        /*! Destroys the instance */
        ~XcbHotkey();

        /*! Registers hotkey.
         * \param key Hotkey. Cannot have more than one key combination. */
        bool registerHotkey(QKeySequence key);

        /*! Disables currently registered hotkey. */
        bool unregisterHotkey();

        /*! Temporarily suspends hotkey capturing. Not thread-safe. */
        void suspend();

        /*! Resumes previously suspended capturing. Not thread-safe. */
        void resume();


    signals:

        /*! Signals hotkey has been activated */
        void activated();


    protected:
        bool nativeEventFilter(const QByteArray& eventType, void* message, long* result) override;

    private:
        bool _isRegistered = false;
        int suspensionLevel = 0;
        QKeySequence _key;
        void nativeInit();
        bool nativeRegisterHotkey(Qt::Key key, Qt::KeyboardModifiers modifiers);
        bool nativeUnregisterHotkey();

        uint16_t _hotkeyMods;
        uint8_t _hotkeyKey; //actually xcb_keycode_t; just 64-bit integer here to avoid include of X11 headers

};
