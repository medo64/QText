#include <QDebug>
#include "appsetupmutex.h"

const int mutexNameLength = 4096;

AppSetupMutex::AppSetupMutex(const QString& mutexName) {
#if defined(Q_OS_WIN)
    wchar_t mutexNameChars[mutexNameLength]; //assume 4K is enough
    int len = mutexName.toWCharArray(mutexNameChars);
    assert(len + 1 < mutexNameLength);
    mutexNameChars[len] = '\0';
    _mutexHandle = CreateMutex(NULL, false, mutexNameChars);
    if (_mutexHandle != nullptr) {
        qDebug().noquote() << "[AppSetupMutex]" << mutexName << "created as" << _mutexHandle;
    } else {
        qDebug().noquote() << "[AppSetupMutex]" << mutexName << "could not be created";
    }
#endif
}

AppSetupMutex::~AppSetupMutex() {
#if defined(Q_OS_WIN)
    CloseHandle(_mutexHandle);
    qDebug().noquote() << "[AppSetupMutex]" << _mutexHandle << "closed";
#endif
}
