#pragma once

#include <QApplication>
#include <QMainWindow>
#include <QToolButton>
#include <QSystemTrayIcon>
#include "medo/hotkey.h"
#include "storage/storage.h"


namespace Ui {
    class MainWindow;
}

class MainWindow : public QMainWindow {
        Q_OBJECT

    public:
        explicit MainWindow(Storage* storage);
        ~MainWindow();

    protected:
        void closeEvent(QCloseEvent* event);
        void changeEvent(QEvent* event);
        void keyPressEvent(QKeyEvent* event);

    private:
        Ui::MainWindow* ui;
        Storage* _storage = nullptr;
        FolderItem* _folder = nullptr;
        QSystemTrayIcon* _tray = nullptr;
        QToolButton* _printButton = nullptr;
        QToolButton* _folderButton = nullptr;
        QToolButton* _appButton = nullptr;
        Hotkey* _hotkey = nullptr;
        void applySettings(bool applyShowInTaskbar = true, bool applyTabTextColorPerType = true);
        void selectFolder(QString folderName);
        void selectFolder(FolderItem* folder);
        void selectFile(QString fileName);
        void selectFile(FileItem* file);

    private slots:
        void onFileActivated(FileItem* file);
        void onFileTitleChanged(FileItem* file);
        void onFileModificationChanged(FileItem* file, bool isModified);
        void onFileNew();
        void onFileReopen();
        void onFileSave();
        void onFileRename();
        void onFileDelete();
        void onFilePrint();
        void onFilePrintPreview();
        void onFilePrintToPdf();
        void onTextCut();
        void onTextCopy();
        void onTextPaste();
        void onTextUndo();
        void onTextRedo();
        void onTextFontBold();
        void onTextFontItalic();
        void onTextFontUnderline();
        void onTextFontStrikethrough();
        void onFind();
        void onFindNext(bool backward = false);
        void onGoto();
        void onFolderSetup();
        void onFolderMenuShow();
        void onFolderMenuSelect();
        void onOpenWithDefaultApplication();
        void onOpenWithVisualStudioCode();
        void onShowContainingDirectory();
        void onShowContainingDirectoryOnly();
        void onCopyContainingPath();
        void onAppSettings();
        void onAppAbout();
        void onTabMenuRequested(const QPoint&);
        void onTabChanged();
        void onTabMoved(int from, int to);
        void onTextStateChanged();
        void onTrayActivate(QSystemTrayIcon::ActivationReason reason);
        void onTrayShow();
        void onAppQuit();
        void onUpdatedFolder(FolderItem* folder);

};
