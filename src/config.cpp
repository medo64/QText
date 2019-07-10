#include "config.h"
#include <QCoreApplication>
#include <QDir>
#include <QStandardPaths>

QString Config::_configurationFilePath;
QString Config::_dataDirectoryPath;
Config::PortableStatus Config::_isPortable(UNKNOWN);

bool Config::isPortable() {
    if (_isPortable == PortableStatus::UNKNOWN) {
        QString exePath = QCoreApplication::applicationFilePath();

#if defined(Q_OS_WIN)
        QString localAppDataEnv { QDir::cleanPath(getenv("LOCALAPPDATA")) };
        QString programFilesEnv { QDir::cleanPath(getenv("ProgramFiles")) };
        QString programFilesX86Env { QDir::cleanPath(getenv("ProgramFiles(x86)")) };
        QString programFilesW6432Env { QDir::cleanPath(getenv("ProgramW6432")) };
        QString localAppDataPath = !localAppDataEnv.isEmpty() ? localAppDataEnv + "/Programs/" : QString();
        QString programFilesPath = !programFilesEnv.isEmpty() ? programFilesEnv + "/" : QString();
        QString programFilesX86Path = !programFilesX86Env.isEmpty() ? programFilesX86Env + "/" : QString();
        QString programFilesW6432Path = !programFilesW6432Env.isEmpty() ? programFilesW6432Env + "/" : QString();
        QString localAppDataProgramsPath { !localAppDataPath.isEmpty() ? localAppDataPath + "/Programs/" + "/" : QString() };
        bool isLocalAppData = !localAppDataPath.isEmpty() ? exePath.startsWith(localAppDataPath, Qt::CaseInsensitive) : false;
        bool isProgramFiles = !programFilesPath.isEmpty() ? exePath.startsWith(programFilesPath, Qt::CaseInsensitive) : false;
        bool isProgramFilesX86 = !programFilesX86Path.isEmpty() ? exePath.startsWith(programFilesX86Path, Qt::CaseInsensitive) : false;
        bool isProgramFilesW6432 = !programFilesW6432Path.isEmpty() ? exePath.startsWith(programFilesW6432Path, Qt::CaseInsensitive) : false;
        bool isInstalled = isLocalAppData || isProgramFiles || isProgramFilesX86 || isProgramFilesW6432;
#elif defined(Q_OS_LINUX)
        QStringList homePaths = QStandardPaths::standardLocations(QStandardPaths::HomeLocation);
        QString homeBinPath = (homePaths.count() > 0) ? QDir::cleanPath(homePaths[0] + "/bin") + "/" : QString();
        QString homeLocalBinPath = (homePaths.count() > 0) ? QDir::cleanPath(homePaths[0] + "/.local/bin") + "/" : QString();
        bool isHomeBin = !homeBinPath.isEmpty() ? exePath.startsWith(homeBinPath, Qt::CaseSensitive) : false;
        bool isHomeLocalBin = !homeLocalBinPath.isEmpty() ? exePath.startsWith(homeLocalBinPath, Qt::CaseSensitive) : false;
        bool isOpt = exePath.startsWith("/opt/", Qt::CaseSensitive);
        bool isBin = exePath.startsWith("/bin/", Qt::CaseSensitive);
        bool isUsrBin = exePath.startsWith("/usr/bin/", Qt::CaseSensitive);
        bool isUsrLocalBin = exePath.startsWith("/usr/local/bin/", Qt::CaseSensitive);
        bool isInstalled = isHomeBin || isHomeLocalBin || isOpt || isBin || isUsrBin || isUsrLocalBin;
#endif

        QFileInfo portableConfigFile (configurationFilePathWhenPortable());
        if (portableConfigFile.exists()) {
            _isPortable = PortableStatus::TRUE; //force portable if there is a local file
        } else {
            _isPortable = !isInstalled ? PortableStatus::TRUE : PortableStatus::FALSE;
        }
    }
    return _isPortable;
}

void Config::setPortable(bool portable) {
    _isPortable = portable ? PortableStatus::TRUE : PortableStatus::FALSE;
    _configurationFilePath = QString();
    _dataDirectoryPath = QString();
}


QString Config::configurationFile() {
    QString configPath = configurationFilePath();

    QFileInfo configFileInfo (configPath);
    QDir configFileDir = configFileInfo.dir();
    if (!configFileDir.exists()) { configFileDir.mkpath("."); }

    QFile configFile (configPath);
    if (!configFile.exists()) {
        configFile.open(QIODevice::WriteOnly);
        configFile.close();
    }

    return configPath;
}

QString Config::configurationFilePath() {
    if (_configurationFilePath.isEmpty()) {
        _configurationFilePath = isPortable() ? configurationFilePathWhenPortable() : configurationFilePathWhenInstalled();
    }
    return _configurationFilePath;
}


QString Config::configurationFilePathWhenPortable() {
#if defined(Q_OS_WIN)
    QString applicationName = QCoreApplication::applicationName();
    assert(applicationName.length() > 0);

    QString exeDir = QCoreApplication::applicationDirPath();
    QString configFile = exeDir + "/" + applicationName + ".cfg";
#elif defined(Q_OS_LINUX)
    QString applicationName = QCoreApplication::applicationName().simplified().replace(" ", "").toLower(); //lowercase with spaces removed
    assert(applicationName.length() > 0);

    QString exeDir = QCoreApplication::applicationDirPath();
    QString configFile = exeDir + "/." + applicationName.toLower();
#endif

    return QDir::cleanPath(configFile);
}

QString Config::configurationFilePathWhenInstalled() {
#if defined(Q_OS_WIN)
    QString applicationName = QCoreApplication::applicationName();
    assert(applicationName.length() > 0);

    QString appDataLocation = QStandardPaths::writableLocation(QStandardPaths::AppDataLocation);
    QString configFile = appDataLocation + "/" + applicationName + ".cfg";
#elif defined(Q_OS_LINUX)
    QString applicationName = QCoreApplication::applicationName().simplified().replace(" ", "").toLower(); //lowercase with spaces removed
    assert(applicationName.length() > 0);

    QString configLocation = QStandardPaths::writableLocation(QStandardPaths::GenericConfigLocation);
    QString configFile = configLocation + "/" + applicationName.toLower() + ".conf";
#endif

    return QDir::cleanPath(configFile);
}


QString Config::dataDirectory() {
    QString dataPath = dataDirectoryPath();

    QDir dataDir (dataPath);
    if (!dataDir.exists()) { dataDir.mkpath("."); }

    return dataPath;
}

QString Config::dataDirectoryPath() {
    if (_dataDirectoryPath.isEmpty()) {
        _dataDirectoryPath = isPortable() ? dataDirectoryPathWhenPortable() : dataDirectoryPathWhenInstalled();
    }
    return _dataDirectoryPath;
}

QString Config::dataDirectoryPathWhenPortable() {
#if defined(Q_OS_WIN)
    QString applicationName = QCoreApplication::applicationName();
    assert(applicationName.length() > 0);

    QString exeDir = QCoreApplication::applicationDirPath();
    QString dataDirectory = exeDir + "/" + applicationName + ".Data";
#elif defined(Q_OS_LINUX)
    QString applicationName = QCoreApplication::applicationName().simplified().replace(" ", "").toLower(); //lowercase with spaces removed
    assert(applicationName.length() > 0);

    QString exeDir = QCoreApplication::applicationDirPath();
    QString dataDirectory = exeDir + "/." + applicationName.toLower() + ".data";
#endif

    return QDir::cleanPath(dataDirectory);
}

QString Config::dataDirectoryPathWhenInstalled() {
#if defined(Q_OS_WIN)
    QString applicationName = QCoreApplication::applicationName();
    assert(applicationName.length() > 0);

    QString appDataLocation = QStandardPaths::writableLocation(QStandardPaths::AppDataLocation);
    QString dataDirectory = appDataLocation + "/Data";
#elif defined(Q_OS_LINUX)
    QString applicationName = QCoreApplication::applicationName().simplified().replace(" ", "").toLower(); //lowercase with spaces removed
    assert(applicationName.length() > 0);

    QString dataLocation = QStandardPaths::writableLocation(QStandardPaths::GenericDataLocation);
    QString dataDirectory = dataLocation + "/" + applicationName.toLower();
#endif

    return QDir::cleanPath(dataDirectory);
}


QString Config::read(QString key, QString defaultValue) {
    return defaultValue;
}

void Config::write(QString key, QString value) {

}


QString Config::read(QString key, const char* defaultValue) {
    return read(key, QString(defaultValue));
}

void Config::write(QString key, const char* value) {
    write (key, QString(value));
}


bool Config::read(QString key, bool defaultValue) {
    QString text = read(key, QString()).trimmed();
    if ((text.compare("true", Qt::CaseInsensitive) == 0) || (text.compare("yes", Qt::CaseInsensitive) == 0)
            || (text.compare("T", Qt::CaseInsensitive) == 0) || (text.compare("Y", Qt::CaseInsensitive) == 0)
            || (text.compare("+", Qt::CaseInsensitive) == 0)) {
        return true;
    } else if ((text.compare("false", Qt::CaseInsensitive) == 0) || (text.compare("no", Qt::CaseInsensitive) == 0)
            || (text.compare("F", Qt::CaseInsensitive) == 0) || (text.compare("N", Qt::CaseInsensitive) == 0)
            || (text.compare("-", Qt::CaseInsensitive) == 0)) {
        return false;
    } else {
        bool isOK; long long value = text.toLongLong(&isOK);
        return isOK ? (value != 0) : defaultValue;
    }
}

void Config::write(QString key, bool value) {
    write(key, value ? "true" : "false");
}


int Config::read(QString key, int defaultValue) {
    QString text = read(key, QString()).trimmed();
    bool isOK; int value = text.toInt(&isOK);
    return isOK ? value : defaultValue;
}

void Config::write(QString key, int value) {
    write(key, QString::number(value));
}


long Config::read(QString key, long defaultValue) {
    QString text = read(key, QString()).trimmed();
    bool isOK; long value = text.toLong(&isOK);
    return isOK ? value : defaultValue;
}

void Config::write(QString key, long value) {
    write(key, QString::number(value));
}


double Config::read(QString key, double defaultValue) {
    QString text = read(key, QString()).trimmed();
    bool isOK; double value = text.toDouble(&isOK);
    return isOK ? value : defaultValue;
}

void Config::write(QString key, double value) {
    write(key, QString::number(value));
}
