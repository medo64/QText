#ifndef FILENAMEDIALOG_H
#define FILENAMEDIALOG_H

#include "folderitem.h"
#include <QDialog>
#include <QString>
#include <QWidget>

namespace Ui {
    class FileNameDialog;
}

class FileNameDialog : public QDialog {
    Q_OBJECT

    public:
        explicit FileNameDialog(QWidget *parent, FolderItem* folder);
        explicit FileNameDialog(QWidget *parent, FileItem* file);
        ~FileNameDialog();
        QString getTitle();

    private:
        explicit FileNameDialog(QWidget *parent);
        Ui::FileNameDialog *ui;
        FolderItem* _folder = nullptr; //used for new file
        FileItem* _file = nullptr; //used for rename - empty when new

    private slots:
        void onTextChanged(const QString &text);

};

#endif // FILENAMEDIALOG_H
