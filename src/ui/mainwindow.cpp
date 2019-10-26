#include <QDir>
#include <QMenu>
#include <QMessageBox>
#include <QTabBar>
#include "clipboard.h"
#include "helpers.h"
#include "icons.h"
#include "settings.h"
#include "storage.h"
#include "medo/config.h"
#include "medo/singleinstance.h"
#include "medo/state.h"

#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "filenamedialog.h"
#include "ui_filenamedialog.h"
#include "gotodialog.h"
#include "ui_gotodialog.h"
#include "settingsdialog.h"
#include "ui_settingsdialog.h"

MainWindow::MainWindow(Storage* storage) : QMainWindow(nullptr), ui(new Ui::MainWindow) {
    ui->setupUi(this);

    _storage = storage;

    { //application icon
        this->setWindowIcon(Icons::application());
    }

    { //tray
        _tray = new QSystemTrayIcon(this);
        connect(_tray, SIGNAL(activated(QSystemTrayIcon::ActivationReason)), this, SLOT(onTrayActivate(QSystemTrayIcon::ActivationReason)));

        QMenu* trayMenu = new QMenu(this);
        auto defaultAction = trayMenu->addAction("&Show", this, SLOT(onTrayShow()));
        trayMenu->addSeparator();
        trayMenu->addAction("&Quit", this, SLOT(onAppQuit()));

        auto font = defaultAction->font();
        font.setBold(true);
        defaultAction->setFont(font);
        _tray->setContextMenu(trayMenu);
        _tray->setIcon(Icons::tray());
        auto hotkeyText = Settings::hotkey().toString(QKeySequence::PortableText);
        if (hotkeyText.length() > 0) {
            _tray->setToolTip( + "Access notes from tray or press " + hotkeyText + " hotkey.");
        } else {
            _tray->setToolTip( + "Access notes from tray.");
        }
        _tray->show();
    }

    { //hotkey
        _hotkey = new Hotkey(this);
        _hotkey->registerHotkey(Settings::hotkey());
        connect(_hotkey, SIGNAL(activated()), this, SLOT(onTrayShow()));
    }

    { //single instance
        connect(SingleInstance::instance(), SIGNAL(newInstanceDetected()), this, SLOT(onTrayShow()));
    }

    { //toolbar setup
        ui->actionNew->setIcon(Icons::newFile());
        connect(ui->actionNew, SIGNAL(triggered()), this, SLOT(onFileNew()));

        ui->actionSave->setIcon(Icons::saveFile());
        connect(ui->actionSave, SIGNAL(triggered()), this, SLOT(onFileSave()));

        ui->actionRename->setIcon(Icons::renameFile());
        connect(ui->actionRename, SIGNAL(triggered()), this, SLOT(onFileRename()));

        ui->actionDelete->setIcon(Icons::deleteFile());
        connect(ui->actionDelete, SIGNAL(triggered()), this, SLOT(onFileDelete()));

        ui->actionCut->setIcon(Icons::cut());
        connect(ui->actionCut, SIGNAL(triggered()), this, SLOT(onTextCut()));

        ui->actionCopy->setIcon(Icons::copy());
        connect(ui->actionCopy, SIGNAL(triggered()), this, SLOT(onTextCopy()));

        ui->actionPaste->setIcon(Icons::paste());
        connect(ui->actionPaste, SIGNAL(triggered()), this, SLOT(onTextPaste()));

        ui->actionUndo->setIcon(Icons::undo());
        connect(ui->actionUndo, SIGNAL(triggered()), this, SLOT(onTextUndo()));

        ui->actionRedo->setIcon(Icons::redo());
        connect(ui->actionRedo, SIGNAL(triggered()), this, SLOT(onTextRedo()));

        ui->actionGoto->setIcon(Icons::gotoDialog());
        connect(ui->actionGoto, SIGNAL(triggered()), this, SLOT(onGoto()));
    }

    //align-right
    QWidget* spacerWidget = new QWidget(this);
    spacerWidget->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);
    spacerWidget->setVisible(true);
    ui->mainToolBar->addWidget(spacerWidget);

    { //folder button menu
        _folderButton = new QToolButton();
        _folderButton->setPopupMode(QToolButton::InstantPopup);
        _folderButton->setMenu(new QMenu());
        ui->mainToolBar->addWidget(_folderButton);
        connect(_folderButton->menu(), SIGNAL(aboutToShow()), this, SLOT(onFolderMenuShow()));

        onFolderMenuSelect(); //setup folder select menu button
        onTabChanged(); //update toolbar & focus
        connect(ui->actionReopen, SIGNAL(triggered()), this, SLOT(onFileReopen()));
        connect(ui->actionShowContainingDirectory, SIGNAL(triggered()), this, SLOT(onShowContainingDirectory()));
        connect(ui->actionShowContainingDirectoryOnly, SIGNAL(triggered()), this, SLOT(onShowContainingDirectoryOnly()));
        connect(ui->actionCopyContainingPath, SIGNAL(triggered()), this, SLOT(onCopyContainingPath()));
    }

    { //app button menu
        _appButton = new QToolButton();
        _appButton->setIcon(Icons::appMenu());
        _appButton->setPopupMode(QToolButton::MenuButtonPopup);
        _appButton->setMenu(new QMenu());
        connect(_appButton, SIGNAL(clicked()), this, SLOT(onAppSettings()));

        QAction* appSettingsAction = new QAction("&Settings");
        connect(appSettingsAction, SIGNAL(triggered()), this, SLOT(onAppSettings()));
        _appButton->menu()->addAction(appSettingsAction);

        QAction* appAboutAction = new QAction("&About");
        connect(appAboutAction, SIGNAL(triggered()), this, SLOT(onAppAbout()));
        _appButton->menu()->addAction(appAboutAction);

        _appButton->menu()->addSeparator();

        QAction* appQuitAction = new QAction("&Quit");
        connect(appQuitAction, SIGNAL(triggered()), this, SLOT(onAppQuit()));
        _appButton->menu()->addAction(appQuitAction);
    }
    ui->mainToolBar->addWidget(_appButton);

    //tabs
    ui->tabWidget->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(ui->tabWidget, SIGNAL(customContextMenuRequested(const QPoint&)), SLOT(onTabMenuRequested(const QPoint&)));
    ui->tabWidget->tabBar()->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(ui->tabWidget->tabBar(), SIGNAL(customContextMenuRequested(const QPoint&)), SLOT(onTabMenuRequested(const QPoint&)));
    connect(ui->tabWidget, SIGNAL(currentChanged(int)), SLOT(onTabChanged()));
    connect(ui->tabWidget, SIGNAL(tabMoved(int, int)), SLOT(onTabMoved(int, int)));

    //clipboard
    connect(QApplication::clipboard(), SIGNAL(dataChanged()), SLOT(onTextStateChanged()));

    { //State
        connect(State::instance(), &State::writeToConfig, [=] (QString key, QString value) { Config::stateWrite("State!" + key, value); });
        connect(State::instance(), &State::readFromConfig, [=] (QString key) { return Config::stateRead("State!" + key, QString()); });
        State::load(this);
    }

    { //show shortcuts in context menu
        for(QAction* action : findChildren<QAction*>()) {
            action->setShortcutVisibleInContextMenu(true);
        }
    }

    { //show shortcut in tooltip
        for(QAction* action : ui->mainToolBar->actions()) {
            auto shortcut = action->shortcut();
            if (!shortcut.isEmpty()) {
                action->setToolTip(action->toolTip() + "\n(" + shortcut.toString() + ")");
            }
        }
    }

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


void MainWindow::changeEvent(QEvent *event) {
    if (event->type() == QEvent::WindowStateChange) {
        if (isMinimized() && Settings::minimizeToTray()) {
            this->hide();
        }
    }
    return QMainWindow::changeEvent(event);
}

void MainWindow::closeEvent(QCloseEvent *event) {
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

void MainWindow::keyPressEvent(QKeyEvent *event) {
    auto data = static_cast<uint>(event->key()) | event->modifiers();
    switch(data) {
        case Qt::Key_Escape:
        case Qt::ControlModifier | Qt::Key_F4: {
            State::save(this);
            this->hide();
#ifdef QT_DEBUG //close immediately for easier debugging
            QCoreApplication::exit(0);
#endif
        } break;

        case Qt::ControlModifier | Qt::Key_O: {
            _folderButton->showMenu();
        } break;

        case Qt::ControlModifier | Qt::AltModifier | Qt::Key_C: {
            onCopyContainingPath();
        } break;

        case Qt::AltModifier | Qt::Key_1: {
            if (ui->tabWidget->count() >= 1) { ui->tabWidget->setCurrentIndex(0); }
        } break;

        case Qt::AltModifier | Qt::Key_2: {
            if (ui->tabWidget->count() >= 2) { ui->tabWidget->setCurrentIndex(1); }
        } break;

        case Qt::AltModifier | Qt::Key_3: {
            if (ui->tabWidget->count() >= 3) { ui->tabWidget->setCurrentIndex(2); }
        } break;

        case Qt::AltModifier | Qt::Key_4: {
            if (ui->tabWidget->count() >= 4) { ui->tabWidget->setCurrentIndex(3); }
        } break;

        case Qt::AltModifier | Qt::Key_5: {
            if (ui->tabWidget->count() >= 5) { ui->tabWidget->setCurrentIndex(4); }
        } break;

        case Qt::AltModifier | Qt::Key_6: {
            if (ui->tabWidget->count() >= 6) { ui->tabWidget->setCurrentIndex(5); }
        } break;

        case Qt::AltModifier | Qt::Key_7: {
            if (ui->tabWidget->count() >= 7) { ui->tabWidget->setCurrentIndex(6); }
        } break;

        case Qt::AltModifier | Qt::Key_8: {
            if (ui->tabWidget->count() >= 8) { ui->tabWidget->setCurrentIndex(7); }
        } break;

        case Qt::AltModifier | Qt::Key_9: {
            if (ui->tabWidget->count() >= 9) { ui->tabWidget->setCurrentIndex(8); }
        } break;

        case Qt::AltModifier | Qt::Key_0: {
            if (ui->tabWidget->count() >= 10) { ui->tabWidget->setCurrentIndex(9); }
        } break;

        case Qt::AltModifier | Qt::Key_Left: {
            if (ui->tabWidget->currentIndex() > 0) { ui->tabWidget->setCurrentIndex(ui->tabWidget->currentIndex() - 1); }
        } break;

        case Qt::AltModifier | Qt::Key_Right: {
            if (ui->tabWidget->currentIndex() < ui->tabWidget->count() - 1) { ui->tabWidget->setCurrentIndex(ui->tabWidget->currentIndex() + 1); }
        } break;

        case Qt::AltModifier | Qt::Key_Up: {
            for (int i = 1; i < _storage->folderCount(); i++) {
                auto folder = _storage->getFolder(i);
                if (folder->getKey().compare(_folder->getKey(), Qt::CaseSensitive) == 0) {
                    auto newFolder = _storage->getFolder(i - 1);
                    selectFolder(newFolder->getKey());
                    break;
                }
            }
        } break;

        case Qt::AltModifier | Qt::Key_Down: {
            for (int i = 0; i < _storage->folderCount() - 1; i++) {
                auto folder = _storage->getFolder(i);
                if (folder->getKey().compare(_folder->getKey(), Qt::CaseSensitive) == 0) {
                    auto newFolder = _storage->getFolder(i + 1);
                    selectFolder(newFolder->getKey());
                    break;
                }
            }
        } break;

        case Qt::AltModifier | Qt::Key_Home: {
            selectFolder(_storage->getBaseFolder()->getKey());
        } break;

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
    auto dialog = new FileNameDialog(this, _folder.get());
    switch (dialog->exec()) {
        case QDialog::Accepted:
            {
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
        case QDialog::Accepted:
            {
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

void MainWindow::onGoto() {
    auto dialog = std::make_shared<GotoDialog>(this, _storage);
    switch (dialog->exec()) {
        case QDialog::Accepted: {
                selectFolder(dialog->FolderKey);
                if (dialog->FileKey.length() > 0) { selectFile(dialog->FileKey); }
            } break;
        default:
            break;
    }
}

void MainWindow::onFolderMenuShow() {
    QFont italicFont = _folderButton->menu()->font();
    italicFont.setItalic(true);

    _folderButton->menu()->clear();
    for(int i = 0; i < _storage->folderCount(); i++) {
        auto folder = _storage->getFolder(i);
        QAction* folderAction = new QAction(folder->getTitle());
        folderAction->setData(folder->getKey());
        folderAction->setDisabled(folder == _folder);
        if (folder->hasPrefix()) { folderAction->setFont(italicFont); }
        connect(folderAction, SIGNAL(triggered()), this, SLOT(onFolderMenuSelect()));
        _folderButton->menu()->addAction(folderAction);
    }
}

void MainWindow::onFolderMenuSelect() {
    QAction *action = qobject_cast<QAction *>(sender());

    if (action != nullptr) {
        auto key = action->data().value<QString>();
        selectFolder(key);

        auto lastFileKey = Settings::lastFile(_folder->getKey());
        selectFile(lastFileKey);
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
    auto dialog = std::make_shared<SettingsDialog>(this);
    switch (dialog->exec()) {
        case QDialog::Accepted: {
            applySettings(dialog->changedShowInTaskbar);
            this->show(); //to show window after setWindowFlag
            this->activateWindow();
        } break;

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

void MainWindow::onTabMenuRequested(const QPoint &point) {
    if (point.isNull()) { return; }

    auto tabbar = ui->tabWidget->tabBar();
    auto tabIndex = tabbar->tabAt(point);
    FileItem *file = nullptr;
    if (tabIndex >= 0) {
        ui->tabWidget->setCurrentIndex(tabIndex);
        file = dynamic_cast<FileItem*>(ui->tabWidget->widget(tabIndex));
    }

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

    ui->actionSave->setDisabled(!exists || file->isModified());
    ui->actionRename->setDisabled(ui->tabWidget->count() == 0);

    onTextStateChanged();

    if (exists) {
        file->setFocus();
        Settings::setLastFile(_folder->getKey(), file->getKey());
    }
}

void MainWindow::onTabMoved(int from, int to) {
    _folder->moveFile(from, to);
}


void MainWindow::onTextStateChanged() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    bool isSelectionAvailable = (file != nullptr) ? file->textCursor().hasSelection() : false;

    auto document = (file != nullptr) ? file->document() : nullptr;
    bool isUndoAvailable = (document !=nullptr) ? document->isUndoAvailable() : false;
    bool isRedoAvailable = (document !=nullptr) ? document->isRedoAvailable() : false;

    bool isClipboardTextAvailable = Clipboard::hasText();

    ui->actionCut->setDisabled(!isSelectionAvailable);
    ui->actionCopy->setDisabled(!isSelectionAvailable);
    ui->actionPaste->setDisabled(!isClipboardTextAvailable);
    ui->actionUndo->setDisabled(!isUndoAvailable);
    ui->actionRedo->setDisabled(!isRedoAvailable);
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
    std::shared_ptr<FolderItem> selectedFolder = nullptr;

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

    if (selectedFolder != nullptr) {
        if (_folder != nullptr) { _folder->saveAll(); } //save all files in previous folder / just in case
        _folder = selectedFolder;

        QFont italicFont = _folderButton->menu()->font();
        italicFont.setItalic(true);

        _folderButton->setText(_folder->getTitle() + " ");
        _folderButton->setFont(_folder->hasPrefix() ? italicFont : _folderButton->menu()->font());
        Settings::setLastFolder(_folder->getKey());

        ui->tabWidget->blockSignals(true);
        ui->tabWidget->clear();
        for (size_t i = 0; i < _folder->fileCount(); i++) {
            auto file = _folder->getFile(i);
            ui->tabWidget->addTab(file, file->getTitle());
            QObject::connect(file, SIGNAL(titleChanged(FileItem*)), this, SLOT(onFileTitleChanged(FileItem*)));
            QObject::connect(file, SIGNAL(modificationChanged(FileItem*, bool)), this, SLOT(onFileModificationChanged(FileItem*, bool)));
            QObject::connect(file, SIGNAL(activated(FileItem*)), this, SLOT(onFileActivated(FileItem*)));
            QObject::connect(file, SIGNAL(cursorPositionChanged()), this, SLOT(onTextStateChanged()));
            QObject::connect(file->document(), SIGNAL(undoAvailable(bool)), this, SLOT(onTextStateChanged()));
            QObject::connect(file->document(), SIGNAL(redoAvailable(bool)), this, SLOT(onTextStateChanged()));
        }
        ui->tabWidget->blockSignals(false);

        selectFile(Settings::lastFile(_folder->getKey()));
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
