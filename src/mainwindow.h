#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include "storage.h"
#include <memory>
#include <QApplication>
#include <QMainWindow>
#include <QToolButton>
#include <QSystemTrayIcon>

namespace Ui {
    class MainWindow;
}

class MainWindow : public QMainWindow {
    Q_OBJECT

    public:
        explicit MainWindow(std::shared_ptr<Storage> storage, QSystemTrayIcon *tray);
        ~MainWindow();

    protected:
        void closeEvent(QCloseEvent *event);
        void keyPressEvent(QKeyEvent *event);

    private:
        Ui::MainWindow *ui;
        std::shared_ptr<Storage> _storage;
        std::shared_ptr<FolderItem> _folder;
        QSystemTrayIcon *_tray;
        QToolButton* _folderButton = nullptr;
        QClipboard* _clipboard = QApplication::clipboard();

    private slots:
        void onFileActivated(FileItem* file);
        void onFileTitleChanged(FileItem* file);
        void onFileModificationChanged(FileItem* file, bool isModified);
        void onFileNew();
        void onFileReopen();
        void onFileSave();
        void onFileRename();
        void onFileDelete();
        void onTextCut();
        void onTextCopy();
        void onTextPaste();
        void onTextUndo();
        void onTextRedo();
        void onFolderSelect();
        void onShowContainingDirectory();
        void onShowContainingDirectoryOnly();
        void onTabMenuRequested(const QPoint&);
        void onTabChanged();
        void onTextStateChanged();
        void onTrayActivate(QSystemTrayIcon::ActivationReason reason);
        void onTrayShow();
        void onTrayExit();

};

#endif // MAINWINDOW_H
