#ifndef QTABBAREX_H
#define QTABBAREX_H

#include <QElapsedTimer>
#include <QMouseEvent>
#include <QPoint>
#include <QTabBar>

class QTabBarEx : public QTabBar {
    Q_OBJECT

    public:
        QTabBarEx(QWidget *parent = nullptr);
        void moveTab(int from, int to);

    protected:
        void mouseMoveEvent(QMouseEvent* event);
        void mousePressEvent(QMouseEvent* event);
        void mouseReleaseEvent(QMouseEvent* event);

    private:
        int _sourceIndex;
        QPoint _sourcePoint;
        bool _moveInProgress;
        QElapsedTimer* _longClickTimer = nullptr;

    signals:
        void tabMoved(int from, int to);

};

#endif // QTABBAREX_H
