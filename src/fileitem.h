#ifndef FILEITEM_H
#define FILEITEM_H

#include <memory>
#include <QDateTime>
#include <QFocusEvent>
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
        void focusInEvent(QFocusEvent *event);
        void focusOutEvent(QFocusEvent *event);

    private:
        FolderItem* _folder;
        QString _fileName;
        QTextEdit* _editor = nullptr;
        QTimer* _timerSavePending = nullptr;
        void setIsModified(bool isModified);
        QDateTime _modificationTime;

    signals:
        void activated(FileItem* file);
        void titleChanged(FileItem* file);
        void modificationChanged(FileItem* file, bool isModified);

    private slots:
        void onModificationChanged(bool changed);
        void onSavePendingTimeout();

};

#endif // FILEITEM_H
