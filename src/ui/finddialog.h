#ifndef FINDDIALOG_H
#define FINDDIALOG_H

#include <QDialog>

namespace Ui {
    class FindDialog;
}

class FindDialog : public QDialog {
    Q_OBJECT

    public:
        explicit FindDialog(QWidget *parent, QString searchText, bool matchCase);
        ~FindDialog();
        QString searchText();
        bool matchCase();

    private:
        Ui::FindDialog *ui;

    private slots:
        void onTextChanged(const QString &text);

};

#endif // FINDDIALOG_H
