#pragma once
#include <QLineEdit>
#include "medo/hotkey.h"

class HotkeyEdit: public QLineEdit {
        Q_OBJECT

    public:
        HotkeyEdit(QWidget* parent = nullptr);
        void setHotkey(Hotkey* hotkey, QKeySequence key);
        QKeySequence newKey() const { return _newKey; }
        void setNewKey(QKeySequence key);

    protected:
        bool event(QEvent* event);
        void focusInEvent(QFocusEvent* e);
        void focusOutEvent(QFocusEvent* e);

    private :
        Hotkey* _hotkey = nullptr;
        QKeySequence _oldKey;
        QKeySequence _newKey;

};
