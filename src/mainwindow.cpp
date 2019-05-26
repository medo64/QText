#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "storage.h"
#include <QTextEdit>

MainWindow::MainWindow(std::shared_ptr<Storage> storage) : QMainWindow(nullptr), ui(new Ui::MainWindow) {
    ui->setupUi(this);

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

    ui->tabWidget->clear();
    auto folder = storage->getBaseFolder();
    for (size_t i = 0; i < folder->fileCount(); i++) {
        auto file = folder->getFile(i);
        ui->tabWidget->addTab(file, file->getTitle());
    }
}

MainWindow::~MainWindow() {
    delete ui;
}
