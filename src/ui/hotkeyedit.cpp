#include <QDebug>
#include <QEvent>
#include <QKeyEvent>
#include "hotkeyedit.h"

HotkeyEdit::HotkeyEdit(QWidget* parent)
    : QLineEdit(parent) {
    setReadOnly(true);
}


void HotkeyEdit::setHotkey(Hotkey* hotkey, QKeySequence key) {
    _hotkey = hotkey;
    _oldKey = key;
    setPlaceholderText(_oldKey.toString(QKeySequence::NativeText));
}

void HotkeyEdit::setNewKey(QKeySequence key) {
    _newKey = key;
    if ((key == 0) || (key == _oldKey)) {
        setText("");
        setPlaceholderText(_oldKey.toString(QKeySequence::NativeText));
    } else {
        setText(key.toString(QKeySequence::NativeText));
    }
}


bool HotkeyEdit::event(QEvent* event) {
    if (event->type() == QEvent::KeyPress) {
        QKeyEvent* e = static_cast<QKeyEvent*>(event);
        auto eModifiers = e->modifiers();
        auto eKey = e->key();

        if (eModifiers & Qt::ShiftModifier)   { eKey += Qt::SHIFT; eModifiers ^= Qt::ShiftModifier; }
        if (eModifiers & Qt::ControlModifier) { eKey += Qt::CTRL;  eModifiers ^= Qt::ControlModifier; }
        if (eModifiers & Qt::AltModifier)     { eKey += Qt::ALT;   eModifiers ^= Qt::AltModifier; }
        if (eModifiers & Qt::MetaModifier)    { eKey += Qt::META;  eModifiers ^= Qt::MetaModifier; }

        if (eModifiers == 0) { //all modifiers have been processed
            QKeySequence key = QKeySequence(eKey);
            auto keyboardKey = Qt::Key(key[0] & static_cast<int>(~Qt::KeyboardModifierMask));
            auto keyboardModifiers = Qt::KeyboardModifiers(key[0] & static_cast<int>(Qt::KeyboardModifierMask));

            if ((keyboardKey > 0) && (keyboardKey < 0xFFFF) && (keyboardModifiers != Qt::NoModifier) && (keyboardModifiers != Qt::ShiftModifier)) {
                setText(key.toString(QKeySequence::NativeText));
                _newKey = key;
                return true;
            }
            if ((keyboardModifiers == Qt::NoModifier) && (keyboardKey == Qt::Key_Escape) && (_newKey != 0)) {
                setText(QString());
                _newKey = 0;
                return true;
            }
        }
    }
    return QLineEdit::event(event);
}

void HotkeyEdit::focusInEvent(QFocusEvent* e) {
    if (_hotkey != nullptr) { _hotkey->suspend(); }
    QLineEdit::focusInEvent(e);
    setPlaceholderText("Press hotkey combination or Escape to reset");
}

void HotkeyEdit::focusOutEvent(QFocusEvent* e) {
    if (_hotkey != nullptr) { _hotkey->resume(); }
    QLineEdit::focusOutEvent(e);
    setPlaceholderText(_oldKey.toString(QKeySequence::NativeText));
}
