#include "storagemonitorlocker.h"
#include "storagemonitorthread.h"

StorageMonitorLocker::StorageMonitorLocker(StorageMonitorThread* monitor) {
    _monitor = monitor;
    _monitor->stopMonitoring();
}

StorageMonitorLocker::~StorageMonitorLocker() {
    _monitor->continueMonitoring();
}
