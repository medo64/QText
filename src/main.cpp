#include "mainwindow.h"
#include "settings.h"
#include "setup.h"
#include "medo/config.h"
#include "medo/singleinstance.h"

static std::shared_ptr<Storage> storage;

int main(int argc, char *argv[]) {
    QCoreApplication::setApplicationName("QText");
    QCoreApplication::setOrganizationName("Josip Medved");

    qputenv("QT_DEVICE_PIXEL_RATIO", QByteArray("auto"));

    QApplication a(argc, argv);
    QApplication::setQuitOnLastWindowClosed(false);

    if (!SingleInstance::attach()) {
        return static_cast<int>(0x80004004); //exit immediately if another instance is running
    }

    storage = std::make_shared<Storage>(Settings::dataPath());

    MainWindow w { storage };

#ifndef QT_DEBUG
    if (!QSystemTrayIcon::isSystemTrayAvailable()) {
        w.show();
    }
#else //show immediately when debugging
    w.show();
#endif

    if (!Settings::setupCompleted() && !Config::isPortable()) {
        Settings::setSetupCompleted(true);
        Setup::setAutostart(true);
    }

    return a.exec();
}
