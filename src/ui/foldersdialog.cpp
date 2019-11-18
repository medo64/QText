#include "foldersdialog.h"
#include "ui_foldersdialog.h"

#include <QMessageBox>

FoldersDialog::FoldersDialog(QWidget* parent, Storage* storage, FolderItem* selectedFolder) :
    QDialog(parent),
    ui(new Ui::FoldersDialog) {
    ui->setupUi(this);

    _storage = storage;
    _selectedFolder = selectedFolder;

    _renameButton = ui->buttonBox->addButton("&Rename", QDialogButtonBox::ResetRole);
    _renameButton->setEnabled(false);
    connect(_renameButton, &QAbstractButton::clicked, this, &FoldersDialog::onRename);

    _deleteButton = ui->buttonBox->addButton("&Delete", QDialogButtonBox::ResetRole);
    _deleteButton->setEnabled(false);
    connect(_deleteButton, &QAbstractButton::clicked, this, &FoldersDialog::onDelete);

    QFont italicFont = ui->listWidget->font();
    italicFont.setItalic(true);

    if (storage != nullptr) {
        for (int i = 0; i < storage->folderCount(); i++) {
            FolderItem* folder = storage->getFolder(i);
            QListWidgetItem* item = new QListWidgetItem();
            item->setText(folder->getTitle());
            if (!folder->isPrimary()) { item->setFont(italicFont); }
            item->setData(Qt::UserRole, QVariant::fromValue(static_cast<void*>(folder)));
            if (!folder->isRoot()) { item->setFlags(item->flags() | Qt::ItemIsEditable); }
            ui->listWidget->addItem(item);
            if (folder == selectedFolder) {
                //item->setSelected(true);
                ui->listWidget->setCurrentItem(item);
            }
        }
        connect(ui->listWidget, &QListWidget::currentItemChanged, this, &FoldersDialog::onCurrentItemChanged);
        connect(ui->listWidget, &QListWidget::itemChanged, this,  &FoldersDialog::onItemChanged);
        connect(ui->listWidget, &QListWidget::itemDoubleClicked, this,  &FoldersDialog::onItemDoubleClicked);
    }
    onCurrentItemChanged(ui->listWidget->currentItem());
}

FoldersDialog::~FoldersDialog() {
    delete ui;
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
