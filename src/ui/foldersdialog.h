#pragma once

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
        FolderItem* selectedFolder() const { return _selectedFolder; }

    protected:
        void keyPressEvent(QKeyEvent* event);
        void hideEvent(QHideEvent* event);

    private:
        Ui::FoldersDialog* ui;
        QPushButton* _newButton;
        QPushButton* _renameButton;
        QPushButton* _deleteButton;
        Storage* _storage = nullptr;
        FolderItem* _selectedFolder = nullptr;
        void fillList();

    private slots:
        void onNew();
        void onRename();
        void onDelete();
        void onCurrentItemChanged(QListWidgetItem* current);
        void onItemChanged(QListWidgetItem* item);
        void onItemDoubleClicked(QListWidgetItem* item);

};
