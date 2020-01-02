#include <QCommandLineParser>
#include "ui/mainwindow.h"
#include "settings.h"
#include "setup.h"
#include "medo/config.h"
#include "medo/singleinstance.h"
#include "medo/state.h"

static Storage* storage;

int main(int argc, char* argv[]) {
    QCoreApplication::setApplicationName(APP_PRODUCT);
    QCoreApplication::setOrganizationName(APP_COMPANY);
    QCoreApplication::setApplicationVersion(APP_VERSION);

    QCoreApplication::setAttribute(Qt::AA_DisableWindowContextHelpButton);

    qputenv("QT_AUTO_SCREEN_SCALE_FACTOR", QByteArray("true"));
    qputenv("QT_SCALE_FACTOR", QByteArray("1"));

    QApplication app(argc, argv);

    QCommandLineParser cli;
    cli.setApplicationDescription(APP_DESCRIPTION);
    cli.addHelpOption();
    cli.addVersionOption();
    QCommandLineOption hideOption(QStringList() << "s" << "hide", "Application is immediatelly sent to tray.");
    cli.addOption(hideOption);
    cli.process(app);

    QApplication::setQuitOnLastWindowClosed(false);

    if (!SingleInstance::attach()) {
        return static_cast<int>(0x80004004); //exit immediately if another instance is running
    }

    QStringList dataPaths = Settings::dataPaths();
    Config::setStateFilePath(dataPaths[0] + "/.qtext.user"); //store state file in the first directory
    QApplication::connect(State::instance(), &State::writeToConfig, [=] (QString key, QString value) { Config::stateWrite("State!" + key, value); });
    QApplication::connect(State::instance(), &State::readFromConfig, [=] (QString key) { return Config::stateRead("State!" + key, QString()); });

    storage = new Storage(dataPaths);
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
