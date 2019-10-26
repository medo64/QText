#ifndef QTABBAREX_H
#define QTABBAREX_H

#include <QMouseEvent>
#include <QPoint>
#include <QTabBar>

class QTabBarEx : public QTabBar {
    Q_OBJECT

    public:
        QTabBarEx(QWidget *parent = nullptr);

    protected:
        void mouseMoveEvent(QMouseEvent* event);
        void mousePressEvent(QMouseEvent* event);
        void mouseReleaseEvent(QMouseEvent* event);

    private:
        int _sourceIndex;
        QPoint _sourcePoint;
        bool _cursorSet;

    signals:
        void tabMoved(int from, int to);

};

#endif // QTABBAREX_H
