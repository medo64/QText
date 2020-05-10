#include <QDir>
#include <QFileDialog>
#include <QMenu>
#include <QMessageBox>
#include <QTabBar>
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
#include "storage.h"

#include "medo/config.h"
#include "medo/singleinstance.h"
#include "medo/state.h"

#include "filenamedialog.h"
#include "finddialog.h"
#include "foldersdialog.h"
#include "gotodialog.h"
#include "settingsdialog.h"

#include "mainwindow.h"
#include "ui_mainwindow.h"


MainWindow::MainWindow(Storage* storage) : QMainWindow(nullptr), ui(new Ui::MainWindow) {
    ui->setupUi(this);

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
    _tray->setIcon(Settings::colorTrayIcon() ? Icons::trayColor() : Icons::trayWhite());
    auto hotkeyText = Settings::hotkey().toString(QKeySequence::PortableText);
    if (hotkeyText.length() > 0) {
        _tray->setToolTip( + "Access notes from tray or press " + hotkeyText + " hotkey.");
    } else {
        _tray->setToolTip( + "Access notes from tray.");
    }
    _tray->show();

    //hotkey
    _hotkey = new Hotkey(this);
    _hotkey->registerHotkey(Settings::hotkey());
    connect(_hotkey, &Hotkey::activated, this, &MainWindow::onTrayShow);

    //single instance
    connect(SingleInstance::instance(), &SingleInstance::newInstanceDetected, this, &MainWindow::onTrayShow);

    //toolbar setup
    ui->actionNew->setIcon(Icons::newFile());
    connect(ui->actionNew, &QAction::triggered, this, &MainWindow::onFileNew);

    ui->actionSave->setIcon(Icons::saveFile());
    connect(ui->actionSave, &QAction::triggered, this, &MainWindow::onFileSave);

    ui->actionRename->setIcon(Icons::renameFile());
    connect(ui->actionRename, &QAction::triggered, this, &MainWindow::onFileRename);

    ui->actionDelete->setIcon(Icons::deleteFile());
    connect(ui->actionDelete, &QAction::triggered, this, &MainWindow::onFileDelete);

    ui->actionPrint->setIcon(Icons::printFile());
    connect(ui->actionPrint, &QAction::triggered, this, &MainWindow::onFilePrint);

    ui->actionPrintPreview->setIcon(Icons::printPreviewFile());
    connect(ui->actionPrintPreview, &QAction::triggered, this, &MainWindow::onFilePrintPreview);

    ui->actionPrintToPdf->setIcon(Icons::printToPdfFile());
    connect(ui->actionPrintToPdf, &QAction::triggered, this, &MainWindow::onFilePrintToPdf);

    ui->actionCut->setIcon(Icons::cut());
    connect(ui->actionCut, &QAction::triggered, this, &MainWindow::onTextCut);

    ui->actionCopy->setIcon(Icons::copy());
    connect(ui->actionCopy, &QAction::triggered, this, &MainWindow::onTextCopy);

    ui->actionPaste->setIcon(Icons::paste());
    connect(ui->actionPaste, &QAction::triggered, this, &MainWindow::onTextPaste);

    ui->actionUndo->setIcon(Icons::undo());
    connect(ui->actionUndo, &QAction::triggered, this, &MainWindow::onTextUndo);

    ui->actionRedo->setIcon(Icons::redo());
    connect(ui->actionRedo, &QAction::triggered, this, &MainWindow::onTextRedo);

    ui->actionFind->setIcon(Icons::find());
    connect(ui->actionFind, &QAction::triggered, this, &MainWindow::onFind);

    ui->actionFindNext->setIcon(Icons::findNext());
    connect(ui->actionFindNext, &QAction::triggered, this, &MainWindow::onFindNext);

    ui->actionGoto->setIcon(Icons::gotoIcon());
    connect(ui->actionGoto, &QAction::triggered, this, &MainWindow::onGoto);

    //print button menu
    _printButton = new QToolButton();
    _printButton->setIcon(Icons::printFile());
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
    ui->mainToolBar->addWidget(spacerWidget);

    //folder button menu
    _folderButton = new QToolButton();
    _folderButton->setPopupMode(QToolButton::MenuButtonPopup);
    _folderButton->setMenu(new QMenu());
    ui->mainToolBar->addWidget(_folderButton);
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
    _appButton->setIcon(Icons::settings());
    _appButton->setPopupMode(QToolButton::MenuButtonPopup);
    _appButton->setMenu(new QMenu());
    connect(_appButton, &QToolButton::clicked, this, &MainWindow::onAppSettings);

    QAction* appSettingsAction = new QAction("&Settings");
    connect(appSettingsAction, &QAction::triggered, this, &MainWindow::onAppSettings);
    _appButton->menu()->addAction(appSettingsAction);

    QAction* appAboutAction = new QAction("&About");
    connect(appAboutAction, &QAction::triggered, this, &MainWindow::onAppAbout);
    _appButton->menu()->addAction(appAboutAction);

    _appButton->menu()->addSeparator();

    QAction* appQuitAction = new QAction("&Quit");
    connect(appQuitAction, &QAction::triggered, this, &MainWindow::onAppQuit);
    _appButton->menu()->addAction(appQuitAction);

    ui->mainToolBar->addWidget(_appButton);

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
    if (Settings::alwaysOnTop()) { setWindowFlag(Qt::WindowStaysOnTopHint); } //always on top cannot be set dynamically :(
    applySettings();

    if (!Settings::setupCompleted()) { // show extra info when ran first time
        auto duration = Config::isPortable() ? 1000 : 2500; //shorter duration if config is portable
        _tray->showMessage(QCoreApplication::applicationName(), _tray->toolTip(), QSystemTrayIcon::Information, duration);
    }

    //determine last used folder
    selectFolder(storage->getBaseFolder()->getKey()); //default folder
    selectFolder(Settings::lastFolder());
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
    auto data = static_cast<uint>(event->key()) | event->modifiers();
    switch (data) {
        case Qt::Key_Escape:
        case Qt::ControlModifier | Qt::Key_F4:
            State::save(this);
            this->hide();
#ifdef QT_DEBUG //close immediately for easier debugging
            QCoreApplication::exit(0);
#endif
            break;

        case Qt::ShiftModifier | Qt::Key_F3:
            onFindNext(true);
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


        case Qt::ControlModifier | Qt::Key_O:
            _folderButton->showMenu();
            break;

        case Qt::ControlModifier | Qt::AltModifier | Qt::Key_C:
            onCopyContainingPath();
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

        case Qt::AltModifier | Qt::Key_Left:
            if (ui->tabWidget->currentIndex() > 0) { ui->tabWidget->setCurrentIndex(ui->tabWidget->currentIndex() - 1); }
            break;

        case Qt::AltModifier | Qt::Key_Right:
            if (ui->tabWidget->currentIndex() < ui->tabWidget->count() - 1) { ui->tabWidget->setCurrentIndex(ui->tabWidget->currentIndex() + 1); }
            break;

        case Qt::AltModifier | Qt::Key_Up:
        case Qt::AltModifier | Qt::Key_PageUp:
            for (int i = 1; i < _storage->folderCount(); i++) {
                auto folder = _storage->getFolder(i);
                if (folder->getKey().compare(_folder->getKey(), Qt::CaseSensitive) == 0) {
                    auto newFolder = _storage->getFolder(i - 1);
                    selectFolder(newFolder->getKey());
                    break;
                }
            }
            break;

        case Qt::AltModifier | Qt::Key_Down:
        case Qt::AltModifier | Qt::Key_PageDown:
            for (int i = 0; i < _storage->folderCount() - 1; i++) {
                auto folder = _storage->getFolder(i);
                if (folder->getKey().compare(_folder->getKey(), Qt::CaseSensitive) == 0) {
                    auto newFolder = _storage->getFolder(i + 1);
                    selectFolder(newFolder->getKey());
                    break;
                }
            }
            break;

        case Qt::AltModifier | Qt::Key_Home:
            selectFolder(_storage->getBaseFolder()->getKey());
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

        default: QMainWindow::keyPressEvent(event);
    }
}


void MainWindow::onFileModificationChanged(FileItem* file, bool isModified) {
    auto tabIndex = ui->tabWidget->indexOf(file);
    if (isModified) {
        ui->tabWidget->setTabText(tabIndex, file->getTitle() + "*");
        ui->actionSave->setDisabled(false);
    } else {
        ui->tabWidget->setTabText(tabIndex, file->getTitle());
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
    auto dialog = new FileNameDialog(this, _folder);
    switch (dialog->exec()) {
        case QDialog::Accepted: {
                auto newTitle = dialog->getTitle();
                auto file = _folder->newFile(newTitle);
                auto index = ui->tabWidget->addTab(file, file->getTitle());
                ui->tabWidget->setCurrentIndex(index);
            }
            break;
        default:
            break;
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

    auto dialog = new FileNameDialog(this, file);
    switch (dialog->exec()) {
        case QDialog::Accepted: {
                auto newTitle = dialog->getTitle();
                file->setTitle(newTitle);
                ui->tabWidget->setTabText(tabIndex, newTitle);
                file->setFocus();
            }
            break;
        default:
            break;
    }
}

void MainWindow::onFileDelete() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    QMessageBox msgBox;
    msgBox.setIcon(QMessageBox::Question);
    msgBox.setText("The document has been modified");
    msgBox.setInformativeText("Do you really want to delete " + file->getTitle() + "?");
    msgBox.setStandardButtons(QMessageBox::Yes | QMessageBox::No);
    msgBox.setDefaultButton(QMessageBox::Yes);
    if (file->isEmpty() || (msgBox.exec() == QMessageBox::Yes)) {
        file->deleteLater();
        _folder->deleteFile(file);
    }
}

void MainWindow::onFilePrint() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    QPrinter printer(QPrinter::PrinterResolution);

    QPrintDialog dialog(&printer, this);
    Helpers::setupResizableDialog(&dialog);
    dialog.setWindowTitle("Print: " + file->getTitle());
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
    dialog.setWindowTitle("Print Preview: " + file->getTitle());
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
    Clipboard::cutText(file->textCursor());
}

void MainWindow::onTextCopy() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    Clipboard::copyText(file->textCursor());
}

void MainWindow::onTextPaste() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    Clipboard::pasteText(file->textCursor());
}

void MainWindow::onTextUndo() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->document()->undo();
}

void MainWindow::onTextRedo() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->document()->redo();
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
            FolderItem* folder = file->getFolder();
            FolderItem* nextFolder = nextFile->getFolder();
            if (folder != nextFolder) { selectFolder(nextFolder); }
            selectFile(nextFile->getKey());
        }
    } else {
        onFind(); //show dialog if no text
    }
}

void MainWindow::onGoto() {
    auto dialog = new GotoDialog(this, _storage);
    switch (dialog->exec()) {
        case QDialog::Accepted:
            selectFolder(dialog->FolderKey);
            if (dialog->FileKey.length() > 0) { selectFile(dialog->FileKey); }
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

    _folderButton->menu()->clear();
    for (int i = 0; i < _storage->folderCount(); i++) {
        auto folder = _storage->getFolder(i);
        QAction* folderAction = new QAction(folder->getTitle());
        folderAction->setData(folder->getKey());
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

        auto lastFileKey = Settings::lastFile(_folder->getKey());
        selectFile(lastFileKey);
    }
}


void MainWindow::onOpenWithDefaultApplication() {
    if (ui->tabWidget->currentWidget() != nullptr) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
        Helpers::openWithDefaultApplication(file->getPath());
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
        onShowContainingDirectory2(_folder->getPath(), file->getPath());
    } else {
        onShowContainingDirectory2(_folder->getPath(), nullptr);
    }
}

void MainWindow::onShowContainingDirectoryOnly() {
    onShowContainingDirectory2(_folder->getPath(), nullptr);
}


void MainWindow::onCopyContainingPath() {
    if (ui->tabWidget->currentWidget() != nullptr) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
        Clipboard::setText(QDir::toNativeSeparators(file->getPath()));
    } else {
        Clipboard::setText(QDir::toNativeSeparators(_folder->getPath()));
    }
}

void MainWindow::onAppSettings() {
    auto dialog = new SettingsDialog(this);
    switch (dialog->exec()) {
        case QDialog::Accepted:
            applySettings(dialog->changedShowInTaskbar);
            this->show(); //to show window after setWindowFlag
            this->activateWindow();
            break;

        default:
            break;
    }
}

void MainWindow::onAppAbout() {
    QString description = QCoreApplication::applicationName() + " " + APP_VERSION;
    QString commit = APP_COMMIT;
    if (commit.length() > 0) {
        description.append("+");
        description.append(APP_COMMIT);
    }
#ifdef QT_DEBUG
    description.append("\nDEBUG");
#endif
    description.append("\n\nQt "); description.append(APP_QT_VERSION);
    QMessageBox::about(this, "About",  description);
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

    QMenu menu(this);
    menu.addAction(ui->actionNew);
    if (file != nullptr) {
        menu.addSeparator();
        menu.addAction(ui->actionReopen);
        menu.addAction(ui->actionSave);
        menu.addSeparator();
        menu.addAction(ui->actionRename);
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
        Settings::setLastFile(_folder->getKey(), file->getKey());
    }
}

void MainWindow::onTabMoved(int from, int to) {
    _folder->moveFile(from, to);
    selectFolder(_folder); //workaround around tab widget not updating correctly
    ui->tabWidget->setCurrentIndex(to);
}


void MainWindow::onTextStateChanged() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    bool hasFile = (file != nullptr) ? true : false;
    bool isSelectionAvailable = hasFile ? file->textCursor().hasSelection() : false;

    auto document = hasFile ? file->document() : nullptr;
    bool isUndoAvailable = (document != nullptr) ? document->isUndoAvailable() : false;
    bool isRedoAvailable = (document != nullptr) ? document->isRedoAvailable() : false;

    bool isClipboardTextAvailable = hasFile ? Clipboard::hasText() : false;

    ui->actionCut->setDisabled(!isSelectionAvailable);
    ui->actionCopy->setDisabled(!isSelectionAvailable);
    ui->actionPaste->setDisabled(!isClipboardTextAvailable);
    ui->actionUndo->setDisabled(!isUndoAvailable);
    ui->actionRedo->setDisabled(!isRedoAvailable);
    ui->actionFind->setDisabled(!hasFile);
    ui->actionFindNext->setDisabled(!hasFile);
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

void MainWindow::selectFolder(QString folderKey) {
    qDebug().nospace() << "selectFolder(" << folderKey << ") ";
    FolderItem* selectedFolder = nullptr;

    for (int i = 0; i < _storage->folderCount(); i++) {
        auto folder = _storage->getFolder(i);
        if (folder->getKey().compare(folderKey, Qt::CaseSensitive) == 0) {
            selectedFolder = folder;
            break;
        }
    }
    if (selectedFolder == nullptr) { //try case-insensitive match
        for (int i = 0; i < _storage->folderCount(); i++) {
            auto folder = _storage->getFolder(i);
            if (folder->getKey().compare(folderKey, Qt::CaseInsensitive) == 0) {
                selectedFolder = folder;
                break;
            }
        }
    }

    selectFolder(selectedFolder);
}

void MainWindow::selectFolder(FolderItem* selectedFolder) {
    if (selectedFolder != nullptr) {
        if (_folder != nullptr) { _folder->saveAll(); } //save all files in previous folder / just in case
        _folder = selectedFolder;

        QFont italicFont = _folderButton->menu()->font();
        italicFont.setItalic(true);

        _folderButton->setText(_folder->getTitle() + " ");
        _folderButton->setFont(!_folder->isPrimary() ? italicFont : _folderButton->menu()->font());
        Settings::setLastFolder(_folder->getKey());

        ui->tabWidget->blockSignals(true);
        ui->tabWidget->clear();
        for (int i = 0; i < _folder->fileCount(); i++) {
            auto file = _folder->getFile(i);
            ui->tabWidget->addTab(file, file->getTitle());
            QObject::connect(file, &FileItem::titleChanged, this, &MainWindow::onFileTitleChanged);
            QObject::connect(file, &FileItem::modificationChanged, this, &MainWindow::onFileModificationChanged);
            QObject::connect(file, &FileItem::activated, this, &MainWindow::onFileActivated);
            QObject::connect(file, &FileItem::cursorPositionChanged, this, &MainWindow::onTextStateChanged);
            QObject::connect(file->document(), &QTextDocument::undoAvailable, this, &MainWindow::onTextStateChanged);
            QObject::connect(file->document(), &QTextDocument::redoAvailable, this, &MainWindow::onTextStateChanged);
        }
        ui->tabWidget->blockSignals(false);

        if (_folder->fileCount() > 0) {
            selectFile(Settings::lastFile(_folder->getKey()));
        } else {
            onTabChanged(); //just to refresh disabled buttons
        }
    }
}

void MainWindow::selectFile(QString fileKey) {
    qDebug().nospace() << "selectFile(" << fileKey << ") " << _folder->getKey();
    for (int i = 0; i < ui->tabWidget->count() ; i++) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->widget(i));
        if (file->getKey().compare(fileKey, Qt::CaseInsensitive) == 0) {
            ui->tabWidget->setCurrentIndex(i);
            onTabChanged();
            break;
        }
    }
}

void MainWindow::applySettings(bool applyShowInTaskbar) {
    if (applyShowInTaskbar) {
        if (!Settings::showInTaskbar()) {
            setWindowFlag(Qt::Tool);
        } else {
            setWindowFlag(Qt::Tool, false);
        }
    }
}
