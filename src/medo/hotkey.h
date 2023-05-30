/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */
// 2023-05-28: Initial version

#pragma once
#include <QObject>
#include <QKeySequence>

#if defined(Q_OS_WIN)
    #include "winHotkey.h"
#elif defined(Q_OS_LINUX)
    #include "dconfHotkey.h"
    #include "xcbHotkey.h"
#else
    #error "Only Linux and Windows are supported!"
#endif

namespace Medo { class Hotkey; }

class Hotkey : public QObject {  // because I don't want to mess with interfaces
        Q_OBJECT

    public:

        /*! Creates a new instance */
        explicit Hotkey(QString name, bool forceDConf, bool forceXcb, QObject* parent = nullptr);

        /*! Destroys the instance */
        ~Hotkey();

        /*! Registers hotkey.
         * \param key Hotkey. Cannot have more than one key combination. */
        bool registerHotkey(QKeySequence key);

        /*! Registers a different hotkey.
         * \param key Hotkey. Cannot have more than one key combination. */
        bool reregisterHotkey(QKeySequence key);

        /*! Disables currently registered hotkey. */
        bool unregisterHotkey();


        /*! Temporarily suspends hotkey capturing. Not thread-safe. */
        void suspend();

        /*! Resumes previously suspended capturing. Not thread-safe. */
        void resume();

    private:
#if defined(Q_OS_WIN)
        WinHotkey* _winHotkey = nullptr;
#elif defined(Q_OS_LINUX)
        DConfHotkey* _dconfHotkey = nullptr;
        XcbHotkey* _xcbHotkey = nullptr;
#endif
        bool _isRegistered = false;

    signals:

        /*! Signals hotkey has been activated */
        void activated();

    private slots:
        void onActivated();

};
