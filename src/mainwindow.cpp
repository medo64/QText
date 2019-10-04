#include <QClipboard>
#include <QMenu>
#include <QMessageBox>
#include <QMimeData>
#include <QTabBar>
#include <QTextDocumentFragment>
#include "filenamedialog.h"
#include "helpers.h"
#include "settings.h"
#include "storage.h"
#include "ui_filenamedialog.h"
#include "ui_mainwindow.h"
#include "medo/config.h"
#include "medo/singleinstance.h"
#include "medo/state.h"
#include "mainwindow.h"

MainWindow::MainWindow(std::shared_ptr<Storage> storage) : QMainWindow(nullptr), ui(new Ui::MainWindow) {
    ui->setupUi(this);

    _storage = storage;
    _folder = storage->getBaseFolder();

    //taskbar visibility
    if (!Settings::showInTaskbar()) {
        this->setWindowFlag(Qt::Tool);
    }

    { //application icon
        QIcon appIcon;
        appIcon.addFile(":icons/16x16/qtext.png", QSize(16, 16));
        appIcon.addFile(":icons/32x32/qtext.png", QSize(32, 32));
        appIcon.addFile(":icons/48x48/qtext.png", QSize(48, 48));
        appIcon.addFile(":icons/64x64/qtext.png", QSize(64, 64));
        this->setWindowIcon(appIcon);
    }

    { //tray
        QIcon trayIcon;
        if ((QSysInfo::kernelType() == "winnt") && (QSysInfo::productVersion() == "10")) {
            trayIcon.addFile(":icons/16x16/tray-white.png", QSize(16, 16));
            trayIcon.addFile(":icons/32x32/tray-white.png", QSize(32, 32));
            trayIcon.addFile(":icons/48x48/tray-white.png", QSize(48, 48));
            trayIcon.addFile(":icons/64x64/tray-white.png", QSize(64, 64));
        } else {
            trayIcon.addFile(":icons/16x16/tray-color.png", QSize(16, 16));
            trayIcon.addFile(":icons/32x32/tray-color.png", QSize(32, 32));
            trayIcon.addFile(":icons/48x48/tray-color.png", QSize(48, 48));
            trayIcon.addFile(":icons/64x64/tray-color.png", QSize(64, 64));
        }
        _tray = new QSystemTrayIcon(this);
        connect(_tray, SIGNAL(activated(QSystemTrayIcon::ActivationReason)), this, SLOT(onTrayActivate(QSystemTrayIcon::ActivationReason)));

        QMenu* trayMenu = new QMenu(this);
        auto defaultAction = trayMenu->addAction("&Show", this, SLOT(onTrayShow()));
        trayMenu->addSeparator();
        trayMenu->addAction("E&xit", this, SLOT(onTrayExit()));

        auto font = defaultAction->font();
        font.setBold(true);
        defaultAction->setFont(font);
        _tray->setContextMenu(trayMenu);

        _tray->setIcon(trayIcon);
        auto hotkeyText = Settings::hotkey().toString(QKeySequence::PortableText);
        if (hotkeyText.length() > 0) {
            _tray->setToolTip( + "Access notes from tray or press " + hotkeyText + " hotkey.");
        } else {
            _tray->setToolTip( + "Access notes from tray.");
        }
        _tray->show();
    }

    { //hotkey
        _hotkey = new Hotkey();
        _hotkey->registerHotkey(Settings::hotkey());
        connect(_hotkey, SIGNAL(activated()), this, SLOT(onTrayShow()));
    }

    { //single instance
        connect(SingleInstance::instance(), SIGNAL(newInstanceDetected()), this, SLOT(onTrayShow()));
    }

    { //icon setup
        QIcon newIcon;
        newIcon.addFile(":icons/16x16/new.png", QSize(16, 16));
        newIcon.addFile(":icons/24x24/new.png", QSize(24, 24));
        newIcon.addFile(":icons/32x32/new.png", QSize(32, 32));
        newIcon.addFile(":icons/48x48/new.png", QSize(48, 48));
        newIcon.addFile(":icons/64x64/new.png", QSize(64, 64));
        ui->actionNew->setIcon(newIcon);
        connect(ui->actionNew, SIGNAL(triggered()), this, SLOT(onFileNew()));

        QIcon saveIcon;
        saveIcon.addFile(":icons/16x16/save.png", QSize(16, 16));
        saveIcon.addFile(":icons/24x24/save.png", QSize(24, 24));
        saveIcon.addFile(":icons/32x32/save.png", QSize(32, 32));
        saveIcon.addFile(":icons/48x48/save.png", QSize(48, 48));
        saveIcon.addFile(":icons/64x64/save.png", QSize(64, 64));
        ui->actionSave->setIcon(saveIcon);
        connect(ui->actionSave, SIGNAL(triggered()), this, SLOT(onFileSave()));

        QIcon renameIcon;
        renameIcon.addFile(":icons/16x16/rename.png", QSize(16, 16));
        renameIcon.addFile(":icons/24x24/rename.png", QSize(24, 24));
        renameIcon.addFile(":icons/32x32/rename.png", QSize(32, 32));
        renameIcon.addFile(":icons/48x48/rename.png", QSize(48, 48));
        renameIcon.addFile(":icons/64x64/rename.png", QSize(64, 64));
        ui->actionRename->setIcon(renameIcon);
        connect(ui->actionRename, SIGNAL(triggered()), this, SLOT(onFileRename()));

        QIcon deleteIcon;
        deleteIcon.addFile(":icons/16x16/delete.png", QSize(16, 16));
        deleteIcon.addFile(":icons/24x24/delete.png", QSize(24, 24));
        deleteIcon.addFile(":icons/32x32/delete.png", QSize(32, 32));
        deleteIcon.addFile(":icons/48x48/delete.png", QSize(48, 48));
        deleteIcon.addFile(":icons/64x64/delete.png", QSize(64, 64));
        ui->actionDelete->setIcon(deleteIcon);
        connect(ui->actionDelete, SIGNAL(triggered()), this, SLOT(onFileDelete()));

        QIcon cutIcon;
        cutIcon.addFile(":icons/16x16/cut.png", QSize(16, 16));
        cutIcon.addFile(":icons/24x24/cut.png", QSize(24, 24));
        cutIcon.addFile(":icons/32x32/cut.png", QSize(32, 32));
        cutIcon.addFile(":icons/48x48/cut.png", QSize(48, 48));
        cutIcon.addFile(":icons/64x64/cut.png", QSize(64, 64));
        ui->actionCut->setIcon(cutIcon);
        connect(ui->actionCut, SIGNAL(triggered()), this, SLOT(onTextCut()));

        QIcon copyIcon;
        copyIcon.addFile(":icons/16x16/copy.png", QSize(16, 16));
        copyIcon.addFile(":icons/24x24/copy.png", QSize(24, 24));
        copyIcon.addFile(":icons/32x32/copy.png", QSize(32, 32));
        copyIcon.addFile(":icons/48x48/copy.png", QSize(48, 48));
        copyIcon.addFile(":icons/64x64/copy.png", QSize(64, 64));
        ui->actionCopy->setIcon(copyIcon);
        connect(ui->actionCopy, SIGNAL(triggered()), this, SLOT(onTextCopy()));

        QIcon pasteIcon;
        pasteIcon.addFile(":icons/16x16/paste.png", QSize(16, 16));
        pasteIcon.addFile(":icons/24x24/paste.png", QSize(24, 24));
        pasteIcon.addFile(":icons/32x32/paste.png", QSize(32, 32));
        pasteIcon.addFile(":icons/48x48/paste.png", QSize(48, 48));
        pasteIcon.addFile(":icons/64x64/paste.png", QSize(64, 64));
        ui->actionPaste->setIcon(pasteIcon);
        connect(ui->actionPaste, SIGNAL(triggered()), this, SLOT(onTextPaste()));

        QIcon undoIcon;
        undoIcon.addFile(":icons/16x16/undo.png", QSize(16, 16));
        undoIcon.addFile(":icons/24x24/undo.png", QSize(24, 24));
        undoIcon.addFile(":icons/32x32/undo.png", QSize(32, 32));
        undoIcon.addFile(":icons/48x48/undo.png", QSize(48, 48));
        undoIcon.addFile(":icons/64x64/undo.png", QSize(64, 64));
        ui->actionUndo->setIcon(undoIcon);
        connect(ui->actionUndo, SIGNAL(triggered()), this, SLOT(onTextUndo()));

        QIcon redoIcon;
        redoIcon.addFile(":icons/16x16/redo.png", QSize(16, 16));
        redoIcon.addFile(":icons/24x24/redo.png", QSize(24, 24));
        redoIcon.addFile(":icons/32x32/redo.png", QSize(32, 32));
        redoIcon.addFile(":icons/48x48/redo.png", QSize(48, 48));
        redoIcon.addFile(":icons/64x64/redo.png", QSize(64, 64));
        ui->actionRedo->setIcon(redoIcon);
        connect(ui->actionRedo, SIGNAL(triggered()), this, SLOT(onTextRedo()));
    }

    QWidget* spacerWidget = new QWidget(this);
    spacerWidget->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);
    spacerWidget->setVisible(true);
    ui->mainToolBar->addWidget(spacerWidget);

    onFolderSelect(); //setup folder select menu button
    onTabChanged(); //update toolbar & focus

    connect(ui->actionReopen, SIGNAL(triggered()), this, SLOT(onFileReopen()));
    connect(ui->actionShowContainingDirectory, SIGNAL(triggered()), this, SLOT(onShowContainingDirectory()));
    connect(ui->actionShowContainingDirectoryOnly, SIGNAL(triggered()), this, SLOT(onShowContainingDirectoryOnly()));

    ui->tabWidget->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(ui->tabWidget, SIGNAL(customContextMenuRequested(const QPoint&)), SLOT(onTabMenuRequested(const QPoint&)));

    ui->tabWidget->tabBar()->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(ui->tabWidget->tabBar(), SIGNAL(customContextMenuRequested(const QPoint&)), SLOT(onTabMenuRequested(const QPoint&)));

    connect(ui->tabWidget, SIGNAL(currentChanged(int)), SLOT(onTabChanged()));
    connect(_clipboard, SIGNAL(dataChanged()), SLOT(onTextStateChanged()));

    { //State
        connect(State::instance(), &State::writeToConfig, [=] (QString key, QString value) { Config::write("State!" + key, value); });
        connect(State::instance(), &State::readFromConfig, [=] (QString key) { return Config::read("State!" + key); });
        State::load(this);
    }

    if (!Settings::setupCompleted()) { // show extra info when ran first time
        _tray->showMessage(QCoreApplication::applicationName(), _tray->toolTip(), QSystemTrayIcon::Information, 2500);
    }
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
        }
    }
}

void MainWindow::keyPressEvent(QKeyEvent *event) {
    auto data = static_cast<uint>(event->key()) | event->modifiers();
    switch(data) {
        case Qt::Key_Escape: {
            State::save(this);
            this->hide();
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
    auto dialog = std::make_shared<FileNameDialog>(this, nullptr, _folder);
    switch (dialog->exec()) {
        case QDialog::Accepted:
            {
                auto newTitle = dialog->getFileName();
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

    auto dialog = std::make_shared<FileNameDialog>(this, file->getTitle(), _folder);
    switch (dialog->exec()) {
        case QDialog::Accepted:
            {
                auto newTitle = dialog->getFileName();
                file->setTitle(newTitle);
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
    auto cursor = file->textCursor();
    if (cursor.hasSelection()) {
        _clipboard->clear();
        _clipboard->setText(cursor.selection().toPlainText());

        cursor.removeSelectedText();
    }
}

void MainWindow::onTextCopy() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    auto cursor = file->textCursor();
    if (cursor.hasSelection()) {
        _clipboard->clear();
        _clipboard->setText(cursor.selection().toPlainText());
    }
}

void MainWindow::onTextPaste() {
    auto data = _clipboard->mimeData();
    if (data->hasText()) {
        auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
        auto cursor = file->textCursor();
        if (cursor.hasSelection()) { cursor.removeSelectedText(); }
        cursor.insertText(data->text());
    }
}

void MainWindow::onTextUndo() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->document()->undo();
}

void MainWindow::onTextRedo() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->document()->redo();
}


void MainWindow::onFolderSelect() {
    QAction *action = qobject_cast<QAction *>(sender());

    if (action != nullptr) {
        auto data = action->data();
        auto path = action->data().value<QString>();
        for (size_t i=0; i<_storage->folderCount(); i++) {
            auto folder = _storage->getFolder(i);
            if (folder->getPath().compare(path, Qt::CaseSensitive) == 0) {
                _folder->saveAll(); //save all files in previous folder / just in case
                _folder = folder;
                break;
            }
        }
    }

    if (_folderButton == nullptr) {
        _folderButton = new QToolButton();
        _folderButton->setPopupMode(QToolButton::InstantPopup);
        _folderButton->setMenu(new QMenu());
        ui->mainToolBar->addWidget(_folderButton);
    }

    _folderButton->setText(_folder->getTitle());
    _folderButton->menu()->clear();
    for(size_t i=0; i<_storage->folderCount(); i++) {
        auto folder = _storage->getFolder(i);
        QAction* folderAction = new QAction(folder->getTitle());
        folderAction->setData(folder->getPath());
        folderAction->setDisabled(folder == _folder);
        connect(folderAction, SIGNAL(triggered()), this, SLOT(onFolderSelect()));
        _folderButton->menu()->addAction(folderAction);
    }

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
    onTabChanged();
}

void onShowContainingDirectory2(QString directoryPath, QString filePath) {
    Helpers::showInFileManager(directoryPath, filePath);
}

void MainWindow::onShowContainingDirectory() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    onShowContainingDirectory2(_folder->getPath(), file->getPath());
}

void MainWindow::onShowContainingDirectoryOnly() {
    onShowContainingDirectory2(_folder->getPath(), nullptr);
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
    } else {
        menu.addSeparator();
        menu.addAction(ui->actionShowContainingDirectoryOnly);
    }

    menu.exec(tabbar->mapToGlobal(point));
}

void MainWindow::onTabChanged() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    bool exists = (file != nullptr);

    ui->actionSave->setDisabled(!exists || file->isModified());
    ui->actionRename->setDisabled(ui->tabWidget->count() == 0);

    onTextStateChanged();

    if (exists) { file->setFocus(); }
}

void MainWindow::onTextStateChanged() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    bool isSelectionAvailable = (file != nullptr) ? file->textCursor().hasSelection() : false;

    auto document = (file != nullptr) ? file->document() : nullptr;
    bool isUndoAvailable = (document !=nullptr) ? document->isUndoAvailable() : false;
    bool isRedoAvailable = (document !=nullptr) ? document->isRedoAvailable() : false;

    auto mimeData = _clipboard->mimeData();
    bool isClipboardTextAvailable = mimeData->hasText();

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

void MainWindow::onTrayExit() {
    _tray->hide();
    this->close();
    QCoreApplication::exit(0);
}
