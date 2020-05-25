#pragma once

#include <QDialog>
#include <QString>
#include <QWidget>
#include "storage/folderitem.h"

namespace Ui {
    class FileNameDialog;
}

class FileNameDialog : public QDialog {
        Q_OBJECT

    public:
        explicit FileNameDialog(QWidget* parent, FolderItem* folder);
        explicit FileNameDialog(QWidget* parent, FileItem* file);
        ~FileNameDialog();

    public:
        QString title();

    private:
        explicit FileNameDialog(QWidget* parent);
        Ui::FileNameDialog* ui;

    private:
        FolderItem* _folder = nullptr; //used for new file
        FileItem* _file = nullptr; //used for rename - empty when new

    private slots:
        void onTextChanged(const QString& text);

};
