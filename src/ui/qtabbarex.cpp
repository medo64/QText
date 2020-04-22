#include <math.h>
#include <QApplication>
#include <QMouseEvent>
#include "qtabbarex.h"

QTabBarEx::QTabBarEx(QWidget *parent)
    : QTabBar(parent) {

}

void QTabBarEx::mouseMoveEvent(QMouseEvent* event) {
    if ((event->buttons() & Qt::LeftButton) && (_sourceIndex >= 0) && !_moveInProgress) {
        auto destinationPoint = event->pos();
        auto distanceSquared = pow(destinationPoint.x() - _sourcePoint.x(), 2) + pow(destinationPoint.y() - _sourcePoint.y(), 2);
        if (distanceSquared >= QApplication::startDragDistance()) {
            this->setCursor(Qt::SizeHorCursor);
            _moveInProgress = true;
        }
    }
    QTabBar::mouseMoveEvent(event);
}

void QTabBarEx::mousePressEvent(QMouseEvent* event)  {
    if (event->button() == Qt::LeftButton) {
        if (_longClickTimer != nullptr) { delete _longClickTimer; }
        if (QElapsedTimer::isMonotonic()) { //don't deal with time if clock is not monotonic
            _longClickTimer = new QElapsedTimer();
            _longClickTimer->start();
        }

        _sourceIndex = this->tabAt(event->pos());
        if (_sourceIndex >= 0) {
            _sourcePoint = event->pos();
            _moveInProgress = false;
        }
    }
    QTabBar::mousePressEvent(event);
}

void QTabBarEx::mouseReleaseEvent(QMouseEvent* event) {
    if ((event->button() == Qt::LeftButton) && (_sourceIndex >= 0) && _moveInProgress) {
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
        }
    } else if ((event->button() == Qt::LeftButton) && (_longClickTimer != nullptr) && !_moveInProgress) {
        bool longPress = _longClickTimer->elapsed() >= (QApplication::startDragTime() * 3);
        delete _longClickTimer;
        _longClickTimer = nullptr;
        if (longPress) { this->customContextMenuRequested(event->pos()); }
    }

    _sourceIndex = -1;
    _moveInProgress = false;
    this->setCursor(Qt::ArrowCursor);
    QTabBar::mouseReleaseEvent(event);
}

void QTabBarEx::moveTab(int from, int to) {
    QTabBar::moveTab(from, to);
    emit tabMoved(from, to);
}
