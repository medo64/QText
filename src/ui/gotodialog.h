#pragma once

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
        explicit GotoDialog(QWidget* parent = nullptr,  Storage* storage = nullptr);
        ~GotoDialog();
        QString folderKey() const { return _folderKey; }
        QString fileKey() const { return _fileKey; }

    protected:
        void accept();
        void hideEvent(QHideEvent* event);

    private:
        Ui::GotoDialog* ui;
        bool eventFilter(QObject* obj, QEvent* event);
        QString _folderKey;
        QString _fileKey;
        Storage* _storage = nullptr;
        QIcon _folderIcon;
        QIcon _fileIcon;

    private slots:
        void onTextEdited(const QString& text);
        void onItemSelectionChanged();
        void onItemActivated();

};
