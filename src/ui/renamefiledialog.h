#pragma once

#include <QDialog>
#include <QString>
#include <QWidget>
#include "storage/folderitem.h"

namespace Ui { class RenameFileDialog; }

class RenameFileDialog : public QDialog {
        Q_OBJECT

    public:
        explicit RenameFileDialog(QWidget* parent, FileItem* file);
        ~RenameFileDialog();
        QString title() const;

    private:
        Ui::RenameFileDialog* ui;
        FileItem* _file;

    private slots:
        void onChanged();

};
