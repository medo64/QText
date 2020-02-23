#ifndef FILEITEM_H
#define FILEITEM_H

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
        FileItem(FolderItem* folder, QString fileName);
        ~FileItem();
        FolderItem* getFolder();
        QString getKey();
        QString getPath();
        QString getTitle();
        void setTitle(QString newTitle);
        bool isPlain();
        bool isHtml();
        bool isModified();
        bool isEmpty();
        bool load();
        bool save();

    protected:
        bool event(QEvent *event);
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
        void printPreview(QPrinter *printer);

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

#endif // FILEITEM_H
