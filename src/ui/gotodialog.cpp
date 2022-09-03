#include <QPushButton>
#include "gotodialog.h"
#include "icons.h"
#include "ui_gotodialog.h"
#include "medo/state.h"
#include "helpers.h"

GotoDialog::GotoDialog(QWidget* parent, Storage* storage) : QDialog(parent), ui(new Ui::GotoDialog) {
    ui->setupUi(this);
    Helpers::setupResizableDialog(this);
    ui->textSearch->installEventFilter(this);

    _storage = storage;

    connect(ui->textSearch, &QLineEdit::textEdited, this, &GotoDialog::onTextEdited);
    connect(ui->listWidget, &QListWidget::itemSelectionChanged, this, &GotoDialog::onItemSelectionChanged);
    connect(ui->listWidget, &QListWidget::itemActivated, this, &GotoDialog::onItemActivated);

    onTextEdited("");

    State::load(this);
}

GotoDialog::~GotoDialog() {
    delete ui;
}

void GotoDialog::hideEvent(QHideEvent* event) {
    State::save(this);
    QWidget::hideEvent(event);
}

bool GotoDialog::eventFilter(QObject* obj, QEvent* event) {
    if (event->type() == QEvent::KeyPress) {
        QKeyEvent* keyEvent = static_cast<QKeyEvent*>(event);
        switch (keyEvent->key()) {
            case Qt::Key_Up:
                if (ui->listWidget->currentRow() >= 0) {
                    if (ui->listWidget->currentRow() > 0) {
                        ui->listWidget->setCurrentRow(ui->listWidget->currentRow() - 1);
                    }
                } else if (ui->listWidget->count() > 0) {
                    ui->listWidget->setCurrentRow(0);
                }
                return true;

            case Qt::Key_Down:
                if (ui->listWidget->currentRow() >= 0) {
                    if (ui->listWidget->currentRow() < ui->listWidget->count() - 1) {
                        ui->listWidget->setCurrentRow(ui->listWidget->currentRow() + 1);
                    }
                } else if (ui->listWidget->count() > 0) {
                    ui->listWidget->setCurrentRow(0);
                }
                return true;
        }
    }
    return QObject::eventFilter(obj, event);
}

void GotoDialog::accept() {
    if (ui->listWidget->selectedItems().count() > 0) {
        auto selectedItem = ui->listWidget->selectedItems().first();
        auto keys = selectedItem->data(Qt::UserRole).toString().split('\0');
        _folderKey = keys[0];
        _fileKey = (keys.count() == 2) ? keys[1] : "";
        QDialog::accept();
    }
}


void GotoDialog::onTextEdited(const QString& text) {
    QList<QListWidgetItem*> items;

    if (text.length() == 0) {
        auto folder = _storage->folderAt(0);
        QListWidgetItem* item = new QListWidgetItem(Icons::gotoFolder(), folder->title());
        item->setData(Qt::UserRole, folder->name());
        items.push_back(item);
    } else {
        QString cleanedText = Helpers::getTextWithoutAccents(text);

        for (FolderItem* folder : *_storage) {
            auto folderTitle = folder->title();
            auto cleanedFolderTitle = Helpers::getTextWithoutAccents(folderTitle);
            if (folderTitle.contains(text, Qt::CaseInsensitive) || cleanedFolderTitle.contains(cleanedText, Qt::CaseInsensitive)) {
                QString title = folderTitle;
                if (!folder->isPrimary() && !folder->isRoot()) {
                    title += " in " + folder->rootFolder()->title();
                }
                QListWidgetItem* item = new QListWidgetItem(Icons::gotoFolder(), title);
                item->setData(Qt::UserRole, folder->name());
                items.push_back(item);
            }

            for (FileItem* file : *folder) {
                auto fileTitle = file->title();
                auto cleanedFileTitle = Helpers::getTextWithoutAccents(fileTitle);
                if (fileTitle.contains(text, Qt::CaseInsensitive) || cleanedFileTitle.contains(cleanedText, Qt::CaseInsensitive)) {
                    QString title = fileTitle;
                    if (folder->isPrimary()) {
                        title += " in " + folder->title();
                    } else { //secondary data path
                        title += " in " + folder->title();
                        if (!folder->isRoot()) { title += " " + folder->rootFolder()->title(); }
                    }
                    QListWidgetItem* item = new QListWidgetItem(Icons::gotoFile(), title);
                    item->setData(Qt::UserRole, folder->name() + '\0' + file->name());
                    items.push_back(item);
                }
            }
        }
    }

    std::sort(items.begin(), items.end(), [this] (const QListWidgetItem * item1, const QListWidgetItem * item2) {
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
