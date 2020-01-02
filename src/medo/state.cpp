#include <QApplication>
#include <QScreen>
#include <QWindow>
#include "state.h"

State State::_instance;

State::State()
    : QObject(nullptr) {
}

State::~State() {
}

State* State::instance() {
    return &_instance;
}


void State::load(QMainWindow* window) {
    load(QString(), window);
}

void State::load(QString objectName, QMainWindow* window) {
    if (window == nullptr) { return; }
    if (objectName.isNull()) { objectName = window->objectName(); }
    instance()->loadEx(objectName, window);
}

void State::save(QMainWindow* window) {
    save(QString(), window);
}

void State::save(QString objectName, QMainWindow* window) {
    if (window == nullptr) { return; }
    if (objectName.isNull()) { objectName = window->objectName(); }
    instance()->saveEx(objectName, window);
}


void State::loadEx(QString objectName, QMainWindow* window) {
    QString value = emit readFromConfig(objectName);
    QStringList parts = value.split(' ');
    if (parts.length() == 2) {
        QByteArray geometry = QByteArray::fromBase64(parts[0].toUtf8());
        QByteArray state = QByteArray::fromBase64(parts[1].toUtf8());
        window->restoreGeometry(geometry);
        window->restoreState(state);
    }
}

void State::saveEx(QString objectName, QMainWindow* window) {
    QString geometry = window->saveGeometry().toBase64();
    QString state = window->saveState().toBase64();
    emit writeToConfig(objectName, geometry + " " + state);
}


void State::load(QWidget* widget) {
    load(QString(), widget);
}

void State::load(QString objectName, QWidget* widget) {
    if (widget == nullptr) { return; }
    if (objectName.isNull()) { objectName = widget->objectName(); }
    instance()->loadEx(objectName, widget);

    QWidget* parent = widget->parentWidget();
    if (parent != nullptr) {
        QScreen* screen = nullptr;
        QWindow* widgetHandle = widget->windowHandle();
        if (widgetHandle != nullptr) { screen = widgetHandle->screen(); }
        if (screen == nullptr) {
            QWindow* parentHandle = parent->windowHandle();
            if (parentHandle != nullptr) { screen = parentHandle->screen(); }
        }
        if (screen == nullptr) { screen = QGuiApplication::screenAt(widget->pos()); }
        if (screen == nullptr) { screen = QGuiApplication::primaryScreen(); }

        int minX = screen->availableGeometry().x();
        int minY = screen->availableGeometry().y();
        int maxW = screen->availableGeometry().width();
        int maxH = screen->availableGeometry().height();

        int newX = parent->x() + (parent->width() - widget->width()) / 2;
        int newY = parent->y() + (parent->height() - widget->height()) / 2 + QApplication::startDragDistance() * 2; //frame geometry doesn't really work on X11 so drag distance is workaround
        int newW = widget->width();
        int newH = widget->height();
        if (newX < minX) { newX = minX; }
        if (newY < minY) { newY = minY; }
        if (newX + newW > maxW) { newW = maxW - newX; }
        if (newY + newH > maxH) { newH = maxH - newY; }

        widget->setGeometry(newX, newY, newW, newH);
    }
}

void State::save(QWidget* widget) {
    save(QString(), widget);
}

void State::save(QString objectName, QWidget* widget) {
    if (widget == nullptr) { return; }
    if (objectName.isNull()) { objectName = widget->objectName(); }
    instance()->saveEx(objectName, widget);
}


void State::loadEx(QString objectName, QWidget* widget) {
    QString value = emit readFromConfig(objectName);
    QByteArray geometry = QByteArray::fromBase64(value.toUtf8());
    widget->restoreGeometry(geometry);
}

void State::saveEx(QString objectName, QWidget* widget) {
    QString geometry = widget->saveGeometry().toBase64();
    emit writeToConfig(objectName, geometry);
}
