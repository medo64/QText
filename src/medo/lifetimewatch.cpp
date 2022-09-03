/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

#include <QDebug>
#include "lifetimewatch.h"


LifetimeWatch::LifetimeWatch(const QString& text)
    : LifetimeWatch(text, false) {
}

LifetimeWatch::LifetimeWatch(const QString& text, bool addGuid) {
#ifdef QT_DEBUG
    _text = text;
    _stopwatch = new QElapsedTimer();
    _stopwatch->start();
    if (addGuid) {  _uuid = QUuid::createUuid(); }
    qDebug().noquote().nospace() << "[LifeTimeWatch] " << text
        << ( _uuid.isNull() ? "" : " " + _uuid.toString() );
#else
    Q_UNUSED(text)
    Q_UNUSED(addGuid)
#endif
}

void LifetimeWatch::elapsed() {
    this->elapsed("");
}

void LifetimeWatch::elapsed(const QString& extraText) {
#ifdef QT_DEBUG
    _mutex.lock();
    if (_stopwatch != nullptr) {
        qDebug().noquote().nospace() << "[LifeTimeWatch] " << _text
            << " elapsed at " << _stopwatch->elapsed() << "ms"
            << ( extraText.length() > 0 ? " (" + extraText + ")" : "" )
            << ( _uuid.isNull() ? "" : " " + _uuid.toString() );
    }
    _mutex.unlock();
#else
    Q_UNUSED(extraText)
#endif
}

void LifetimeWatch::done() {
#ifdef QT_DEBUG
    _mutex.lock();
    if (_stopwatch != nullptr) {
        qDebug().noquote().nospace() << "[LifeTimeWatch] " << _text
            << " done in " << _stopwatch->elapsed() << "ms"
            << ( _uuid.isNull() ? "" : " " + _uuid.toString() );
        delete(_stopwatch);
        _stopwatch = nullptr;
    }
    _mutex.unlock();
#endif
}

LifetimeWatch::~LifetimeWatch() {
    this->done();
}
