#ifndef GOTODIALOG_H
#define GOTODIALOG_H

#include <QDialog>
#include <QIcon>
#include <QListWidget>
#include <memory>

#include "storage.h"

namespace Ui {
    class GotoDialog;
}

class GotoDialog : public QDialog {
    Q_OBJECT

    public:
        explicit GotoDialog(QWidget *parent = nullptr,  std::shared_ptr<Storage> storage = nullptr);
        ~GotoDialog();
        QString FolderKey;
        QString FileKey;

    protected:
        void accept();

    private:
        bool eventFilter(QObject* obj, QEvent* event);
        Ui::GotoDialog *ui;
        std::shared_ptr<Storage> _storage;
        QIcon _folderIcon;
        QIcon _fileIcon;

    private slots:
        void onTextEdited(const QString& text);
        void onItemSelectionChanged();
        void onItemActivated();

};

#endif // GOTODIALOG_H
