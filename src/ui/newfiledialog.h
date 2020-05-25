#pragma once

#include <QDialog>
#include <QString>
#include <QWidget>
#include "storage/fileitem.h"
#include "storage/folderitem.h"

namespace Ui {
    class NewFileDialog;
}

class NewFileDialog : public QDialog {
        Q_OBJECT

    public:
        explicit NewFileDialog(QWidget* parent, FolderItem* folder);
        ~NewFileDialog();

    public:
        QString title();
        FileType type();

    private:
        Ui::NewFileDialog* ui;

    private:
        FolderItem* _folder = nullptr; //used for new file

    private slots:
        void onChanged();

};
