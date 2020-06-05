#include <QKeyEvent>
#include <QPushButton>
#include "settingsdialog.h"
#include "ui_settingsdialog.h"
#include "medo/config.h"
#include "helpers.h"
#include "settings.h"
#include "setup.h"

SettingsDialog::SettingsDialog(QWidget* parent) : QDialog(parent), ui(new Ui::SettingsDialog) {
    ui->setupUi(this);
    Helpers::setupFixedSizeDialog(this);

    _oldAlwaysOnTop = Settings::alwaysOnTop();
    _oldAutostart = Setup::autostart();
    _oldMinimizeToTray = Settings::minimizeToTray();
    _oldShowInTaskbar = Settings::showInTaskbar();
    _oldShowMarkdown = Settings::showMarkdown();
    _oldTabTextColorPerType = Settings::tabTextColorPerType();
    _oldUseHtmlByDefault = (Settings::defaultFileType() == FileType::Html); //ignoring markdown

    connect(ui->buttonBox, &QDialogButtonBox::clicked, this,  &SettingsDialog::onButtonClicked);
    reset();
}

SettingsDialog::~SettingsDialog() {
    delete ui;
}


void SettingsDialog::keyPressEvent(QKeyEvent* event) {
    auto keyData = static_cast<uint>(event->key()) | event->modifiers();
    switch (keyData) {
        case Qt::Key_Escape:
            close();
            break;

        case Qt::Key_F8:
            Helpers::showInFileManager(QString(), Config::configurationFile());
            break;

        case Qt::ShiftModifier | Qt::Key_F8:
            if (Helpers::openWithVSCodeAvailable()) {
                Helpers::openFileWithVSCode(Config::configurationFile());
            } else {
                Helpers::openWithDefaultApplication(Config::configurationFile());
            }
            break;
    }
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
    ui->checkboxShowMarkdown->setChecked(_oldShowMarkdown);
    ui->checkboxTabTextColorPerType->setChecked(_oldTabTextColorPerType);
    ui->checkboxUseHtmlByDefault->setChecked(_oldUseHtmlByDefault);
}

void SettingsDialog::restoreDefaults() {
    ui->checkboxAlwaysOnTop->setChecked(Settings::defaultAlwaysOnTop());
    ui->checkboxAutostart->setChecked(true);
    ui->checkboxMinimizeToTray->setChecked(Settings::defaultMinimizeToTray());
    ui->checkboxShowInTaskbar->setChecked(Settings::defaultShowInTaskbar());
    ui->checkboxShowMarkdown->setChecked(Settings::defaultShowMarkdown());
    ui->checkboxTabTextColorPerType->setChecked(Settings::defaultTabTextColorPerType());
    ui->checkboxUseHtmlByDefault->setChecked(Settings::defaultFileType() == FileType::Html);
}

void SettingsDialog::accept() {
    bool newAlwaysOnTop = (ui->checkboxAlwaysOnTop->checkState() == Qt::Checked);
    _changedAlwaysOnTop = newAlwaysOnTop != _oldAlwaysOnTop;
    if (_changedAlwaysOnTop) { Settings::setAlwaysOnTop(newAlwaysOnTop); }

    bool newAutostart = (ui->checkboxAutostart->checkState() == Qt::Checked);
    _changedAutostart = newAutostart != _oldAutostart;
    if (_changedAutostart) { Setup::setAutostart(newAutostart); }

    bool newMinimizeToTray = (ui->checkboxMinimizeToTray->checkState() == Qt::Checked);
    _changedMinimizeToTray = newMinimizeToTray != _oldMinimizeToTray;
    if (_changedMinimizeToTray) { Settings::setMinimizeToTray(newMinimizeToTray); }

    bool newShowInTaskbar = (ui->checkboxShowInTaskbar->checkState() == Qt::Checked);
    _changedShowInTaskbar = newShowInTaskbar != _oldShowInTaskbar;
    if (_changedShowInTaskbar) { Settings::setShowInTaskbar(newShowInTaskbar); }

    bool newShowMarkdown = (ui->checkboxShowMarkdown->checkState() == Qt::Checked);
    _changedShowMarkdown = newShowMarkdown != _oldShowMarkdown;
    if (_changedShowMarkdown) { Settings::setShowMarkdown(newShowMarkdown); }

    bool newTabTextColorPerType = (ui->checkboxTabTextColorPerType->checkState() == Qt::Checked);
    _changedTabTextColorPerType = newTabTextColorPerType != _oldTabTextColorPerType;
    if (_changedTabTextColorPerType) { Settings::setTabTextColorPerType(newTabTextColorPerType); }

    bool newUseHtmlByDefault = (ui->checkboxUseHtmlByDefault->checkState() == Qt::Checked);
    _changedUseHtmlByDefault = newUseHtmlByDefault != _oldUseHtmlByDefault;
    if (_changedUseHtmlByDefault) { Settings::setDefaultFileType(newUseHtmlByDefault ? FileType::Html : FileType::Plain); }

    QDialog::accept();
}
