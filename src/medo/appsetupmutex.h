/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

// 2020-06-09: Initial version

#pragma once

#include <QString>
#if defined(Q_OS_WIN)
    #include "windows.h"
#endif

namespace Medo { class AppSetupMutex; }

/* Intended for use with InnoSetup - make sense for Windows only */
class AppSetupMutex {

    public:
        AppSetupMutex(const QString& mutexName);
        ~AppSetupMutex();

    private:
#if defined(Q_OS_WIN)
        HANDLE _mutexHandle;
#endif

};
