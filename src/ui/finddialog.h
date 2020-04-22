#ifndef FINDDIALOG_H
#define FINDDIALOG_H

#include <QDialog>

namespace Ui {
    class FindDialog;
}

class FindDialog : public QDialog {
    Q_OBJECT

    public:
        explicit FindDialog(QWidget *parent, QString searchText, bool matchCase, bool wholeWord, bool useRegEx);
        ~FindDialog();
        QString searchText();
        bool matchCase();
        bool wholeWord();
        bool useRegEx();

    private:
        Ui::FindDialog *ui;

    private slots:
        void onStateChanged();

};

#endif // FINDDIALOG_H
