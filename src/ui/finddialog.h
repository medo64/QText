#pragma once

#include <QDialog>
#include "find.h"

namespace Ui {
    class FindDialog;
}

class FindDialog : public QDialog {
        Q_OBJECT

    public:
        explicit FindDialog(QWidget* parent);
        ~FindDialog();

    public:
        QString searchText();
        bool matchCase();
        bool wholeWord();
        bool useRegEx();
        Find::SearchScope searchScope();

    protected:
        void accept() override;

    private:
        Ui::FindDialog* ui;

    private:
        QString composeTerm(QString text, bool matchCase, bool wholeWord, bool useRegEx, Find::SearchScope searchScope);
        QString decomposeTerm(QString term, bool* matchCase, bool* wholeWord, bool* useRegEx, Find::SearchScope* searchScope);
        QString decomposeTerm(QString term);

    private slots:
        void onStateChanged();
        void onHistorySelected();

};
