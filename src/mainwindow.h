#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include "storage.h"
#include "medo/hotkey.h"
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
        explicit MainWindow(std::shared_ptr<Storage> storage);
        ~MainWindow();

    protected:
        void closeEvent(QCloseEvent *event);
        void changeEvent(QEvent *event);
        void keyPressEvent(QKeyEvent *event);

    private:
        Ui::MainWindow *ui;
        std::shared_ptr<Storage> _storage;
        std::shared_ptr<FolderItem> _folder;
        QSystemTrayIcon *_tray;
        QToolButton* _folderButton = nullptr;
        QToolButton* _appButton = nullptr;
        QClipboard* _clipboard = QApplication::clipboard();
        Hotkey* _hotkey;

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
        void onAppAbout();
        void onTabMenuRequested(const QPoint&);
        void onTabChanged();
        void onTextStateChanged();
        void onTrayActivate(QSystemTrayIcon::ActivationReason reason);
        void onTrayShow();
        void onTrayExit();

};

#endif // MAINWINDOW_H
