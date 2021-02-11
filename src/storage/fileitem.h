#pragma once

#include <QDateTime>
#include <QFocusEvent>
#include <QPrinter>
#include <QString>
#include <QTextCodec>
#include <QTextEdit>
#include <QTimer>
#include <QUuid>
#include "filetype.h"
#include "folderitem.h"

class FolderItem;

class FileItem : public QTextEdit {
        Q_OBJECT
        friend class FolderItem;
        friend class Storage;

    public:
        FolderItem* folder() const;
        QUuid key() const { return _key; }
        QString name() const;
        QString path() const;
        QString title() const;
        void setTitle(QString newTitle);
        FileType type() const;
        bool setType(FileType newType);
        QString extension() const;
        bool isModified() const;
        bool isEmpty() const;
        bool load();
        bool save() const;
        bool setFolder(FolderItem* newFolder);

    public:
        bool isTextUndoAvailable();
        void textUndo();
        bool isTextRedoAvailable();
        void textRedo();

        bool canTextCut();
        void textCut(bool forcePlain = false);
        bool canTextCopy();
        void textCopy(bool forcePlain = false);
        bool canTextPaste();
        void textPaste(bool forcePlain = false);

        bool isFontBold();
        void setFontBold(bool bold = true);
        bool isFontItalic();
        void setFontItalic(bool italic = true);
        bool isFontUnderline();
        void setFontUnderline(bool underline = true);
        bool isFontStrikethrough();
        void setFontStrikethrough(bool strikethrough = true);

    public:
        FileItem(const FileItem&) = delete;
        void operator=(const FileItem&) = delete;

    protected:
        virtual bool event(QEvent* event);
        virtual bool eventFilter(QObject* obj, QEvent* event);
        virtual void focusInEvent(QFocusEvent* e);
        virtual void focusOutEvent(QFocusEvent* e);
        virtual void wheelEvent(QWheelEvent* e);

    private:
        FileItem(FolderItem* folder, QString fileName);
        ~FileItem();
        QString findAnchorAt(QPoint pos);
        QUuid _key = QUuid::createUuid();
        FolderItem* _folder = nullptr;
        QString _fileName;
        QTextEdit* _editor = nullptr;
        QTextCodec* _utf8Codec = QTextCodec::codecForName("UTF-8");
        QTimer* _timerSavePending = nullptr;
        mutable QDateTime _modificationTime;
        int zoomAmount = 0;
        bool customCursorSet = false;

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
        void onContextMenuCut();
        void onContextMenuCopy();
        void onContextMenuPaste();
        void onContextMenuCutPlain();
        void onContextMenuCopyPlain();
        void onContextMenuPastePlain();
        void onContextMenuDelete();
        void onContextMenuSelectAll();
        void onContextMenuInsertTime();
        void onContextMenuResetFont();
        void onGoToUrl();

};
