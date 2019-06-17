#include "config.h"
#include <QCoreApplication>
#include <QDir>
#include <QFileInfo>
#include <QStandardPaths>

QString Config::_configurationFile;
QString Config::_dataDirectory;

/*!
 * \brief Forces recalculation of configuration file and data directory.
 */
void Config::reset() {
    _configurationFile = "";
    _dataDirectory = "";
}

/*!
 * \brief Returns name of configuration file.
 *        If file doesn't exist, it's created.
 */
QString Config::getConfigurationFile() {
    if (_configurationFile.length() == 0) {
        QString applicationName = QCoreApplication::applicationName();
        assert(applicationName.length() > 0);

#if defined(Q_OS_WIN) //use roaming directory
        QString appDataLocation = QStandardPaths::writableLocation(QStandardPaths::AppDataLocation);
        QString configFile = appDataLocation + "/" + applicationName + ".cfg";
#else
        applicationName = applicationName.simplified().replace(" ", "").toLower(); //lowercase with spaces removed
        QString configLocation = QStandardPaths::writableLocation(QStandardPaths::GenericConfigLocation);
        QString configFile = configLocation + "/" + applicationName.toLower() + ".conf";
#endif

        _configurationFile = QDir::cleanPath(configFile);
    }

    QFileInfo configFileInfo (_configurationFile);
    QDir configFileDir = configFileInfo.dir();
    if (!configFileDir.exists()) { configFileDir.mkpath("."); }

    QFile configFile (_configurationFile);
    if (!configFile.exists()) {
        configFile.open(QIODevice::WriteOnly);
        configFile.close();
    }

    return _configurationFile;
}

/*!
 * \brief Returns data directory.
 *        If directory doesn't exist, it's created.
 */
QString Config::getDataDirectory() {
    if (_dataDirectory.length() == 0) {
        QString applicationName = QCoreApplication::applicationName();
        assert(applicationName.length() > 0);

#if defined(Q_OS_WIN) //use roaming directory
        QString appDataLocation = QStandardPaths::writableLocation(QStandardPaths::AppDataLocation);
        QString dataDir = appDataLocation + "/Data";
#else
        applicationName = applicationName.simplified().replace(" ", "").toLower(); //lowercase with spaces removed
        QString dataLocation = QStandardPaths::writableLocation(QStandardPaths::GenericDataLocation);
        QString dataDir = dataLocation + "/" + applicationName.toLower();
#endif

        _dataDirectory = QDir::cleanPath(dataDir);
    }

    QDir dataDir (_dataDirectory);
    if (!dataDir.exists()) { dataDir.mkpath("."); }

    return _dataDirectory;
}


QString Config::read(QString key, QString defaultValue) {
    return "";
}

void Config::write(QString key, QString value) {

}

