#ifndef FILEITEM_H
#define FILEITEM_H

#include <memory>
#include <QFocusEvent>
#include <QString>
#include <QTextEdit>
#include <QTimer>

class FileItem : public QTextEdit {
    Q_OBJECT

    public:
        FileItem(QString directoryPath, QString fileName);
        ~FileItem();
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
        void focusInEvent(QFocusEvent *event);
        void focusOutEvent(QFocusEvent *event);

    private:
        QString _directoryPath;
        QString _fileName;
        QTextEdit* _editor = nullptr;
        QTimer* _timerSavePending = nullptr;
        void setIsModified(bool isModified);

    signals:
        void activated(FileItem* file);
        void titleChanged(FileItem* file);
        void modificationChanged(FileItem* file, bool isModified);

    private slots:
        void onModificationChanged(bool changed);
        void onSavePendingTimeout();

};

#endif // FILEITEM_H
