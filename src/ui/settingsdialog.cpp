#include <QPushButton>
#include "settings.h"
#include "setup.h"
#include "settingsdialog.h"
#include "ui_settingsdialog.h"

SettingsDialog::SettingsDialog(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::SettingsDialog) {
    ui->setupUi(this);

    _oldShowInTaskbar = Settings::showInTaskbar();
    _oldAutostart = Setup::autostart();
    _oldMinimizeToTray = Settings::minimizeToTray();

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
    ui->checkboxShowInTaskbar->setChecked(_oldShowInTaskbar);
    ui->checkboxAutostart->setChecked(_oldAutostart);
    ui->checkboxMinimizeToTray->setChecked(_oldMinimizeToTray);
}

void SettingsDialog::restoreDefaults() {
    ui->checkboxShowInTaskbar->setChecked(Settings::defaultShowInTaskbar());
    ui->checkboxAutostart->setChecked(true);
    ui->checkboxMinimizeToTray->setChecked(Settings::defaultMinimizeToTray());
}

void SettingsDialog::accept() {
    bool newShowInTaskbar = (ui->checkboxShowInTaskbar->checkState() == Qt::Checked);
    changedShowInTaskbar = newShowInTaskbar != _oldShowInTaskbar;
    if (changedShowInTaskbar) { Settings::setShowInTaskbar(newShowInTaskbar); }

    bool newAutostart = (ui->checkboxAutostart->checkState() == Qt::Checked);
    changedAutostart = newAutostart != _oldAutostart;
    if (changedAutostart) { Setup::setAutostart(newAutostart); }

    bool newMinimizeToTray = (ui->checkboxMinimizeToTray->checkState() == Qt::Checked);
    changedMinimizeToTray = newMinimizeToTray != _oldMinimizeToTray;
    if (changedMinimizeToTray) { Settings::setMinimizeToTray(newMinimizeToTray); }

    QDialog::accept();
}
