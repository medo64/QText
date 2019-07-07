#include "mainwindow.h"
#include "singleinstance.h"

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

    storage = std::make_shared<Storage>("/home/josip/.qtext");

    MainWindow w { storage };

    if (!QSystemTrayIcon::isSystemTrayAvailable()) {
        w.show();
    }

    return a.exec();
}
