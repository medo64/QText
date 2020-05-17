#pragma once

#include <QDialog>

namespace Ui {
    class InsertTimeDialog;
}

class InsertTimeDialog : public QDialog {
        Q_OBJECT

    public:
        explicit InsertTimeDialog(QWidget* parent = nullptr);
        ~InsertTimeDialog();
        QString FormattedTime;

    protected:
        void accept();

    private:
        Ui::InsertTimeDialog* ui;
        QTimer* updateExample = nullptr;
        QString getFormattedText(QString format = QString());

    private slots:
        void onTextEdited(const QString& text);
        void onUpdateExampleTimeout();

};
