#include <QPushButton>
#include "setup.h"
#include "settingsdialog.h"
#include "ui_settingsdialog.h"

SettingsDialog::SettingsDialog(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::SettingsDialog) {
    ui->setupUi(this);

    _oldAutostart = Setup::autostart();

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
    ui->checkboxAutostart->setChecked(_oldAutostart);
}

void SettingsDialog::restoreDefaults() {
    ui->checkboxAutostart->setChecked(true);
}

void SettingsDialog::accept() {
    bool newAutostart = (ui->checkboxAutostart->checkState() == Qt::Checked);
    if (newAutostart != _oldAutostart) {
        Setup::setAutostart(newAutostart);
    }

    QDialog::accept();
}
