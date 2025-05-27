#include <QDir>
#include <QFileDialog>
#include <QMenu>
#include <QMessageBox>
#include <QTabBar>
#include <QtConcurrent/QtConcurrent>
#include <QtPrintSupport/QAbstractPrintDialog>
#include <QtPrintSupport/QPageSetupDialog>
#include <QtPrintSupport/QPrintDialog>
#include <QtPrintSupport/QPrinter>
#include <QtPrintSupport/QPrintPreviewDialog>

#include "clipboard.h"
#include "find.h"
#include "helpers.h"
#include "icons.h"
#include "qtabbarex.h"
#include "settings.h"
#include "storage/storage.h"

#include "medo/about.h"
#include "medo/config.h"
#include "medo/feedback.h"
#include "medo/lifetimewatch.h"
#include "medo/singleinstance.h"
#include "medo/state.h"
#include "medo/upgrade.h"

#include "finddialog.h"
#include "foldersdialog.h"
#include "gotodialog.h"
#include "newfiledialog.h"
#include "phoneticdialog.h"
#include "renamefiledialog.h"
#include "settingsdialog.h"

#include "mainwindow.h"
#include "ui_mainwindow.h"


MainWindow::MainWindow(Storage* storage) : QMainWindow(nullptr), ui(new Ui::MainWindow) {
    ui->setupUi(this);
    //ui->tabWidget->tabBar()->setStyleSheet("QTabBar::tab:!selected { background: #10000000; }");

    _storage = storage;

    //application icon
    this->setWindowIcon(Icons::app());

    //tray
    _tray = new QSystemTrayIcon(this);
    connect(_tray, &QSystemTrayIcon::activated, this, &MainWindow::onTrayActivate);

    QMenu* trayMenu = new QMenu(this);
    auto defaultAction = trayMenu->addAction("&Show", this, &MainWindow::onTrayShow);
    trayMenu->addSeparator();
    trayMenu->addAction("&Quit", this, &MainWindow::onAppQuit);

    auto font = defaultAction->font();
    font.setBold(true);
    defaultAction->setFont(font);
    _tray->setContextMenu(trayMenu);

    if (Settings::colorTrayIcon()) {
        _tray->setIcon(Icons::app());
    } else {
        bool isWindows = (QSysInfo::kernelType() == "winnt");
        bool isWindows10 = isWindows && (QSysInfo::kernelVersion().startsWith("10.0.1"));
        bool isWindows11 = isWindows && (QSysInfo::kernelVersion().startsWith("10.0.2")); // 10.0.22000 is the first

        if (isWindows10) {
            _tray->setIcon(Icons::appWhite());
        } else if (isWindows11) {
            _tray->setIcon(Icons::appBlack());
        } else {
            _tray->setIcon(Icons::app());
        }
    }

    auto hotkeyText = Settings::hotkey().toString(QKeySequence::PortableText);
    if (hotkeyText.length() > 0) {
        _tray->setToolTip( + "Access notes from tray or press " + hotkeyText + " hotkey.");
    } else {
        _tray->setToolTip( + "Access notes from tray.");
    }
    _tray->show();

    //hotkey
    if (QString::fromUtf8(getenv("XDG_CURRENT_DESKTOP")).compare("KDE", Qt::CaseSensitive) != 0) {
        _hotkey = new Hotkey(this);
        _hotkey->registerHotkey(Settings::hotkey());
        connect(_hotkey, &Hotkey::activated, this, &MainWindow::onHotkeyPress);

#if defined(Q_OS_LINUX)
        if (Settings::hotkeyUseDConf()) {
            _dconfHotkey = new DConfHotkey("QText", this);
            if (_dconfHotkey->hasRegisteredHotkey() == false) {  // register if not already in settings
                _dconfHotkey->registerHotkey(Settings::hotkey());
            }
        }
#endif
    }

    //single instance
    connect(SingleInstance::instance(), &SingleInstance::newInstanceDetected, this, &MainWindow::onTrayShow);

    //toolbar setup
    connect(ui->actionNew, &QAction::triggered, this, &MainWindow::onFileNew);
    connect(ui->actionSave, &QAction::triggered, this, &MainWindow::onFileSave);
    connect(ui->actionRename, &QAction::triggered, this, &MainWindow::onFileRename);
    connect(ui->actionDelete, &QAction::triggered, this, &MainWindow::onFileDelete);
    connect(ui->actionPrint, &QAction::triggered, this, &MainWindow::onFilePrint);
    connect(ui->actionPrintPreview, &QAction::triggered, this, &MainWindow::onFilePrintPreview);
    connect(ui->actionPrintToPdf, &QAction::triggered, this, &MainWindow::onFilePrintToPdf);
    connect(ui->actionCut, &QAction::triggered, this, &MainWindow::onTextCut);
    connect(ui->actionCopy, &QAction::triggered, this, &MainWindow::onTextCopy);
    connect(ui->actionPaste, &QAction::triggered, this, &MainWindow::onTextPaste);
    connect(ui->actionUndo, &QAction::triggered, this, &MainWindow::onTextUndo);
    connect(ui->actionRedo, &QAction::triggered, this, &MainWindow::onTextRedo);
    connect(ui->actionFontBold, &QAction::triggered, this, &MainWindow::onTextFontBold);
    connect(ui->actionFontItalic, &QAction::triggered, this, &MainWindow::onTextFontItalic);
    connect(ui->actionFontUnderline, &QAction::triggered, this, &MainWindow::onTextFontUnderline);
    connect(ui->actionFontStrikethrough, &QAction::triggered, this, &MainWindow::onTextFontStrikethrough);
    connect(ui->actionFind, &QAction::triggered, this, &MainWindow::onFind);
    connect(ui->actionFindNext, &QAction::triggered, this, &MainWindow::onFindNext);
    connect(ui->actionGoto, &QAction::triggered, this, &MainWindow::onGoto);

    //print button menu
    _printButton = new QToolButton();
    _printButton->setPopupMode(QToolButton::MenuButtonPopup);
    _printButton->setMenu(new QMenu());
    connect(_printButton, &QToolButton::clicked, this, &MainWindow::onFilePrint);

    _printButton->menu()->addAction(ui->actionPrint);
    _printButton->menu()->addAction(ui->actionPrintPreview);
    _printButton->menu()->addAction(ui->actionPrintToPdf);
    ui->mainToolBar->insertWidget(ui->mainToolBar->actions()[3], _printButton);

    //align-right
    QWidget* spacerWidget = new QWidget(this);
    spacerWidget->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);
    spacerWidget->setVisible(true);
    ui->mainToolBar->addWidget(spacerWidget)->setPriority(QAction::HighPriority);

    //folder button menu
    _folderButton = new QToolButton();
    _folderButton->setPopupMode(QToolButton::MenuButtonPopup);
    _folderButton->setMenu(new QMenu());
    ui->mainToolBar->addWidget(_folderButton)->setPriority(QAction::HighPriority);
    connect(_folderButton, &QToolButton::clicked, this, &MainWindow::onFolderSetup);
    connect(_folderButton->menu(), &QMenu::aboutToShow, this, &MainWindow::onFolderMenuShow);

    onFolderMenuSelect(); //setup folder select menu button
    onTabChanged(); //update toolbar & focus
    connect(ui->actionReopen, &QAction::triggered, this, &MainWindow::onFileReopen);
    connect(ui->actionOpenWithDefaultApplication,  &QAction::triggered, this, &MainWindow::onOpenWithDefaultApplication);
    connect(ui->actionOpenWithVisualStudioCode,  &QAction::triggered, this, &MainWindow::onOpenWithVisualStudioCode);
    connect(ui->actionShowContainingDirectory,  &QAction::triggered, this, &MainWindow::onShowContainingDirectory);
    connect(ui->actionShowContainingDirectoryOnly,  &QAction::triggered, this, &MainWindow::onShowContainingDirectoryOnly);
    connect(ui->actionCopyContainingPath,  &QAction::triggered, this, &MainWindow::onCopyContainingPath);

    //app button menu
    _appButton = new QToolButton();
    _appButton->setPopupMode(QToolButton::MenuButtonPopup);
    _appButton->setMenu(new QMenu());
    connect(_appButton, &QToolButton::clicked, this, &MainWindow::onAppSettings);

    QAction* appSettingsAction = new QAction("&Settings");
    connect(appSettingsAction, &QAction::triggered, this, &MainWindow::onAppSettings);
    _appButton->menu()->addAction(appSettingsAction);

    _appButton->menu()->addSeparator();

    QAction* appFeedbackAction = new QAction("Send &Feedback");
    connect(appFeedbackAction, &QAction::triggered, this, &MainWindow::onAppFeedback);
    _appButton->menu()->addAction(appFeedbackAction);

#if defined(Q_OS_WIN) //upgrade visible only on Windows
    QAction* appUpgradeAction = new QAction("Check for &Upgrade");
    connect(appUpgradeAction, &QAction::triggered, this, &MainWindow::onAppUpgrade);
    _appButton->menu()->addAction(appUpgradeAction);
#endif

    QAction* appAboutAction = new QAction("&About");
    connect(appAboutAction, &QAction::triggered, this, &MainWindow::onAppAbout);
    _appButton->menu()->addAction(appAboutAction);

    _appButton->menu()->addSeparator();

    QAction* appQuitAction = new QAction("&Quit");
    connect(appQuitAction, &QAction::triggered, this, &MainWindow::onAppQuit);
    _appButton->menu()->addAction(appQuitAction);

    ui->mainToolBar->addWidget(_appButton)->setPriority(QAction::HighPriority);

    //toolbar icon setup
    applyToolbarIcons();

    //tabs
    ui->tabWidget->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(ui->tabWidget, &QTabWidget::customContextMenuRequested, this, &MainWindow::onTabMenuRequested);
    ui->tabWidget->tabBar()->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(ui->tabWidget->tabBar(), &QTabBar::customContextMenuRequested, this, &MainWindow::onTabMenuRequested);
    connect(ui->tabWidget, &QTabWidget::currentChanged, this, &MainWindow::onTabChanged);
    connect(ui->tabWidget, &QTabWidgetEx::tabMoved, this, &MainWindow::onTabMoved);

    //clipboard
    connect(QApplication::clipboard(), &QClipboard::dataChanged, this, &MainWindow::onTextStateChanged);

    //State
    State::load(this);

    //show shortcuts in context menu
    for (QAction* action : findChildren<QAction*>()) {
        action->setShortcutVisibleInContextMenu(true);
    }

    //show shortcut in tooltip
    for (QAction* action : ui->mainToolBar->actions()) {
        auto shortcut = action->shortcut();
        if (!shortcut.isEmpty()) {
            action->setToolTip(action->toolTip() + "\n(" + shortcut.toString() + ")");
        }
    }

    //settings
    applySettings();

    if (!Settings::setupCompleted()) { // show extra info when ran first time
        auto duration = Config::isPortable() ? 1000 : 2500; //shorter duration if config is portable
        _tray->showMessage(QCoreApplication::applicationName(), _tray->toolTip(), QSystemTrayIcon::Information, duration);
    }

    //determine last used folder
    if (!selectFolder(Settings::lastFolder())) {
        selectFolder(storage->baseFolder()->name()); //default folder
    }

    //storage updates
    connect(_storage, &Storage::updatedFolder, this, &MainWindow::onUpdatedFolder);

    //upgrade
    _futureWatcher = new QFutureWatcher<bool>();
    connect(_futureWatcher, &QFutureWatcher<bool>::finished, this, &MainWindow::onAppUpgradeChecked);
    QFuture<bool> future = QtConcurrent::run([]() { return Upgrade::available("https://medo64.com/upgrade/"); });
    _futureWatcher->setFuture(future);
}

MainWindow::~MainWindow() {
    delete ui;
}


void MainWindow::changeEvent(QEvent* event) {
    if (event->type() == QEvent::WindowStateChange) {
        if (isMinimized() && Settings::minimizeToTray()) {
            this->hide();
        }
    }
    return QMainWindow::changeEvent(event);
}

void MainWindow::closeEvent(QCloseEvent* event) {
    State::save(this);
    if (event->spontaneous()) {
        if (event->type() == QEvent::Close) {
            event->ignore();
            this->hide();
#ifdef QT_DEBUG //close immediately for easier debugging
            QCoreApplication::exit(0);
#endif
        }
    }
}

void MainWindow::keyPressEvent(QKeyEvent* event) {
    auto keyData = static_cast<uint>(event->key()) | event->modifiers();
    switch (keyData) {
        case Qt::Key_Escape:
        case Qt::ControlModifier | Qt::Key_F4:
        case Qt::ControlModifier | Qt::Key_W:
            State::save(this);
            this->hide();
#ifdef QT_DEBUG //close immediately for easier debugging
            QCoreApplication::exit(0);
#endif
            break;

        case Qt::Key_F1:
            _appButton->showMenu();
            break;

        case Qt::ShiftModifier | Qt::Key_F1:
            onPhoneticSpelling();
            break;

        case Qt::ControlModifier | Qt::Key_F1:
            _appButton->click();
            break;

        case Qt::ShiftModifier | Qt::Key_F3:
            onFindNext(true);
            break;

        case Qt::Key_F4:
        case Qt::ControlModifier | Qt::Key_O:
            _folderButton->showMenu();
            break;

        case Qt::ShiftModifier | Qt::Key_F4:
        case Qt::ControlModifier | Qt::ShiftModifier  | Qt::Key_O:
            _folderButton->click();
            break;

        case Qt::AltModifier | Qt::Key_F8:
            if (Helpers::openWithVSCodeAvailable()) {
                if (ui->tabWidget->count() > 0) {
                    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
                    Helpers::openDirectoryWithVSCode(file);
                } else {
                    Helpers::openDirectoryWithVSCode(_folder);
                }
            }
            break;

        case Qt::ControlModifier | Qt::AltModifier | Qt::Key_C:
            onCopyContainingPath();
            break;

        case Qt::ControlModifier | Qt::Key_T:
            onTopMost();
            break;

        case Qt::AltModifier | Qt::Key_1:
            if (ui->tabWidget->count() >= 1) { ui->tabWidget->setCurrentIndex(0); }
            break;

        case Qt::AltModifier | Qt::Key_2:
            if (ui->tabWidget->count() >= 2) { ui->tabWidget->setCurrentIndex(1); }
            break;

        case Qt::AltModifier | Qt::Key_3:
            if (ui->tabWidget->count() >= 3) { ui->tabWidget->setCurrentIndex(2); }
            break;

        case Qt::AltModifier | Qt::Key_4:
            if (ui->tabWidget->count() >= 4) { ui->tabWidget->setCurrentIndex(3); }
            break;

        case Qt::AltModifier | Qt::Key_5:
            if (ui->tabWidget->count() >= 5) { ui->tabWidget->setCurrentIndex(4); }
            break;

        case Qt::AltModifier | Qt::Key_6:
            if (ui->tabWidget->count() >= 6) { ui->tabWidget->setCurrentIndex(5); }
            break;

        case Qt::AltModifier | Qt::Key_7:
            if (ui->tabWidget->count() >= 7) { ui->tabWidget->setCurrentIndex(6); }
            break;

        case Qt::AltModifier | Qt::Key_8:
            if (ui->tabWidget->count() >= 8) { ui->tabWidget->setCurrentIndex(7); }
            break;

        case Qt::AltModifier | Qt::Key_9:
            if (ui->tabWidget->count() >= 9) { ui->tabWidget->setCurrentIndex(8); }
            break;

        case Qt::AltModifier | Qt::Key_0:
            if (ui->tabWidget->count() >= 10) { ui->tabWidget->setCurrentIndex(9); }
            break;

        case Qt::AltModifier | Qt::Key_Comma:
            onFileRename();
            break;

        case Qt::AltModifier | Qt::Key_Left:
            if (ui->tabWidget->currentIndex() > 0) { ui->tabWidget->setCurrentIndex(ui->tabWidget->currentIndex() - 1); }
            break;

        case Qt::AltModifier | Qt::Key_Right:
            if (ui->tabWidget->currentIndex() < ui->tabWidget->count() - 1) { ui->tabWidget->setCurrentIndex(ui->tabWidget->currentIndex() + 1); }
            break;

        case Qt::AltModifier | Qt::Key_Up:
        case Qt::AltModifier | Qt::Key_PageUp:
            for (int i = 1; i < _storage->folderCount(); i++) {
                auto folder = _storage->folderAt(i);
                if (folder->name().compare(_folder->name(), Qt::CaseSensitive) == 0) {
                    auto newFolder = _storage->folderAt(i - 1);
                    selectFolder(newFolder->name());
                    break;
                }
            }
            break;

        case Qt::AltModifier | Qt::Key_Down:
        case Qt::AltModifier | Qt::Key_PageDown:
            for (int i = 0; i < _storage->folderCount() - 1; i++) {
                auto folder = _storage->folderAt(i);
                if (folder->name().compare(_folder->name(), Qt::CaseSensitive) == 0) {
                    auto newFolder = _storage->folderAt(i + 1);
                    selectFolder(newFolder->name());
                    break;
                }
            }
            break;

        case Qt::AltModifier | Qt::Key_Home:
            selectFolder(_storage->baseFolder()->name());
            break;

        case Qt::AltModifier | Qt::Key_End:
            _folderButton->showMenu();
            break;

        case Qt::ShiftModifier | Qt::AltModifier | Qt::Key_Left:
            if (ui->tabWidget->currentIndex() > 0) {
                QTabBarEx* tabbar = dynamic_cast<QTabBarEx*>(ui->tabWidget->tabBar());
                tabbar->moveTab(ui->tabWidget->currentIndex(), ui->tabWidget->currentIndex() - 1);
            }
            break;

        case Qt::ShiftModifier | Qt::AltModifier | Qt::Key_Right:
            if (ui->tabWidget->currentIndex() < ui->tabWidget->count() - 1) {
                QTabBarEx* tabbar = dynamic_cast<QTabBarEx*>(ui->tabWidget->tabBar());
                tabbar->moveTab(ui->tabWidget->currentIndex(), ui->tabWidget->currentIndex() + 1);
            }
            break;

        case Qt::ShiftModifier | Qt::Key_Menu:
            if (ui->tabWidget->currentIndex() > -1) {
                QTabBarEx* tabbar = dynamic_cast<QTabBarEx*>(ui->tabWidget->tabBar());
                auto rect = tabbar->tabRect(ui->tabWidget->currentIndex());
                auto offset = QApplication::startDragDistance() / 2;
                auto point = rect.bottomLeft() + QPoint(offset, -offset);
                onTabMenuRequested(point);
            }
            break;

        default: QMainWindow::keyPressEvent(event);
    }
}

void MainWindow::moveEvent(QMoveEvent* event) {
    QMainWindow::moveEvent(event);
    if (_moveTimer == nullptr) { //setup state save a second later; don't create if timer already exists
        _moveTimer = new QTimer();
        _moveTimer->setSingleShot(true);
        _moveTimer->start(1000);
        QObject::connect(_moveTimer, &QTimer::timeout, [&]() { //triggered only once per setup
            delete _moveTimer; //release timer
            _moveTimer = nullptr;
            if (this->isVisible()) { State::save(this); }
        });
    }
}

void MainWindow::resizeEvent(QResizeEvent* event) {
    QMainWindow::resizeEvent(event);
    if (this->isVisible()) { State::save(this); }
}

void MainWindow::onFileModificationChanged(FileItem* file, bool isModified) {
    auto tabIndex = ui->tabWidget->indexOf(file);
    if (isModified) {
        ui->tabWidget->setTabText(tabIndex, file->title() + "*");
        ui->actionSave->setDisabled(false);
    } else {
        ui->tabWidget->setTabText(tabIndex, file->title());
        ui->actionSave->setDisabled(true);
    }
}

void MainWindow::onFileTitleChanged(FileItem* file) {
    onFileModificationChanged(file, file->isModified());
}

void MainWindow::onFileActivated(FileItem* file) {
    onFileModificationChanged(file, file->isModified());
}


void MainWindow::onFileNew() {
    auto currentFile = (ui->tabWidget->currentWidget() != nullptr) ? dynamic_cast<FileItem*>(ui->tabWidget->currentWidget()) : nullptr;
    auto dialog = new NewFileDialog(this, _folder);
    if (dialog->exec() == QDialog::Accepted) {
        auto newTitle = dialog->title();
        auto file = _folder->newFile(newTitle, dialog->type(), currentFile);
        selectFolder(_folder); //to refresh folder
        selectFile(file);
    }
}

void MainWindow::onFileReopen() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->load();
    file->setFocus();
}

void MainWindow::onFileSave() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->save();
    file->setFocus();
}

void MainWindow::onFileRename() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    auto tabIndex = ui->tabWidget->currentIndex();

    auto dialog = new RenameFileDialog(this, file);
    if (dialog->exec() == QDialog::Accepted) {
        auto newTitle = dialog->title();
        file->setTitle(newTitle);
        ui->tabWidget->setTabText(tabIndex, newTitle);
        file->setFocus();
    }
}

void MainWindow::onFileDelete() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    QMessageBox msgBox;
    msgBox.setIcon(QMessageBox::Question);
    msgBox.setText("The document has been modified");
    msgBox.setInformativeText("Do you really want to delete " + file->title() + "?");
    msgBox.setStandardButtons(QMessageBox::Yes | QMessageBox::No);
    msgBox.setDefaultButton(QMessageBox::Yes);
    if (file->isEmpty() || (msgBox.exec() == QMessageBox::Yes)) {
        file->deleteLater();
        _folder->deleteFile(file, Settings::deletionSyle());
    }
}

void MainWindow::onFilePrint() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    QPrinter printer(QPrinter::PrinterResolution);

    QPrintDialog dialog(&printer, this);
    Helpers::setupResizableDialog(&dialog);
    dialog.setWindowTitle("Print: " + file->title());
    if (file->textCursor().hasSelection()) {
        dialog.addEnabledOption(QAbstractPrintDialog::PrintSelection);
        dialog.setPrintRange(QAbstractPrintDialog::Selection); //make selection default
    }

    if (dialog.exec() == QDialog::Accepted) {
        QApplication::setOverrideCursor(Qt::WaitCursor);
        file->document()->print(&printer);
        QApplication::restoreOverrideCursor();
    }
}

void MainWindow::onFilePrintPreview() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    QApplication::setOverrideCursor(Qt::WaitCursor);

    QPrinter printer(QPrinter::ScreenResolution);

    QPrintPreviewDialog dialog(&printer, this, Qt::Dialog | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
    Helpers::setupResizableDialog(&dialog);
    dialog.setWindowTitle("Print Preview: " + file->title());
    connect(&dialog, &QPrintPreviewDialog::paintRequested, file, &FileItem::printPreview);
    dialog.showMaximized();

    QApplication::restoreOverrideCursor();

    dialog.exec();
}

void MainWindow::onFilePrintToPdf() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    QFileDialog dialog(this);
    dialog.setFileMode(QFileDialog::AnyFile);
    dialog.setDefaultSuffix(".pdf");
    dialog.setNameFilter("Documents (*.pdf)");
    dialog.setAcceptMode(QFileDialog::AcceptSave);
    if (dialog.exec()) {
        auto fileNames = dialog.selectedFiles();
        if (fileNames.length() > 0) {
            auto fileName = fileNames[0];
            QPrinter printer(QPrinter::PrinterResolution);
            printer.setOutputFileName(fileName);

            QApplication::setOverrideCursor(Qt::WaitCursor);
            file->print(&printer);
            QApplication::restoreOverrideCursor();
        }
    }
}

void MainWindow::onTextCut() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->textCut();
}

void MainWindow::onTextCopy() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->textCopy();
}

void MainWindow::onTextPaste() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->textPaste();
}

void MainWindow::onTextUndo() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->textUndo();
}

void MainWindow::onTextRedo() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->textRedo();
}

void MainWindow::onTextFontBold() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->setFontBold(ui->actionFontBold->isChecked());
}

void MainWindow::onTextFontItalic() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->setFontItalic(ui->actionFontItalic->isChecked());
}

void MainWindow::onTextFontUnderline() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->setFontUnderline(ui->actionFontUnderline->isChecked());
}

void MainWindow::onTextFontStrikethrough() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->setFontStrikethrough(ui->actionFontStrikethrough->isChecked());
}

void MainWindow::onFind() {
    auto dialog = new FindDialog(this);
    if (dialog->exec() == QDialog::Accepted) {
        Find::setup(_storage, dialog->searchText(), dialog->matchCase(), dialog->wholeWord(), dialog->useRegEx(), dialog->searchScope());
        onFindNext();
    }
}

void MainWindow::onFindNext(bool backward) {
    if (Find::hasText()) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
        auto nextFile = Find::findNext(file, backward);
        if ((nextFile != nullptr) && (nextFile != file)) {
            FolderItem* folder = file->folder();
            FolderItem* nextFolder = nextFile->folder();
            if (folder != nextFolder) { selectFolder(nextFolder); }
            selectFile(nextFile->name());
        }
    } else {
        onFind(); //show dialog if no text
    }
}

void MainWindow::onGoto() {
    auto dialog = new GotoDialog(this, _storage);
    switch (dialog->exec()) {
        case QDialog::Accepted:
            selectFolder(dialog->folderKey());
            if (dialog->fileKey().length() > 0) { selectFile(dialog->fileKey()); }
            break;
        default:
            break;
    }
}

void MainWindow::onFolderSetup() {
    _folder->saveAll();
    auto dialog = new FoldersDialog(this, _storage, _folder);
    dialog->exec();
    selectFolder(dialog->selectedFolder()); //will adjust folder if deleted
}

void MainWindow::onFolderMenuShow() {
    QFont italicFont = _folderButton->menu()->font();
    italicFont.setItalic(true);

    bool isFirst = true;
    _folderButton->menu()->clear();
    for (FolderItem* folder : *_storage) {
        if (isFirst) {
            isFirst = false;
        } else {
            if (folder->isRoot()) { _folderButton->menu()->addSeparator();                 }
        }
        QAction* folderAction = new QAction(folder->title());
        folderAction->setData(folder->name());
        folderAction->setDisabled(folder == _folder);
        if (!folder->isPrimary()) { folderAction->setFont(italicFont); }
        connect(folderAction, &QAction::triggered, this, &MainWindow::onFolderMenuSelect);
        _folderButton->menu()->addAction(folderAction);
    }
}

void MainWindow::onFolderMenuSelect() {
    QAction* action = qobject_cast<QAction*>(sender());

    if (action != nullptr) {
        auto key = action->data().value<QString>();
        selectFolder(key);

        auto lastFileKey = Settings::lastFile(_folder->name());
        selectFile(lastFileKey);
    }
}


void MainWindow::onFolderMove() {
    QAction* action = qobject_cast<QAction*>(sender());

    if ((ui->tabWidget->currentWidget() != nullptr) && (action != nullptr)) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

        auto key = action->data().value<QUuid>();
        auto destinationFolder = _storage->folderFromKey(key);
        if (destinationFolder != nullptr) {
            if (file->setFolder(destinationFolder)) {
                ui->tabWidget->removeTab(ui->tabWidget->currentIndex());
            }
        }
    }
}

void MainWindow::onFileConvert() {
    QAction* action = qobject_cast<QAction*>(sender());

    if ((ui->tabWidget->currentWidget() != nullptr) && (action != nullptr)) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

        auto type = static_cast<FileType>(action->data().value<int>());
        file->setType(type);
        onTextStateChanged();
    }
}


void MainWindow::onOpenWithDefaultApplication() {
    if (ui->tabWidget->currentWidget() != nullptr) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
        Helpers::openWithDefaultApplication(file->path());
    }
}

void MainWindow::onOpenWithVisualStudioCode() {
    if (ui->tabWidget->currentWidget() != nullptr) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
        Helpers::openFileWithVSCode(file);
    }
}

void onShowContainingDirectory2(QString directoryPath, QString filePath) {
    Helpers::showInFileManager(directoryPath, filePath);
}

void MainWindow::onShowContainingDirectory() {
    if (ui->tabWidget->currentWidget() != nullptr) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
        onShowContainingDirectory2(_folder->path(), file->path());
    } else {
        onShowContainingDirectory2(_folder->path(), nullptr);
    }
}

void MainWindow::onShowContainingDirectoryOnly() {
    onShowContainingDirectory2(_folder->path(), nullptr);
}


void MainWindow::onCopyContainingPath() {
    if (ui->tabWidget->currentWidget() != nullptr) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
        Clipboard::setData(QDir::toNativeSeparators(file->path()));
    } else {
        Clipboard::setData(QDir::toNativeSeparators(_folder->path()));
    }
}

void MainWindow::onPhoneticSpelling() {
    QString text = "";
    if (ui->tabWidget->currentWidget() != nullptr) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
        text = file->textCursor().selectedText();
    }
    auto dialog = new PhoneticDialog(this, text);
    dialog->exec();
}

void MainWindow::onTopMost() {
    Settings::setAlwaysOnTop(!Settings::alwaysOnTop());
    applySettings(false, false, true, false, false, false, false, false);
    this->show(); //to show window after setWindowFlag
    this->activateWindow();
}

void MainWindow::onAppSettings() {
    auto dialog = new SettingsDialog(this, _hotkey);
    if (dialog->exec() == QDialog::Accepted) {
        applySettings(dialog->changedShowInTaskbar(),
                      dialog->changedTabTextColorPerType(),
                      dialog->changedAlwaysOnTop(),
                      dialog->changedHotkey(),
                      dialog->changedHotkeyUseDConf(),
                      dialog->changedDataPath(),
                      dialog->changedFont(),
                      dialog->changedForceDarkMode());
        this->show(); //to show window after setWindowFlag
        this->activateWindow();
    }
}

void MainWindow::onAppFeedback() {
    Feedback::showDialog(this, QUrl("https://medo64.com/feedback/"));
}

void MainWindow::onAppUpgrade() {
    if (Upgrade::showDialog(this, QUrl("https://medo64.com/upgrade/"))) {
        onAppQuit();
    }
}

void MainWindow::onAppUpgradeChecked() {
    bool hasUpgrade = _futureWatcher->future().result();
    if (hasUpgrade && this->isVisible()) {
        _appButton->setIcon(Icons::settingsWithUpgrade());
    }
    delete _futureWatcher;
    _futureWatcher = nullptr;
}

void MainWindow::onAppAbout() {
    About::showDialog(this);
}

void MainWindow::onAppQuit() {
    _tray->hide();
    this->close();
    QCoreApplication::exit(0);
}

void MainWindow::onTabMenuRequested(const QPoint& point) {
    if (point.isNull()) { return; }

    auto tabbar = ui->tabWidget->tabBar();
    auto tabIndex = tabbar->tabAt(point);
    FileItem* file = nullptr;
    if (tabIndex >= 0) {
        ui->tabWidget->setCurrentIndex(tabIndex);
        file = dynamic_cast<FileItem*>(ui->tabWidget->widget(tabIndex));
    }

    bool vscodeAvailable = Helpers::openWithVSCodeAvailable();

    QFont italicFont = _folderButton->menu()->font();
    italicFont.setItalic(true);

    QMenu menu(this);
    menu.addAction(ui->actionNew);
    if (file != nullptr) {
        menu.addSeparator();
        menu.addAction(ui->actionReopen);
        menu.addAction(ui->actionSave);

        menu.addSeparator();
        menu.addAction(ui->actionRename);

        if (_storage->folderCount() > 1) {
            auto moveMenu = menu.addMenu("Move");
            bool isFirst = true;
            for (FolderItem* folder : *_storage) {
                if (isFirst) {
                    isFirst = false;
                } else {
                    if (folder->isRoot()) { moveMenu->addSeparator();                 }
                }
                auto folderAction = moveMenu->addAction(folder->title(), this, &MainWindow::onFolderMove);
                if (!folder->isPrimary()) { folderAction->setFont(italicFont); }
                folderAction->setData(folder->key());
                moveMenu->addAction(folderAction);
            }
        }

        auto convertMenu = menu.addMenu("Convert");
        auto convertToPlainAction = convertMenu->addAction("To Plain", this, &MainWindow::onFileConvert);
        convertToPlainAction->setEnabled(file->type() != FileType::Plain);
        convertToPlainAction->setData(static_cast<int>(FileType::Plain));
        auto convertToHtmlAction = convertMenu->addAction("To HTML", this, &MainWindow::onFileConvert);
        convertToHtmlAction->setEnabled(file->type() != FileType::Html);
        convertToHtmlAction->setData(static_cast<int>(FileType::Html));
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
        if (Settings::showMarkdown()) {
            auto convertToMarkdownAction = convertMenu->addAction("To Markdown", this, &MainWindow::onFileConvert);
            convertToMarkdownAction->setEnabled(file->type() != FileType::Markdown);
            convertToMarkdownAction->setData(static_cast<int>(FileType::Markdown));
        }
#endif

        menu.addSeparator();
        menu.addAction(ui->actionDelete);
        menu.addSeparator();
        menu.addAction(ui->actionOpenWithDefaultApplication);
        if (vscodeAvailable) { menu.addAction(ui->actionOpenWithVisualStudioCode); }
        menu.addAction(ui->actionShowContainingDirectory);
        menu.addAction(ui->actionCopyContainingPath);
    } else {
        menu.addSeparator();
        menu.addAction(ui->actionShowContainingDirectoryOnly);
        menu.addAction(ui->actionCopyContainingPath);
    }

    menu.exec(tabbar->mapToGlobal(point));
}

void MainWindow::onTabChanged() {
    LifetimeWatch watch("onTabChanged()", true);
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    bool exists = (file != nullptr);
    bool hasAny = (ui->tabWidget->count() == 0);

    ui->actionSave->setDisabled(!exists || file->isModified());
    ui->actionRename->setDisabled(hasAny);
    _printButton->setDisabled(hasAny);
    ui->actionPrint->setDisabled(hasAny);
    ui->actionPrintPreview->setDisabled(hasAny);
    ui->actionPrintToPdf->setDisabled(hasAny);

    onTextStateChanged();

    if (exists) {
        file->setFocus();
        Settings::setLastFile(_folder->name(), file->name());
    }
}

void MainWindow::onTabMoved(int from, int to) {
    _folder->moveFile(from, to);
    selectFolder(_folder); //workaround around tab widget not updating correctly
    ui->tabWidget->setCurrentIndex(to);
}


void MainWindow::onTextStateChanged() {
    LifetimeWatch watch("onTextStateChanged()");
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    bool hasFile = (file != nullptr) ? true : false;

    bool hasFontOperations = hasFile && (file->type() != FileType::Plain);
    bool isSelectionFontBold = hasFile && file->isFontBold();
    bool isSelectionFontItalic = hasFile && file->isFontItalic();
    bool isSelectionFontUnderline = hasFile && file->isFontUnderline();
    bool isSelectionFontStrikethrough = hasFile && file->isFontStrikethrough();

    ui->actionFontBold->setVisible(hasFontOperations);
    ui->actionFontItalic->setVisible(hasFontOperations);
    ui->actionFontUnderline->setVisible(hasFontOperations);
    ui->actionFontStrikethrough->setVisible(hasFontOperations);

    ui->actionCut->setDisabled(!hasFile || !file->canTextCut());
    ui->actionCopy->setDisabled(!hasFile || !file->canTextCopy());
    ui->actionPaste->setDisabled(!hasFile || !file->canTextPaste());
    ui->actionUndo->setDisabled(!hasFile || !file->isTextUndoAvailable());
    ui->actionRedo->setDisabled(!hasFile || !file->isTextRedoAvailable());
    ui->actionFind->setDisabled(!hasFile);
    ui->actionFindNext->setDisabled(!hasFile);

    ui->actionFontBold->setChecked(isSelectionFontBold);
    ui->actionFontItalic->setChecked(isSelectionFontItalic);
    ui->actionFontUnderline->setChecked(isSelectionFontUnderline);
    ui->actionFontStrikethrough->setChecked(isSelectionFontStrikethrough);
}

void MainWindow::onHotkeyPress() {
    if (this->isVisible() && Settings::hotkeyTogglesVisibility()) {
        this->hide();
    } else {
        onTrayShow();
    }
}

void MainWindow::onTrayActivate(QSystemTrayIcon::ActivationReason reason) {
    if (reason == QSystemTrayIcon::DoubleClick) {
        onTrayShow();
    }
}

void MainWindow::onTrayShow() {
    this->setWindowState((windowState() & ~Qt::WindowMinimized) | Qt::WindowActive);
    this->hide(); //workaround for Ubuntu
    this->show();
    this->raise(); //workaround for MacOS
    this->activateWindow(); //workaround for Windows
}

bool MainWindow::selectFolder(QString folderName) {
    LifetimeWatch watch("selectFolder(" + folderName + ") ");

    FolderItem* selectedFolder = nullptr;
    for (int i = 0; i < _storage->folderCount(); i++) {
        auto folder = _storage->folderAt(i);
        if (folder->name().compare(folderName, Qt::CaseSensitive) == 0) {
            selectedFolder = folder;
            break;
        }
    }
    if (selectedFolder == nullptr) { //try case-insensitive match
        for (int i = 0; i < _storage->folderCount(); i++) {
            auto folder = _storage->folderAt(i);
            if (folder->name().compare(folderName, Qt::CaseInsensitive) == 0) {
                selectedFolder = folder;
                break;
            }
        }
    }

    return selectFolder(selectedFolder);
}

bool MainWindow::selectFolder(FolderItem* selectedFolder) {
    if (selectedFolder != nullptr) {
        LifetimeWatch watch("selectFolder*(" + selectedFolder->name() + ")");

        if (_folder != nullptr) { _folder->saveAll(); } //save all files in previous folder / just in case
        _folder = selectedFolder;

        QFont italicFont = _folderButton->menu()->font();
        italicFont.setItalic(true);

        _folderButton->setText(_folder->title() + " ");
        _folderButton->setFont(!_folder->isPrimary() ? italicFont : _folderButton->menu()->font());
        Settings::setLastFolder(_folder->name());

        ui->tabWidget->blockSignals(true);
        ui->tabWidget->clear();
        for (int i = 0; i < _folder->fileCount(); i++) {
            auto file = _folder->fileAt(i);
            ui->tabWidget->addTab(file, file->title());
            QObject::connect(file, &FileItem::titleChanged, this, &MainWindow::onFileTitleChanged);
            QObject::connect(file, &FileItem::modificationChanged, this, &MainWindow::onFileModificationChanged);
            QObject::connect(file, &FileItem::activated, this, &MainWindow::onFileActivated);
            QObject::connect(file, &FileItem::currentCharFormatChanged, this, &MainWindow::onTextStateChanged);
            QObject::connect(file, &FileItem::cursorPositionChanged, this, &MainWindow::onTextStateChanged);
            QObject::connect(file->document(), &QTextDocument::undoAvailable, this, &MainWindow::onTextStateChanged);
            QObject::connect(file->document(), &QTextDocument::redoAvailable, this, &MainWindow::onTextStateChanged);
        }
        ui->tabWidget->blockSignals(false);

        if (_folder->fileCount() > 0) {
            selectFile(Settings::lastFile(_folder->name()));
        } else {
            onTabChanged(); //just to refresh disabled buttons
        }
        return true;
    }
    return false;
}

void MainWindow::selectFile(QString fileName) {
    LifetimeWatch watch("selectFile(" + fileName + ")");

    for (int i = 0; i < ui->tabWidget->count() ; i++) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->widget(i));
        if (file->name().compare(fileName, Qt::CaseInsensitive) == 0) {
            ui->tabWidget->setCurrentIndex(i);
            onTabChanged();
            break;
        }
    }
}

void MainWindow::selectFile(FileItem* file) {
    if (file != nullptr) {
        selectFile(file->name());
    }
}

void MainWindow::applySettings(bool applyShowInTaskbar, bool applyTabTextColorPerType, bool applyAlwaysOnTop, bool applyHotkey, bool applyDConfHotkey, bool applyDataPath, bool applyFont, bool applyForceDarkMode) {
    if (applyDataPath) {
        _storage->saveAll();
        ui->tabWidget->clear();
        _storage->deleteLater();

        _storage = new Storage(Settings::dataPaths());
        selectFolder(_storage->folderAt(0));
        connect(_storage, &Storage::updatedFolder, this, &MainWindow::onUpdatedFolder);
    }

    if (applyHotkey) { //just register again with the new key
        if (_hotkey != nullptr) {
            _hotkey->unregisterHotkey();
            _hotkey->registerHotkey(Settings::hotkey());
        }
#if defined(Q_OS_LINUX)
        if (_dconfHotkey != nullptr) {
            if (Settings::hotkeyUseDConf()) {
                _dconfHotkey->registerHotkey(Settings::hotkey());
            }
        }
#endif
    }

#if defined(Q_OS_LINUX)
    if (applyDConfHotkey) {
        if (_dconfHotkey != nullptr) {
            if (Settings::hotkeyUseDConf()) {
                if (!applyHotkey) {  // apply only if not applied already above
                    _dconfHotkey->registerHotkey(Settings::hotkey());
                }
            } else {
                _dconfHotkey->unregisterHotkey();
            }
        }
    }
#endif

    if (applyAlwaysOnTop) { //always on top might require restart (at least on Linux)
        auto flags = windowFlags();
        if (Settings::alwaysOnTop()) {
            flags = (flags | Qt::WindowStaysOnTopHint);
        } else {
            flags = (flags & ~Qt::WindowStaysOnTopHint);
        }
        setWindowFlags(flags);
    }

    if (applyShowInTaskbar) {
        if (!Settings::showInTaskbar()) {
            setWindowFlag(Qt::Tool);
        } else {
            setWindowFlag(Qt::Tool, false);
        }
    }
    if (applyTabTextColorPerType) {
        selectFolder(_folder);
    }

    if (applyForceDarkMode) { applyToolbarIcons(); }

    if (applyFont) {
        for (auto i = 0; i < ui->tabWidget->count(); i++) {  // refresh all tabs
            auto file = dynamic_cast<FileItem*>(ui->tabWidget->widget(i));
            file->load();
        }
    }
}

void MainWindow::applyToolbarIcons() {
    bool isDark = Helpers::isOSInDarkMode() || Settings::forceDarkMode();
    Helpers::setupTheme(isDark);
    Icons::setDarkMode(isDark);

    ui->actionNew->setIcon(Icons::newFile());
    ui->actionSave->setIcon(Icons::saveFile());
    ui->actionRename->setIcon(Icons::renameFile());
    ui->actionDelete->setIcon(Icons::deleteFile());
    ui->actionPrint->setIcon(Icons::printFile());
    ui->actionPrintPreview->setIcon(Icons::printPreviewFile());
    ui->actionPrintToPdf->setIcon(Icons::printToPdfFile());
    ui->actionCut->setIcon(Icons::cut());
    ui->actionCopy->setIcon(Icons::copy());
    ui->actionPaste->setIcon(Icons::paste());
    ui->actionUndo->setIcon(Icons::undo());
    ui->actionRedo->setIcon(Icons::redo());
    ui->actionFontBold->setIcon(Icons::fontBold());
    ui->actionFontItalic->setIcon(Icons::fontItalic());
    ui->actionFontUnderline->setIcon(Icons::fontUnderline());
    ui->actionFontStrikethrough->setIcon(Icons::fontStrikethrough());
    ui->actionFind->setIcon(Icons::find());
    ui->actionFindNext->setIcon(Icons::findNext());
    ui->actionGoto->setIcon(Icons::gotoIcon());
    _printButton->setIcon(Icons::printFile());
    _appButton->setIcon(Icons::settings());
}

void MainWindow::onUpdatedFolder(FolderItem* folder) {
    if (folder == _folder) {
        selectFolder(folder); //refresh current folder
    }
}
