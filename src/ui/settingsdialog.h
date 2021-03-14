#pragma once
#include <QAbstractButton>
#include <QDialog>
#include "medo/hotkey.h"

namespace Ui {
    class SettingsDialog;
}

class SettingsDialog : public QDialog {
        Q_OBJECT

    public:
        explicit SettingsDialog(QWidget* parent = nullptr, Hotkey* hotkey = nullptr);
        ~SettingsDialog();
        bool changedAlwaysOnTop() const { return _changedAlwaysOnTop; }
        bool changedAutostart() const { return _changedAutostart; }
        bool changedDataPath() const { return _changedDataPath; }
        bool changedForceDarkMode() const { return _changedForceDarkMode; }
        bool changedForcePlainCopyPaste() const { return _changedForcePlainCopyPaste; }
        bool changedHotkey() const { return _changedHotkey; }
        bool changedHotkeyTogglesVisibility() const { return _changedHotkeyTogglesVisibility; }
        bool changedMinimizeToTray() const { return _changedMinimizeToTray; }
        bool changedShowInTaskbar() const { return _changedShowInTaskbar; }
        bool changedShowMarkdown() const { return _changedShowMarkdown; }
        bool changedTabTextColorPerType() const { return _changedTabTextColorPerType; }
        bool changedUseHtmlByDefault() const { return _changedUseHtmlByDefault; }

    protected:
        void keyPressEvent(QKeyEvent* event);
        void accept();

    private:
        Ui::SettingsDialog* ui;
        void reset();
        void restoreDefaults();
        bool _changedAlwaysOnTop;
        bool _changedAutostart;
        bool _changedDataPath;
        bool _changedForceDarkMode;
        bool _changedForcePlainCopyPaste;
        bool _changedHotkey;
        bool _changedHotkeyTogglesVisibility;
        bool _changedMinimizeToTray;
        bool _changedShowInTaskbar;
        bool _changedShowMarkdown;
        bool _changedTabTextColorPerType;
        bool _changedUseHtmlByDefault;
        bool _oldAlwaysOnTop;
        bool _oldAutostart;
        QString _oldDataPath;
        bool _oldForceDarkMode;
        bool _oldForcePlainCopyPaste;
        QKeySequence _oldHotkey;
        bool _oldHotkeyTogglesVisibility;
        bool _oldMinimizeToTray;
        bool _oldShowInTaskbar;
        bool _oldShowMarkdown;
        bool _oldTabTextColorPerType;
        bool _oldUseHtmlByDefault;

    private slots:
        void onButtonClicked(QAbstractButton* button);
        void onDataPathClicked();

};
