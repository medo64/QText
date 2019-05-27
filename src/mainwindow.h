#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include "storage.h"
#include <memory>
#include <QMainWindow>

namespace Ui {
    class MainWindow;
}

class MainWindow : public QMainWindow {
    Q_OBJECT

    public:
        explicit MainWindow(std::shared_ptr<Storage> storage);
        ~MainWindow();

    private:
        Ui::MainWindow *ui;
        std::shared_ptr<Storage> _storage;

    private slots:
        void onFileActivated(FileItem* file);
        void onFileTitleChanged(FileItem* file);
        void onFileModificationChanged(FileItem* file, bool isModified);
        void onNew();
        void onReopen();
        void onSave();
        void onRename();
        void onTabMenuRequested(const QPoint&);

};

#endif // MAINWINDOW_H
