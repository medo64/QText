#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include "storage.h"
#include <memory>
#include <QApplication>
#include <QMainWindow>
#include <QToolButton>

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
        std::shared_ptr<FolderItem> _folder;
        QToolButton* _folderButton = nullptr;
        QClipboard* _clipboard = QApplication::clipboard();

    private slots:
        void onFileActivated(FileItem* file);
        void onFileTitleChanged(FileItem* file);
        void onFileModificationChanged(FileItem* file, bool isModified);
        void onNew();
        void onReopen();
        void onSave();
        void onRename();
        void onCut();
        void onCopy();
        void onPaste();
        void onUndo();
        void onRedo();
        void onFolderSelect();
        void onShowContainingDirectory();
        void onShowContainingDirectoryOnly();
        void onTabMenuRequested(const QPoint&);
        void onTextStateChanged();

};

#endif // MAINWINDOW_H
