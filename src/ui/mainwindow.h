#pragma once

#include <QApplication>
#include <QFutureWatcher>
#include <QMainWindow>
#include <QToolButton>
#include <QSystemTrayIcon>
#include "medo/dconfHotkey.h"
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
        void closeEvent(QCloseEvent* event) override;
        void changeEvent(QEvent* event) override;
        void keyPressEvent(QKeyEvent* event) override;
        void moveEvent(QMoveEvent* event) override;
        void resizeEvent(QResizeEvent* event) override;

    private:
        Ui::MainWindow* ui;
        Storage* _storage = nullptr;
        FolderItem* _folder = nullptr;
        QSystemTrayIcon* _tray = nullptr;
        QToolButton* _printButton = nullptr;
        QToolButton* _folderButton = nullptr;
        QToolButton* _appButton = nullptr;
        Hotkey* _hotkey = nullptr;
        DConfHotkey* _dconfHotkey = nullptr;
        QFutureWatcher<bool>* _futureWatcher = nullptr;
        QTimer* _moveTimer = nullptr;
        void applySettings() { applySettings(true, true, true, false, false, false, false); }
        void applySettings(bool applyShowInTaskbar, bool applyTabTextColorPerType, bool applyAlwaysOnTop, bool applyHotkey, bool applyDConfHotkey, bool applyDataPath, bool applyForceDarkMode);
        void applyToolbarIcons();
        bool selectFolder(QString folderName);
        bool selectFolder(FolderItem* folder);
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
        void onFolderMove();
        void onFileConvert();
        void onOpenWithDefaultApplication();
        void onOpenWithVisualStudioCode();
        void onShowContainingDirectory();
        void onShowContainingDirectoryOnly();
        void onCopyContainingPath();
        void onPhoneticSpelling();
        void onTopMost();
        void onAppSettings();
        void onAppFeedback();
        void onAppUpgrade();
        void onAppUpgradeChecked();
        void onAppAbout();
        void onAppQuit();
        void onTabMenuRequested(const QPoint&);
        void onTabChanged();
        void onTabMoved(int from, int to);
        void onTextStateChanged();
        void onHotkeyPress();
        void onTrayActivate(QSystemTrayIcon::ActivationReason reason);
        void onTrayShow();
        void onUpdatedFolder(FolderItem* folder);

};
