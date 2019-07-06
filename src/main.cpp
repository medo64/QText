#include "mainwindow.h"

static std::shared_ptr<Storage> storage;

int main(int argc, char *argv[]) {
    QCoreApplication::setApplicationName("QText");
    QCoreApplication::setOrganizationName("Josip Medved");

    qputenv("QT_DEVICE_PIXEL_RATIO", QByteArray("auto"));

    QApplication a(argc, argv);
    storage = std::make_shared<Storage>("/home/josip/.qtext");
    QApplication::setQuitOnLastWindowClosed(false);

    MainWindow w { storage };

    if (!QSystemTrayIcon::isSystemTrayAvailable()) {
        w.show();
    }

    return a.exec();
}
