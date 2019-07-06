#ifndef HOTKEY_H
#define HOTKEY_H

#include <QAbstractNativeEventFilter>
#include <QObject>
#include <QKeySequence>

#if defined(Q_OS_WIN)
    #include <windows.h>
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
#endif

    signals:
        void activated();

    public slots:
};

#endif // HOTKEY_H
