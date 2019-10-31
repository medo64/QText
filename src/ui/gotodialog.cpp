#include <QPushButton>
#include "gotodialog.h"
#include "ui_gotodialog.h"

GotoDialog::GotoDialog(QWidget *parent, Storage* storage) : QDialog(parent), ui(new Ui::GotoDialog) {
    ui->setupUi(this);
    ui->textSearch->installEventFilter(this);

    _storage = storage;

    _fileIcon.addFile(":icons/16x16/file.png", QSize(16, 16));
    _fileIcon.addFile(":icons/24x24/file.png", QSize(24, 24));
    _fileIcon.addFile(":icons/32x32/file.png", QSize(32, 32));
    _fileIcon.addFile(":icons/48x48/file.png", QSize(48, 48));
    _fileIcon.addFile(":icons/64x64/file.png", QSize(64, 64));

    _folderIcon.addFile(":icons/16x16/folder.png", QSize(16, 16));
    _folderIcon.addFile(":icons/24x24/folder.png", QSize(24, 24));
    _folderIcon.addFile(":icons/32x32/folder.png", QSize(32, 32));
    _folderIcon.addFile(":icons/48x48/folder.png", QSize(48, 48));
    _folderIcon.addFile(":icons/64x64/folder.png", QSize(64, 64));

    connect(ui->textSearch, SIGNAL(textEdited(const QString&)), SLOT(onTextEdited(const QString&)));
    connect(ui->listWidget, SIGNAL(itemSelectionChanged()), SLOT(onItemSelectionChanged()));
    connect(ui->listWidget, SIGNAL(itemActivated(QListWidgetItem*)), SLOT(onItemActivated()));

    onTextEdited("");
}

GotoDialog::~GotoDialog() {
    delete ui;
}

bool GotoDialog::eventFilter(QObject* obj, QEvent* event) {
    if (event->type() == QEvent::KeyPress) {
        QKeyEvent *keyEvent = static_cast<QKeyEvent *>(event);
        switch(keyEvent->key()) {
            case Qt::Key_Up: {
                if (ui->listWidget->currentRow() >= 0) {
                    if (ui->listWidget->currentRow() > 0) {
                        ui->listWidget->setCurrentRow(ui->listWidget->currentRow() - 1);
                    }
                } else if (ui->listWidget->count() > 0) {
                    ui->listWidget->setCurrentRow(0);
                }
            } return true;

            case Qt::Key_Down: {
                if (ui->listWidget->currentRow() >= 0) {
                    if (ui->listWidget->currentRow() < ui->listWidget->count() - 1) {
                        ui->listWidget->setCurrentRow(ui->listWidget->currentRow() + 1);
                    }
                } else if (ui->listWidget->count() > 0) {
                    ui->listWidget->setCurrentRow(0);
                }
            } return true;
        }
    }
    return QObject::eventFilter(obj, event);
}

void GotoDialog::accept() {
    if (ui->listWidget->selectedItems().count() > 0) {
        auto selectedItem = ui->listWidget->selectedItems().first();
        auto keys = selectedItem->data(Qt::UserRole).toString().split('\0');
        FolderKey = keys[0];
        FileKey = (keys.count() == 2) ? keys[1] : "";
        QDialog::accept();
    }
}


void GotoDialog::onTextEdited(const QString& text) {
    QList<QListWidgetItem*> items;

    if (text.length() == 0) {
        auto folder = _storage->getFolder(0);
        QListWidgetItem* item = new QListWidgetItem(_folderIcon, folder->getTitle());
        item->setData(Qt::UserRole, folder->getKey());
        items.push_back(item);
    } else {
        for (int i = 0; i < _storage->folderCount(); i++) {
            auto folder = _storage->getFolder(i);
            auto folderTitle = folder->getTitle();
            if (folderTitle.contains(text, Qt::CaseInsensitive)) {
                QListWidgetItem* item = new QListWidgetItem(_folderIcon, folderTitle);
                item->setData(Qt::UserRole, folder->getKey());
                items.push_back(item);
            }

            for (int j = 0; j < folder->fileCount(); j++) {
                auto file = folder->getFile(j);
                auto fileTitle = file->getTitle();
                if (fileTitle.contains(text, Qt::CaseInsensitive)) {
                    QString newTitle = fileTitle + " " + (i>0 ? "(in " + folderTitle + ")" : folderTitle);
                    QListWidgetItem* item = new QListWidgetItem(_fileIcon, newTitle);
                    item->setData(Qt::UserRole, folder->getKey() + '\0' + file->getKey());
                    items.push_back(item);
                }
            }
        }
    }

    std::sort(items.begin(), items.end(), [this] (const QListWidgetItem* item1, const QListWidgetItem* item2) {
        auto title = ui->textSearch->text();
        auto title1 = item1->text();
        auto title2 = item2->text();
        auto startsWith1 = title1.startsWith(title, Qt::CaseInsensitive);
        auto startsWith2 = title2.startsWith(title, Qt::CaseInsensitive);
        auto isFolder1 = item1->data(Qt::UserRole).toString().contains('\0');
        auto isFolder2 = item2->data(Qt::UserRole).toString().contains('\0');
        auto folderKey1 = item1->data(Qt::UserRole).toString().split('\0')[0];
        auto folderKey2 = item2->data(Qt::UserRole).toString().split('\0')[0];

        if (startsWith1 && !startsWith2) { //sort matching prefixes first
            return true;
        } else if (!startsWith1 && startsWith2) {
            return false;
        } else {
            if (isFolder1 && !isFolder2) { //sort folders last
                return true;
            } else if (!isFolder1 && isFolder2) {
                return false;
            } else {
                auto compareResult = title1.compare(title2, Qt::CaseInsensitive);
                if (compareResult == 0) { //sort by folder key only if all other is same
                    return folderKey1.compare(folderKey2, Qt::CaseSensitive) < 0;
                } else {
                    return (compareResult < 0);
                }
            }
        }
    });

    ui->listWidget->clear();
    foreach (QListWidgetItem* item, items) {
        ui->listWidget->addItem(item);
    }

    bool hasAny = ui->listWidget->count() > 0;
    if (hasAny) { ui->listWidget->setCurrentRow(0); }
    ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(hasAny);
}

void GotoDialog::onItemSelectionChanged() {
    bool hasAnySelected = ui->listWidget->selectedItems().count() > 0;
    ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(hasAnySelected);
}

void GotoDialog::onItemActivated() {
    accept();
}
