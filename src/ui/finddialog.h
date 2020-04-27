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

    protected:
        void accept() override;

    private:
        Ui::FindDialog *ui;
        QString composeTerm(QString text, bool matchCase, bool wholeWord, bool useRegEx);
        QString decomposeTerm(QString term, bool* matchCase, bool* wholeWord, bool* useRegEx);
        QString decomposeTerm(QString term);

    private slots:
        void onStateChanged();
        void onHistorySelected();

};

#endif // FINDDIALOG_H
