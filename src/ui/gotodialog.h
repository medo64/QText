#ifndef GOTODIALOG_H
#define GOTODIALOG_H

#include <QDialog>
#include <QIcon>
#include <QListWidget>
#include "storage.h"

namespace Ui {
    class GotoDialog;
}

class GotoDialog : public QDialog {
    Q_OBJECT

    public:
        explicit GotoDialog(QWidget *parent = nullptr,  Storage* storage = nullptr);
        ~GotoDialog();
        QString FolderKey;
        QString FileKey;

    protected:
        void accept();

    private:
        bool eventFilter(QObject* obj, QEvent* event);
        Ui::GotoDialog *ui;
        Storage* _storage;
        QIcon _folderIcon;
        QIcon _fileIcon;

    private slots:
        void onTextEdited(const QString& text);
        void onItemSelectionChanged();
        void onItemActivated();

};

#endif // GOTODIALOG_H
