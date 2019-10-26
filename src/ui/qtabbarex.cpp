#include "qtabbarex.h"
#include <math.h>
#include <QMouseEvent>

QTabBarEx::QTabBarEx(QWidget *parent)
    : QTabBar(parent) {

}

void QTabBarEx::mouseMoveEvent(QMouseEvent* event) {
    if ((event->buttons() & Qt::LeftButton) && (_sourceIndex >= 0) && !_cursorSet) {
        auto destinationPoint = event->pos();
        auto distanceSquared = pow(destinationPoint.x() - _sourcePoint.x(), 2) + pow(destinationPoint.y() - _sourcePoint.y(), 2);
        if (distanceSquared > 5) {
            this->setCursor(Qt::SizeHorCursor);
            _cursorSet = true;
        }
    }
    QTabBar::mouseMoveEvent(event);
}

void QTabBarEx::mousePressEvent(QMouseEvent* event)  {
    if (event->button() == Qt::LeftButton) {
        _sourceIndex = this->tabAt(event->pos());
        if (_sourceIndex >= 0) {
            _sourcePoint = event->pos();
            _cursorSet = false;
        }
    }
    QTabBar::mousePressEvent(event);
}

void QTabBarEx::mouseReleaseEvent(QMouseEvent* event) {
    if ((event->button() == Qt::LeftButton) && (_sourceIndex >= 0) && _cursorSet) {
        int destinationIndex = this->tabAt(event->pos());
        if (destinationIndex == -1) {
            if (event->x() > this->tabRect(this->count() - 1).right()) {
                destinationIndex = this->count() - 1;
            } else if (event->x() < this->tabRect(0).left()) {
                destinationIndex = 0;
            }
        }
        if ((_sourceIndex != destinationIndex) && (destinationIndex >= 0)) {
            moveTab(_sourceIndex, destinationIndex);
            emit tabMoved(_sourceIndex, destinationIndex);
        }
    }

    _sourceIndex = -1;
    _cursorSet = false;
    this->setCursor(Qt::ArrowCursor);
    QTabBar::mouseReleaseEvent(event);
}
