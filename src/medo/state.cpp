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
        QByteArray geometry = QByteArray::fromBase64(parts[0].toLatin1());
        QByteArray state = QByteArray::fromBase64(parts[1].toLatin1());
        window->restoreGeometry(geometry);
        window->restoreState(state);
    }
}

void State::saveEx(QString objectName, QMainWindow* window) {
    QString geometry = window->saveGeometry().toBase64();
    QString state = window->saveState().toBase64();
    emit writeToConfig(objectName, geometry + " " + state);
}
