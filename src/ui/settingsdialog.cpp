#include <QPushButton>
#include "settings.h"
#include "setup.h"
#include "settingsdialog.h"
#include "ui_settingsdialog.h"

SettingsDialog::SettingsDialog(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::SettingsDialog) {
    ui->setupUi(this);
    this->setFixedSize(this->geometry().width(), this->geometry().height());

    _oldAlwaysOnTop = Settings::alwaysOnTop();
    _oldAutostart = Setup::autostart();
    _oldMinimizeToTray = Settings::minimizeToTray();
    _oldShowInTaskbar = Settings::showInTaskbar();

    connect(ui->buttonBox, SIGNAL(clicked(QAbstractButton*)), this,  SLOT(onButtonClicked(QAbstractButton*)));
    reset();
}

SettingsDialog::~SettingsDialog() {
    delete ui;
}

void SettingsDialog::onButtonClicked(QAbstractButton* button) {
    if (button == ui->buttonBox->button(QDialogButtonBox::Reset)) {
        reset();
    } else if (button == ui->buttonBox->button(QDialogButtonBox::RestoreDefaults)) {
        restoreDefaults();
    }
}

void SettingsDialog::reset() {
    ui->checkboxAlwaysOnTop->setChecked(_oldAlwaysOnTop);
    ui->checkboxAutostart->setChecked(_oldAutostart);
    ui->checkboxMinimizeToTray->setChecked(_oldMinimizeToTray);
    ui->checkboxShowInTaskbar->setChecked(_oldShowInTaskbar);
}

void SettingsDialog::restoreDefaults() {
    ui->checkboxAlwaysOnTop->setChecked(Settings::defaultAlwaysOnTop());
    ui->checkboxAutostart->setChecked(true);
    ui->checkboxMinimizeToTray->setChecked(Settings::defaultMinimizeToTray());
    ui->checkboxShowInTaskbar->setChecked(Settings::defaultShowInTaskbar());
}

void SettingsDialog::accept() {
    bool newAlwaysOnTop = (ui->checkboxAlwaysOnTop->checkState() == Qt::Checked);
    changedAlwaysOnTop = newAlwaysOnTop != _oldAlwaysOnTop;
    if (changedAlwaysOnTop) { Settings::setAlwaysOnTop(newAlwaysOnTop); }

    bool newAutostart = (ui->checkboxAutostart->checkState() == Qt::Checked);
    changedAutostart = newAutostart != _oldAutostart;
    if (changedAutostart) { Setup::setAutostart(newAutostart); }

    bool newMinimizeToTray = (ui->checkboxMinimizeToTray->checkState() == Qt::Checked);
    changedMinimizeToTray = newMinimizeToTray != _oldMinimizeToTray;
    if (changedMinimizeToTray) { Settings::setMinimizeToTray(newMinimizeToTray); }

    bool newShowInTaskbar = (ui->checkboxShowInTaskbar->checkState() == Qt::Checked);
    changedShowInTaskbar = newShowInTaskbar != _oldShowInTaskbar;
    if (changedShowInTaskbar) { Settings::setShowInTaskbar(newShowInTaskbar); }

    QDialog::accept();
}
