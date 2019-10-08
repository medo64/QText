#include <QCommandLineParser>
#include "mainwindow.h"
#include "settings.h"
#include "setup.h"
#include "medo/config.h"
#include "medo/singleinstance.h"

static std::shared_ptr<Storage> storage;

int main(int argc, char* argv[]) {
    QCoreApplication::setApplicationName(APP_PRODUCT);
    QCoreApplication::setOrganizationName(APP_COMPANY);
    QCoreApplication::setApplicationVersion(APP_VERSION);

    qputenv("QT_AUTO_SCREEN_SCALE_FACTOR", QByteArray("true"));
    qputenv("QT_SCALE_FACTOR", QByteArray("1"));

    QApplication app(argc, argv);

    QCommandLineParser cli;
    cli.setApplicationDescription("Note taking utility with auto-save.");
    cli.addHelpOption();
    cli.addVersionOption();
    QCommandLineOption hideOption(QStringList() << "s" << "hide", "Application is immediatelly sent to tray.");
    cli.addOption(hideOption);
    cli.process(app);

    QApplication::setQuitOnLastWindowClosed(false);

    if (!SingleInstance::attach()) {
        return static_cast<int>(0x80004004); //exit immediately if another instance is running
    }

    storage = std::make_shared<Storage>(Settings::dataPath());

    MainWindow w { storage };

#ifndef QT_DEBUG
    if (!QSystemTrayIcon::isSystemTrayAvailable() || !cli.isSet(hideOption)) {
        w.show();
    }
#else //show immediately when debugging
    w.show();
#endif

    if (!Settings::setupCompleted() && !Config::isPortable()) {
        Settings::setSetupCompleted(true);
        Setup::setAutostart(true);
    }

    return app.exec();
}
