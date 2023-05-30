#include <QDebug>
#include <QEvent>
#include <QKeyEvent>
#include "hotkeyedit.h"

static const char* NoShortcutText = "No shortcut assigned";
static const char* ChangeShortcutText = "Press hotkey combination";

static QString getPlaceholderText(QKeySequence key) {
    if (key == 0) {
        return QString(NoShortcutText);
    } else {
        return key.toString(QKeySequence::NativeText);
    }
}

HotkeyEdit::HotkeyEdit(QWidget* parent)
    : QLineEdit(parent) {
    setReadOnly(true);
}


void HotkeyEdit::setHotkey(Hotkey* hotkey, QKeySequence key) {
    _hotkey = hotkey;
    _oldKey = key;
    _newKey = key;
    setPlaceholderText(getPlaceholderText(key));
}

void HotkeyEdit::setNewKey(QKeySequence key) {
    _newKey = key;
    if (key == _oldKey) {
        setText(QString());
        setPlaceholderText(getPlaceholderText(_oldKey));
    } else {
        setPlaceholderText(getPlaceholderText(key));
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

            if ((keyboardModifiers == Qt::NoModifier) && (keyboardKey == Qt::Key_Escape)) {  // cancel edit
                _newKey = _oldKey;
                setText(QString());
                setPlaceholderText(getPlaceholderText(_oldKey));
                return true;
            } else if ((keyboardModifiers == Qt::NoModifier) && (keyboardKey == Qt::Key_Backspace)) {
                _newKey = 0;
                if (_oldKey != _newKey) { setText(QString(NoShortcutText)); }  // mark as changed only if different
                setPlaceholderText(getPlaceholderText(0));
                return true;
            } else if ((keyboardKey > 0) && (keyboardKey < 0xFFFF) && (keyboardModifiers != Qt::NoModifier) && (keyboardModifiers != Qt::ShiftModifier)) {
                _newKey = key;
                if (_oldKey != _newKey) { setText(QString(NoShortcutText)); }  // mark as changed only if different
                setText(key.toString(QKeySequence::NativeText));
                setPlaceholderText(getPlaceholderText(_oldKey));
                return true;
            }
        }
    }
    return QLineEdit::event(event);
}

void HotkeyEdit::focusInEvent(QFocusEvent* e) {
    if (_hotkey != nullptr) { _hotkey->suspend(); }
    QLineEdit::focusInEvent(e);
    setPlaceholderText(QString(ChangeShortcutText));
}

void HotkeyEdit::focusOutEvent(QFocusEvent* e) {
    if (_hotkey != nullptr) { _hotkey->resume(); }
    QLineEdit::focusOutEvent(e);
    setPlaceholderText(getPlaceholderText(_newKey));
}
