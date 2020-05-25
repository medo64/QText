#pragma once

#include <QAbstractButton>
#include <QDialog>

namespace Ui {
    class SettingsDialog;
}

class SettingsDialog : public QDialog {
        Q_OBJECT

    public:
        explicit SettingsDialog(QWidget* parent = nullptr);
        ~SettingsDialog();
        bool changedAlwaysOnTop() const { return _changedAlwaysOnTop; }
        bool changedAutostart() const { return _changedAutostart; }
        bool changedMinimizeToTray() const { return _changedMinimizeToTray; }
        bool changedShowInTaskbar() const { return _changedShowInTaskbar; }
        bool changedTabTextColorPerType() const { return _changedTabTextColorPerType; }

    protected:
        void keyPressEvent(QKeyEvent* event);
        void accept();

    private:
        Ui::SettingsDialog* ui;
        void reset();
        void restoreDefaults();
        bool _changedAlwaysOnTop;
        bool _changedAutostart;
        bool _changedMinimizeToTray;
        bool _changedShowInTaskbar;
        bool _changedTabTextColorPerType;
        bool _oldAlwaysOnTop;
        bool _oldAutostart;
        bool _oldMinimizeToTray;
        bool _oldShowInTaskbar;
        bool _oldTabTextColorPerType;


    private slots:
        void onButtonClicked(QAbstractButton* button);

};
