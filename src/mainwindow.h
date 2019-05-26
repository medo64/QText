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

    private slots:
        void onUpdateTabTitle(FileItem* file);
        void onSave();

};

#endif // MAINWINDOW_H
