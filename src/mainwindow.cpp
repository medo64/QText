#include "filenamedialog.h"
#include "ui_filenamedialog.h"
#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "storage.h"
#include <QDebug>
#include <QMenu>
#include <QTabBar>
#include <QTextEdit>

MainWindow::MainWindow(std::shared_ptr<Storage> storage) : QMainWindow(nullptr), ui(new Ui::MainWindow) {
    ui->setupUi(this);
    _storage = storage;

    QIcon newIcon;
    newIcon.addFile(":icons/16x16/new.png", QSize(16, 16));
    newIcon.addFile(":icons/24x24/new.png", QSize(24, 24));
    newIcon.addFile(":icons/32x32/new.png", QSize(32, 32));
    newIcon.addFile(":icons/48x48/new.png", QSize(48, 48));
    newIcon.addFile(":icons/64x64/new.png", QSize(64, 64));
    ui->actionNew->setIcon(newIcon);

    QIcon saveIcon;
    saveIcon.addFile(":icons/16x16/save.png", QSize(16, 16));
    saveIcon.addFile(":icons/24x24/save.png", QSize(24, 24));
    saveIcon.addFile(":icons/32x32/save.png", QSize(32, 32));
    saveIcon.addFile(":icons/48x48/save.png", QSize(48, 48));
    saveIcon.addFile(":icons/64x64/save.png", QSize(64, 64));
    ui->actionSave->setIcon(saveIcon);

    QObject::connect(ui->actionNew, SIGNAL(triggered()), this, SLOT(onNew()));
    QObject::connect(ui->actionReopen, SIGNAL(triggered()), this, SLOT(onReopen()));
    QObject::connect(ui->actionSave, SIGNAL(triggered()), this, SLOT(onSave()));
    QObject::connect(ui->actionRename, SIGNAL(triggered()), this, SLOT(onRename()));

    ui->tabWidget->clear();
    auto folder = storage->getBaseFolder();
    for (size_t i = 0; i < folder->fileCount(); i++) {
        auto file = folder->getFile(i);
        ui->tabWidget->addTab(file, file->getTitle());
        QObject::connect(file, SIGNAL(titleChanged(FileItem*)), this, SLOT(onFileTitleChanged(FileItem*)));
        QObject::connect(file, SIGNAL(modificationChanged(FileItem*, bool)), this, SLOT(onFileModificationChanged(FileItem*, bool)));
        QObject::connect(file, SIGNAL(activated(FileItem*)), this, SLOT(onFileActivated(FileItem*)));
    }
    ui->actionSave->setDisabled(true);

    ui->tabWidget->tabBar()->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(ui->tabWidget->tabBar(), SIGNAL(customContextMenuRequested(const QPoint&)), SLOT(onTabMenuRequested(const QPoint&)));
}

MainWindow::~MainWindow() {
    delete ui;
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


void MainWindow::onNew() {
    auto dialog = std::make_shared<FileNameDialog>(this, nullptr);
    switch (dialog->exec()) {
        case QDialog::Accepted:
            {
                auto newTitle = dialog->getFileName();
                auto file = _storage->getBaseFolder()->newFile(newTitle);
                auto index = ui->tabWidget->addTab(file, file->getTitle());
                ui->tabWidget->setCurrentIndex(index);
                file->setFocus();
            }
            break;
        default:
            break;
    }
}

void MainWindow::onReopen() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->load();
    file->setFocus();
}

void MainWindow::onSave() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());
    file->save();
    file->setFocus();
}

void MainWindow::onRename() {
    auto file = dynamic_cast<FileItem*>(ui->tabWidget->currentWidget());

    auto dialog = std::make_shared<FileNameDialog>(this, file->getTitle());
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
    }
    menu.exec(tabbar->mapToGlobal(point));
}
