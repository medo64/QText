#include "foldersdialog.h"
#include "ui_foldersdialog.h"
#include "medo/state.h"
#include "helpers.h"
#include "settings.h"

#include <QMessageBox>

FoldersDialog::FoldersDialog(QWidget* parent, Storage* storage, FolderItem* selectedFolder) : QDialog(parent), ui(new Ui::FoldersDialog) {
    ui->setupUi(this);
    Helpers::setupResizableDialog(this);

    _storage = storage;
    _selectedFolder = selectedFolder;

    _newButton = ui->buttonBox->addButton("&New", QDialogButtonBox::ResetRole);
    connect(_newButton, &QAbstractButton::clicked, this, &FoldersDialog::onNew);

    _renameButton = ui->buttonBox->addButton("&Rename", QDialogButtonBox::ResetRole);
    _renameButton->setEnabled(false);
    connect(_renameButton, &QAbstractButton::clicked, this, &FoldersDialog::onRename);

    _deleteButton = ui->buttonBox->addButton("&Delete", QDialogButtonBox::ResetRole);
    _deleteButton->setEnabled(false);
    connect(_deleteButton, &QAbstractButton::clicked, this, &FoldersDialog::onDelete);

    if (storage != nullptr) {
        fillList();
        connect(ui->listWidget, &QListWidget::currentItemChanged, this, &FoldersDialog::onCurrentItemChanged);
        connect(ui->listWidget, &QListWidget::itemChanged, this,  &FoldersDialog::onItemChanged);
        connect(ui->listWidget, &QListWidget::itemDoubleClicked, this,  &FoldersDialog::onItemDoubleClicked);
    }
    onCurrentItemChanged(ui->listWidget->currentItem());

    State::load(this);
}

FoldersDialog::~FoldersDialog() {
    delete ui;
}

void FoldersDialog::hideEvent(QHideEvent* event) {
    State::save(this);
    QWidget::hideEvent(event);
}

void FoldersDialog::keyPressEvent(QKeyEvent* event) {
    auto data = static_cast<uint>(event->key()) | event->modifiers();
    switch (data) {
        case Qt::Key_Escape:
            close();
            break;

        case Qt::AltModifier | Qt::Key_F8:
            if (Helpers::openWithVSCodeAvailable()) {
                Helpers::openDirectoriesWithVSCode(Settings::dataPaths());
            }
            break;
    }
}


void FoldersDialog::onNew() {
    FolderItem* folder = _storage->newFolder("New folder");
    if (folder != nullptr) {
        ui->listWidget->clear();
        _selectedFolder = folder;
        fillList();
        if (ui->listWidget->currentItem() != nullptr) {
            ui->listWidget->editItem(ui->listWidget->currentItem());
        }
    }
}

void FoldersDialog::onRename() {
    ui->listWidget->editItem(ui->listWidget->currentItem());
}

void FoldersDialog::onDelete() {
    QListWidgetItem* item = ui->listWidget->currentItem();
    if (item != nullptr) {
        QVariant data = item->data(Qt::UserRole);
        FolderItem* folder = static_cast<FolderItem*>(data.value<void*>());
        if (folder->fileCount() > 0) {
            QMessageBox msgBox(this);
            msgBox.setText("This folder is not empty!");
            msgBox.setInformativeText("Do you really want to delete this folder?");
            msgBox.setStandardButtons(QMessageBox::Yes | QMessageBox::No);
            msgBox.setDefaultButton(QMessageBox::No);
            if (msgBox.exec() != QMessageBox::Yes) { return; }
        }
        if (_selectedFolder == folder) { _selectedFolder = _storage->getBaseFolder(); }
        if (_storage->deleteFolder(folder)) {
            QListWidgetItem* itemToDelete = ui->listWidget->takeItem(ui->listWidget->currentRow());
            delete itemToDelete;
        }
    }
}


void FoldersDialog::onCurrentItemChanged(QListWidgetItem* current) {
    bool isEditableItem = (current != nullptr) && (current->flags() & Qt::ItemIsEditable);
    _renameButton->setEnabled(isEditableItem);
    _deleteButton->setEnabled(isEditableItem);
}

void FoldersDialog::onItemChanged(QListWidgetItem* item) {
    if (item != nullptr) {
        QVariant data = item->data(Qt::UserRole);
        FolderItem* folder = static_cast<FolderItem*>(data.value<void*>());
        if (!folder->rename(item->text())) {
            item->setText(folder->getTitle()); //restore text if rename was unsuccessful
        }
    }
}

void FoldersDialog::onItemDoubleClicked(QListWidgetItem* item) {
    if (item != nullptr) {
        QVariant data = item->data(Qt::UserRole);
        _selectedFolder = static_cast<FolderItem*>(data.value<void*>());
        accept();
    } else {
        close();
    }
}


FolderItem* FoldersDialog::selectedFolder() {
    return _selectedFolder;
}


void FoldersDialog::fillList() {
    QFont italicFont = ui->listWidget->font();
    italicFont.setItalic(true);
    for (int i = 0; i < _storage->folderCount(); i++) {
        FolderItem* folder = _storage->getFolder(i);
        QListWidgetItem* item = new QListWidgetItem();
        item->setText(folder->getTitle());
        if (!folder->isPrimary()) { item->setFont(italicFont); }
        item->setData(Qt::UserRole, QVariant::fromValue(static_cast<void*>(folder)));
        if (!folder->isRoot()) { item->setFlags(item->flags() | Qt::ItemIsEditable); }
        ui->listWidget->addItem(item);
        if (folder == _selectedFolder) {
            ui->listWidget->setCurrentItem(item);
        }
    }
}
