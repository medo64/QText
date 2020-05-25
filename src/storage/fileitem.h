#pragma once

#include <QDateTime>
#include <QFocusEvent>
#include <QPrinter>
#include <QString>
#include <QTextEdit>
#include <QTimer>
#include "folderitem.h"

class FolderItem;

class FileItem : public QTextEdit {
        Q_OBJECT

    public:
        typedef enum {
            Plain    = 0,
            Markdown = 1,
            Html     = 2,
        } FileType;


    public:
        FileItem(FolderItem* folder, QString fileName);
        ~FileItem();
        FolderItem* folder();
        QString name();
        QString path();
        QString title();
        void setTitle(QString newTitle);
        FileType type();
        QString typeExtension();
        bool isModified();
        bool isEmpty();
        bool load();
        bool save();

    public:
        FileItem(const FileItem&) = delete;
        void operator=(const FileItem&) = delete;

    protected:
        bool event(QEvent* event);
        void focusInEvent(QFocusEvent* e);
        void focusOutEvent(QFocusEvent* e);
        void wheelEvent(QWheelEvent* e);

    private:
        FolderItem* _folder = nullptr;
        QString _fileName;
        QTextEdit* _editor = nullptr;
        QTimer* _timerSavePending = nullptr;
        void setIsModified(bool isModified);
        QDateTime _modificationTime;
        int zoomAmount = 0;

    signals:
        void activated(FileItem* file);
        void titleChanged(FileItem* file);
        void modificationChanged(FileItem* file, bool isModified);

    public slots:
        void printPreview(QPrinter* printer);

    private slots:
        void onModificationChanged(bool changed);
        void onSavePendingTimeout();
        void onContextMenuRequested(const QPoint&);
        void onContextMenuUndo();
        void onContextMenuRedo();
        void onContextMenuCutPlain();
        void onContextMenuCopyPlain();
        void onContextMenuPastePlain();
        void onContextMenuDelete();
        void onContextMenuSelectAll();
        void onContextMenuInsertTime();

};
