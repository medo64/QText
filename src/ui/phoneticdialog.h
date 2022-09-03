#pragma once

#include <QDialog>

namespace Ui {
    class PhoneticDialog;
}

class PhoneticDialog : public QDialog {
        Q_OBJECT

    public:
        explicit PhoneticDialog(QWidget* parent, QString text);
        ~PhoneticDialog();

    private:
        Ui::PhoneticDialog* ui;

    private slots:
        void onChanged();

};
