#pragma once

#include "storagemonitorthread.h"

class StorageMonitorLocker {

    public:
        StorageMonitorLocker(StorageMonitorThread* monitor);
        ~StorageMonitorLocker();

    private:
        StorageMonitorThread* _monitor;

};
