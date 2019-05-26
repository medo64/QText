#include "mainwindow.h"
#include <QApplication>

static std::shared_ptr<Storage> storage;

int main(int argc, char *argv[]) {
    qputenv("QT_DEVICE_PIXEL_RATIO", QByteArray("auto"));

    QApplication a(argc, argv);
    storage = std::make_shared<Storage>("/home/josip/.qtext");
    MainWindow w { storage };
    w.show();

    return a.exec();
}
