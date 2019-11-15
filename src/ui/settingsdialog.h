#ifndef SETTINGSDIALOG_H
#define SETTINGSDIALOG_H

#include <QAbstractButton>
#include <QDialog>

namespace Ui {
    class SettingsDialog;
}

class SettingsDialog : public QDialog {
    Q_OBJECT

    public:
        explicit SettingsDialog(QWidget *parent = nullptr);
        ~SettingsDialog();
        bool changedAlwaysOnTop;
        bool changedAutostart;
        bool changedMinimizeToTray;
        bool changedShowInTaskbar;

    protected:
        void accept();

    private:
        Ui::SettingsDialog *ui;
        void reset();
        void restoreDefaults();
        bool _oldAlwaysOnTop;
        bool _oldAutostart;
        bool _oldMinimizeToTray;
        bool _oldShowInTaskbar;


    private slots:
        void onButtonClicked(QAbstractButton* button);

};

#endif // SETTINGSDIALOG_H
