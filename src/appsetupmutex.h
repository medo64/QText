#pragma once
#include <QString>
#if defined(Q_OS_WIN)
    #include "windows.h"
#endif

/* Intended for use with InnoSetup - make sense for Windows only */
class AppSetupMutex {

    public:
        AppSetupMutex(const QString& mutexName);
        ~AppSetupMutex();

    private:
        HANDLE _mutexHandle;

};
