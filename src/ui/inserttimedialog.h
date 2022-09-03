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
        QString formattedTime() const { return _formattedTime; }

    protected:
        void accept();

    private:
        Ui::InsertTimeDialog* ui;
        QString _formattedTime;
        QTimer* updateExample = nullptr;
        QString getFormattedText(QString format = QString());

    private slots:
        void onTextEdited(const QString& text);
        void onUpdateExampleTimeout();

};
