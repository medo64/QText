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
        explicit FileNameDialog(QWidget *parent, QString fileName, std::shared_ptr<FolderItem> folder);
        ~FileNameDialog();
        QString getFileName();

    private:
        Ui::FileNameDialog *ui;
        std::shared_ptr<FolderItem> _folder;

    private slots:
        void onTextEdited(const QString &text);

};

#endif // FILENAMEDIALOG_H
