#ifndef FILENAMEDIALOG_H
#define FILENAMEDIALOG_H

#include <QDialog>
#include <QString>

namespace Ui {
    class FileNameDialog;
}

class FileNameDialog : public QDialog {
    Q_OBJECT

    public:
        explicit FileNameDialog(QWidget *parent, QString fileName);
        ~FileNameDialog();
        QString getFileName();

    private:
        Ui::FileNameDialog *ui;

};

#endif // FILENAMEDIALOG_H
