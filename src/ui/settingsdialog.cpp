#include <QFileDialog>
#include <QFontDialog>
#include <QKeyEvent>
#include <QPushButton>
#include "settingsdialog.h"
#include "ui_settingsdialog.h"
#include "medo/config.h"
#include "helpers.h"
#include "settings.h"
#include "setup.h"

SettingsDialog::SettingsDialog(QWidget* parent, Hotkey* hotkey) : QDialog(parent), ui(new Ui::SettingsDialog) {
    ui->setupUi(this);
    Helpers::setupFixedSizeDialog(this);
    Helpers::setReadonlyPalette(ui->editDataPath);
    Helpers::setReadonlyPalette(ui->editFontName);
    Helpers::setReadonlyPalette(ui->editFontSize);
    Helpers::setReadonlyPalette(ui->editHotkey);

    ui->editHotkey->setHotkey(hotkey, Settings::hotkey());

    _oldAlwaysOnTop = Settings::alwaysOnTop();
    _oldAutostart = Setup::autostart();
    _oldDataPath = Settings::dataPath();
    _oldFollowUrlWithCtrl = Settings::followUrlWithCtrl();
    _oldFollowUrlWithDoubleClick = Settings::followUrlWithDoubleClick();
    _oldFontName = Settings::fontName();
    _oldFontSize = Settings::fontSize();
    _oldForceDarkMode = Settings::forceDarkMode();
    _oldForcePlainCopyPaste = Settings::forcePlainCopyPaste();
    _oldHotkey = Settings::hotkey();
    _oldHotkeyTogglesVisibility = Settings::hotkeyTogglesVisibility();
    _oldMinimizeToTray = Settings::minimizeToTray();
    _oldShowInTaskbar = Settings::showInTaskbar();
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
    _oldShowMarkdown = Settings::showMarkdown();
#else
    ui->checkboxShowMarkdown->setEnabled(false);
#endif
    _oldTabTextColorPerType = Settings::tabTextColorPerType();
    _oldUseHtmlByDefault = (Settings::defaultFileType() == FileType::Html); //ignoring markdown

    connect(ui->buttonBox, &QDialogButtonBox::clicked, this, &SettingsDialog::onButtonClicked);
    connect(ui->buttonDataPath, &QToolButton::clicked, this, &SettingsDialog::onDataPathClicked);
    connect(ui->buttonFont, &QToolButton::clicked, this, &SettingsDialog::onFontClicked);

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
    ui->editDataPath->setText(QDir::toNativeSeparators(Settings::dataPath()));
    ui->checkboxFollowUrlWithCtrl->setChecked(_oldFollowUrlWithCtrl);
    ui->checkboxFollowUrlWithDoubleClick->setChecked(_oldFollowUrlWithDoubleClick);
    ui->editFontName->setText(Settings::fontName());
    ui->editFontSize->setText(QString::number(Settings::fontSize()) + " pt");
    ui->checkboxForceDarkMode->setChecked(_oldForceDarkMode);
    ui->checkboxForcePlainCopyPaste->setChecked(_oldForcePlainCopyPaste);
    ui->editHotkey->setNewKey(_oldHotkey);
    ui->checkboxHotkeyTogglesVisibility->setChecked(_oldHotkeyTogglesVisibility);
    ui->checkboxMinimizeToTray->setChecked(_oldMinimizeToTray);
    ui->checkboxShowInTaskbar->setChecked(_oldShowInTaskbar);
    ui->checkboxShowMarkdown->setChecked(_oldShowMarkdown);
    ui->checkboxTabTextColorPerType->setChecked(_oldTabTextColorPerType);
    ui->checkboxUseHtmlByDefault->setChecked(_oldUseHtmlByDefault);
}

void SettingsDialog::restoreDefaults() {
    ui->checkboxAlwaysOnTop->setChecked(Settings::defaultAlwaysOnTop());
    ui->checkboxAutostart->setChecked(true);
    ui->editDataPath->setText(QDir::toNativeSeparators(Settings::defaultDataPath()));
    ui->checkboxFollowUrlWithCtrl->setChecked(Settings::defaultFollowUrlWithCtrl());
    ui->checkboxFollowUrlWithDoubleClick->setChecked(Settings::defaultFollowUrlWithDoubleClick());
    ui->editFontName->setText(Settings::defaultFontName());
    ui->editFontSize->setText(QString::number(Settings::defaultFontSize()) + " pt");
    ui->checkboxForceDarkMode->setChecked(Settings::defaultForceDarkMode());
    ui->checkboxForcePlainCopyPaste->setChecked(Settings::defaultForcePlainCopyPaste());
    ui->editHotkey->setNewKey(Settings::defaultHotkey());
    ui->checkboxHotkeyTogglesVisibility->setChecked(Settings::defaultHotkeyTogglesVisibility());
    ui->checkboxMinimizeToTray->setChecked(Settings::defaultMinimizeToTray());
    ui->checkboxShowInTaskbar->setChecked(Settings::defaultShowInTaskbar());
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
    ui->checkboxShowMarkdown->setChecked(Settings::defaultShowMarkdown());
#endif
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

    QString newDataDirectory = QDir::fromNativeSeparators(ui->editDataPath->text());
    if (newDataDirectory.isEmpty()) { newDataDirectory = _oldDataPath; } //if empty, just ignore the new one
    _changedDataPath = newDataDirectory != _oldDataPath;
    if (_changedDataPath) { Settings::setDataPath(newDataDirectory); }

    bool newFollowUrlWithCtrl = (ui->checkboxFollowUrlWithCtrl->checkState() == Qt::Checked);
    _changedFollowUrlWithCtrl = newFollowUrlWithCtrl != _oldFollowUrlWithCtrl;
    if (_changedFollowUrlWithCtrl) { Settings::setFollowUrlWithCtrl(newFollowUrlWithCtrl); }

    bool newFollowUrlWithDoubleClick = (ui->checkboxFollowUrlWithDoubleClick->checkState() == Qt::Checked);
    _changedFollowUrlWithDoubleClick = newFollowUrlWithDoubleClick != _oldFollowUrlWithDoubleClick;
    if (_changedFollowUrlWithDoubleClick) { Settings::setFollowUrlWithDoubleClick(newFollowUrlWithDoubleClick); }

    QString newFontName = ui->editFontName->text();
    int newFontSize = ui->editFontSize->text().split(" ")[0].toInt();
    _changedFont = (newFontName != _oldFontName) || (newFontSize != _oldFontSize);
    if (_changedFont) {
        Settings::setFontName(newFontName);
        Settings::setFontSize(newFontSize);
    }

    bool newForceDarkMode = (ui->checkboxForceDarkMode->checkState() == Qt::Checked);
    _changedForceDarkMode = newForceDarkMode != _oldForceDarkMode;
    if (_changedForceDarkMode) { Settings::setForceDarkMode(newForceDarkMode); }

    bool newForcePlainCopyPaste = (ui->checkboxForcePlainCopyPaste->checkState() == Qt::Checked);
    _changedForcePlainCopyPaste = newForcePlainCopyPaste != _oldForcePlainCopyPaste;
    if (_changedForcePlainCopyPaste) { Settings::setForcePlainCopyPaste(newForcePlainCopyPaste); }

    QKeySequence newHotkey = ui->editHotkey->newKey();
    _changedHotkey = (newHotkey != 0) && (newHotkey != _oldHotkey);
    if (_changedHotkey) { Settings::setHotkey(newHotkey); }

    bool newHotkeyTogglesVisibility = (ui->checkboxHotkeyTogglesVisibility->checkState() == Qt::Checked);
    _changedHotkeyTogglesVisibility = newHotkeyTogglesVisibility != _oldHotkeyTogglesVisibility;
    if (_changedHotkeyTogglesVisibility) { Settings::setHotkeyTogglesVisibility(newHotkeyTogglesVisibility); }

    bool newMinimizeToTray = (ui->checkboxMinimizeToTray->checkState() == Qt::Checked);
    _changedMinimizeToTray = newMinimizeToTray != _oldMinimizeToTray;
    if (_changedMinimizeToTray) { Settings::setMinimizeToTray(newMinimizeToTray); }

    bool newShowInTaskbar = (ui->checkboxShowInTaskbar->checkState() == Qt::Checked);
    _changedShowInTaskbar = newShowInTaskbar != _oldShowInTaskbar;
    if (_changedShowInTaskbar) { Settings::setShowInTaskbar(newShowInTaskbar); }

#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
    bool newShowMarkdown = (ui->checkboxShowMarkdown->checkState() == Qt::Checked);
    _changedShowMarkdown = newShowMarkdown != _oldShowMarkdown;
    if (_changedShowMarkdown) { Settings::setShowMarkdown(newShowMarkdown); }
#endif

    bool newTabTextColorPerType = (ui->checkboxTabTextColorPerType->checkState() == Qt::Checked);
    _changedTabTextColorPerType = newTabTextColorPerType != _oldTabTextColorPerType;
    if (_changedTabTextColorPerType) { Settings::setTabTextColorPerType(newTabTextColorPerType); }

    bool newUseHtmlByDefault = (ui->checkboxUseHtmlByDefault->checkState() == Qt::Checked);
    _changedUseHtmlByDefault = newUseHtmlByDefault != _oldUseHtmlByDefault;
    if (_changedUseHtmlByDefault) { Settings::setDefaultFileType(newUseHtmlByDefault ? FileType::Html : FileType::Plain); }

    QDialog::accept();
}


void SettingsDialog::onDataPathClicked() {
    QString newDataPath = QFileDialog::getExistingDirectory(this, "Select data directory", _oldDataPath, QFileDialog::ShowDirsOnly);
    if (!newDataPath.isNull()) {
        ui->editDataPath->setText(QDir::toNativeSeparators(newDataPath));
    }
}

void SettingsDialog::onFontClicked() {
    QFont currFont = QFont(Settings::fontName(), Settings::fontSize());
    bool ok;
    QFont newFont = QFontDialog::getFont(&ok, currFont, this);
    if (ok) {
        ui->editFontName->setText(newFont.family());
        ui->editFontSize->setText(QString::number(newFont.pointSize()) + " pt");
    }
}
