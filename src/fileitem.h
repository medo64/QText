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
        QString getTitle();
        bool isPlain();
        bool isHtml();
        bool hasChanged();
        bool load();
        bool save();

    protected:
        void focusOutEvent(QFocusEvent *event);

    private:
        QString _directoryPath;
        QString _fileName;
        QString getPath();
        QTextEdit* _editor = nullptr;
        QTimer* _timerSavePending = nullptr;
        bool _hasChanged = false;

    signals:
        void updateTabTitle(FileItem* file);

    private slots:
        void onModificationChanged(bool changed);
        void onSavePendingTimeout();

};

#endif // FILEITEM_H
