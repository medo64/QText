#ifndef FOLDERSDIALOG_H
#define FOLDERSDIALOG_H

#include <QAbstractButton>
#include <QPushButton>
#include <QDialog>
#include <QListWidgetItem>
#include <folderitem.h>
#include <storage.h>

namespace Ui {
    class FoldersDialog;
}

class FoldersDialog : public QDialog {
    Q_OBJECT

    public:
        explicit FoldersDialog(QWidget* parent, Storage* storage, FolderItem* selectedFolder);
        ~FoldersDialog();
        FolderItem* selectedFolder();

    private:
        Ui::FoldersDialog* ui;
        QPushButton* _newButton;
        QPushButton* _renameButton;
        QPushButton* _deleteButton;
        Storage* _storage = nullptr;
        FolderItem* _selectedFolder = nullptr;
        void fillList();
        void onNew();
        void onRename();
        void onDelete();
        void onCurrentItemChanged(QListWidgetItem* current);
        void onItemChanged(QListWidgetItem* item);
        void onItemDoubleClicked(QListWidgetItem* item);

};

#endif // FOLDERSDIALOG_H
