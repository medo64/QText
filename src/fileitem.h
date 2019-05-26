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

    protected:
        void focusOutEvent(QFocusEvent *event);

    private:
        QString _directoryPath;
        QString _fileName;
        QString getPath();
        QTextEdit* _editor = nullptr;
        QTimer* _timer = nullptr;
        bool _hasChanged = false;

    private slots:
        void onContentsChanged();
        void onAfterChangeTimeout();

};

#endif // FILEITEM_H
