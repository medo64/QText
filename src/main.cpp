#include "mainwindow.h"
#include <QApplication>
#include <QCoreApplication>
#include <QSysInfo>
#include <QSystemTrayIcon>

static std::shared_ptr<Storage> storage;

int main(int argc, char *argv[]) {
    QCoreApplication::setApplicationName("QText");
    QCoreApplication::setOrganizationName("Josip Medved");

    qputenv("QT_DEVICE_PIXEL_RATIO", QByteArray("auto"));

    QApplication a(argc, argv);
    storage = std::make_shared<Storage>("/home/josip/.qtext");

    QIcon trayIcon;
    if ((QSysInfo::kernelType() == "winnt") && (QSysInfo::productVersion() == "10")) {
        trayIcon.addFile(":icons/16x16/tray-white.png", QSize(16, 16));
        trayIcon.addFile(":icons/32x32/tray-white.png", QSize(32, 32));
        trayIcon.addFile(":icons/48x48/tray-white.png", QSize(48, 48));
        trayIcon.addFile(":icons/64x64/tray-white.png", QSize(64, 64));
    } else {
        trayIcon.addFile(":icons/16x16/tray-color.png", QSize(16, 16));
        trayIcon.addFile(":icons/32x32/tray-color.png", QSize(32, 32));
        trayIcon.addFile(":icons/48x48/tray-color.png", QSize(48, 48));
        trayIcon.addFile(":icons/64x64/tray-color.png", QSize(64, 64));
    }

    QSystemTrayIcon *tray = new QSystemTrayIcon(trayIcon);

    MainWindow w { storage, tray };

    if (!tray->isSystemTrayAvailable()) {
        w.show();
    }

    return a.exec();
}
