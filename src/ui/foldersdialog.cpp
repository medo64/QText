#include "foldersdialog.h"
#include "ui_foldersdialog.h"

#include <QMessageBox>

FoldersDialog::FoldersDialog(QWidget* parent, Storage* storage, FolderItem* selectedFolder) :
    QDialog(parent),
    ui(new Ui::FoldersDialog) {
    ui->setupUi(this);

    _renameButton = ui->buttonBox->addButton("&Rename", QDialogButtonBox::ResetRole);
    _renameButton->setEnabled(false);
    connect(_renameButton, &QAbstractButton::clicked, this, &FoldersDialog::onRename);

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

void FoldersDialog::onCurrentItemChanged(QListWidgetItem* current) {
    _renameButton->setEnabled((current != nullptr) && (current->flags() & Qt::ItemIsEditable));
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
