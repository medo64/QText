#pragma once

#include <QDateTime>
#include <QFocusEvent>
#include <QPrinter>
#include <QString>
#include <QTextEdit>
#include <QTimer>
#include <QUuid>
#include "filetype.h"
#include "folderitem.h"

class FolderItem;

class FileItem : public QTextEdit {
        Q_OBJECT

    public:
        FileItem(FolderItem* folder, QString fileName);
        ~FileItem();
        FolderItem* folder() const;
        QUuid key() const { return _key; }
        QString name() const;
        QString path() const;
        QString title() const;
        void setTitle(QString newTitle);
        FileType type() const;
        QString extension() const;
        bool isModified() const;
        bool isEmpty() const;
        bool load();
        bool save() const;
        bool setFolder(FolderItem* newFolder);

    public:
        FileItem(const FileItem&) = delete;
        void operator=(const FileItem&) = delete;

    protected:
        bool event(QEvent* event);
        void focusInEvent(QFocusEvent* e);
        void focusOutEvent(QFocusEvent* e);
        void wheelEvent(QWheelEvent* e);

    private:
        QUuid _key = QUuid::createUuid();
        FolderItem* _folder = nullptr;
        QString _fileName;
        QTextEdit* _editor = nullptr;
        QTimer* _timerSavePending = nullptr;
        mutable QDateTime _modificationTime;
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
