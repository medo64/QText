#include "config.h"
#include <QCoreApplication>
#include <QDebug>
#include <QDir>
#include <QSaveFile>
#include <QStandardPaths>
#include <QTextStream>

QMutex Config::_publicAccessMutex(QMutex::Recursive);
QString Config::_configurationFilePath;
QString Config::_stateFilePath;
QString Config::_dataDirectoryPath;
Config::PortableStatus Config::_isPortable(UNKNOWN);
bool Config::_immediateSave(true);
Config::ConfigFile* Config::_configFile(nullptr);
Config::ConfigFile* Config::_stateFile(nullptr);

void Config::reset() {
    QMutexLocker locker(&_publicAccessMutex);

    _configurationFilePath = QString();
    _stateFilePath = QString();
    _dataDirectoryPath = QString();
    _isPortable = PortableStatus::UNKNOWN;
    _immediateSave = true;
    resetConfigFile();
    resetStateFile();
}

bool Config::load() {
    QMutexLocker locker(&_publicAccessMutex);
    resetConfigFile();
    resetStateFile();
    QFile file(configurationFilePath());
    return file.exists();
}

bool Config::save() {
    QMutexLocker locker(&_publicAccessMutex);
    return getConfigFile()->save();
}


bool Config::isPortable() {
    QMutexLocker locker(&_publicAccessMutex);

    if (_isPortable == PortableStatus::UNKNOWN) {
        QString exePath = QCoreApplication::applicationFilePath();
        assert(!exePath.isNull()); //fail if QApplication is not initialized in debug mode
        if (exePath.isNull()) { //probably got called before QApplication got initialized, assume installed
            _isPortable = PortableStatus::FALSE;
            return false;
        }

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
    QMutexLocker locker(&_publicAccessMutex);
    reset();
    _isPortable = portable ? PortableStatus::TRUE : PortableStatus::FALSE;
}


bool Config::immediateSave() {
    QMutexLocker locker(&_publicAccessMutex);
    return _immediateSave;
}

void Config::setImmediateSave(bool saveImmediately) {
    QMutexLocker locker(&_publicAccessMutex);
    _immediateSave = saveImmediately;
    if (_immediateSave) { save(); } //to ensure any pending writes are cleared
}


QString Config::configurationFile() {
    QMutexLocker locker(&_publicAccessMutex);

    QString configPath = configurationFilePath();

    QFileInfo configFileInfo (configPath);
    QDir configFileDir = configFileInfo.dir();
    if (!configFileDir.exists()) { configFileDir.mkpath("."); }

    QFile configFile (configPath);
    if (!configFile.exists()) {
        configFile.open(QFile::WriteOnly);
        configFile.close();
    }

    return configPath;
}

QString Config::configurationFilePath() {
    QMutexLocker locker(&_publicAccessMutex);

    if (_configurationFilePath.isEmpty()) {
        _configurationFilePath = isPortable() ? configurationFilePathWhenPortable() : configurationFilePathWhenInstalled();
    }
    return _configurationFilePath;
}

void Config::setConfigurationFilePath(QString configurationFilePath) {
    QMutexLocker locker(&_publicAccessMutex);
    resetConfigFile();

    QString newPath = QDir::cleanPath(configurationFilePath);
    if (newPath.startsWith("~/")) { newPath = QDir::homePath() + newPath.mid(1); } //allow use of tilda (~) for home directory

    _configurationFilePath = newPath;
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
    if (exeDir.isNull()) { return configurationFilePathWhenInstalled(); } //fallback to installed
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


QString Config::stateFile() {
    QMutexLocker locker(&_publicAccessMutex);

    QString statePath = stateFilePath();

    QFileInfo stateFileInfo (statePath);
    QDir stateFileDir = stateFileInfo.dir();
    if (!stateFileDir.exists()) { stateFileDir.mkpath("."); }

    QFile stateFile (statePath);
    if (!stateFile.exists()) {
        stateFile.open(QFile::WriteOnly);
        stateFile.close();
    }

    return statePath;
}

QString Config::stateFilePath() {
    QMutexLocker locker(&_publicAccessMutex);

    if (_stateFilePath.isEmpty()) {
        _stateFilePath = isPortable() ? stateFilePathWhenPortable() : stateFilePathWhenInstalled();
    }
    return _stateFilePath;
}

void Config::setStateFilePath(QString stateFilePath) {
    QMutexLocker locker(&_publicAccessMutex);
    resetStateFile();

    QString newPath = QDir::cleanPath(stateFilePath);
    if (newPath.startsWith("~/")) { newPath = QDir::homePath() + newPath.mid(1); } //allow use of tilda (~) for home directory

    _stateFilePath = newPath;
}

QString Config::stateFilePathWhenPortable() {
#if defined(Q_OS_WIN)
    QString applicationName = QCoreApplication::applicationName();
    assert(applicationName.length() > 0);

    QString exeDir = QCoreApplication::applicationDirPath();
    QString stateFile = exeDir + "/" + applicationName + ".user";
#elif defined(Q_OS_LINUX)
    QString applicationName = QCoreApplication::applicationName().simplified().replace(" ", "").toLower(); //lowercase with spaces removed
    assert(applicationName.length() > 0);

    QString exeDir = QCoreApplication::applicationDirPath();
    if (exeDir.isNull()) { return stateFilePathWhenInstalled(); } //fallback to installed
    QString stateFile = exeDir + "/." + applicationName.toLower() + ".user";
#endif

    return QDir::cleanPath(stateFile);
}

QString Config::stateFilePathWhenInstalled() {
#if defined(Q_OS_WIN)
    QString applicationName = QCoreApplication::applicationName();
    assert(applicationName.length() > 0);

    QString appDataLocation = QStandardPaths::writableLocation(QStandardPaths::AppDataLocation);
    QString stateFile = appDataLocation + "/" + applicationName + ".user";
#elif defined(Q_OS_LINUX)
    QString applicationName = QCoreApplication::applicationName().simplified().replace(" ", "").toLower(); //lowercase with spaces removed
    assert(applicationName.length() > 0);

    QString configLocation = QStandardPaths::writableLocation(QStandardPaths::GenericConfigLocation);
    QString stateFile = configLocation + "/" + applicationName.toLower() + ".user";
#endif

    return QDir::cleanPath(stateFile);
}


QString Config::dataDirectory() {
    QMutexLocker locker(&_publicAccessMutex);

    QString dataPath = dataDirectoryPath();

    QDir dataDir (dataPath);
    if (!dataDir.exists()) { dataDir.mkpath("."); }

    return dataPath;
}

QString Config::dataDirectoryPath() {
    QMutexLocker locker(&_publicAccessMutex);

    if (_dataDirectoryPath.isEmpty()) {
        _dataDirectoryPath = isPortable() ? dataDirectoryPathWhenPortable() : dataDirectoryPathWhenInstalled();
    }
    return _dataDirectoryPath;
}

void Config::setDataDirectoryPath(QString dataDirectoryPath) {
    QMutexLocker locker(&_publicAccessMutex);
    QString newPath = QDir::cleanPath(dataDirectoryPath);
    if (newPath.startsWith("~/")) { newPath = QDir::homePath() + newPath.mid(1); } //allow use of tilda (~) for home directory
    _dataDirectoryPath = newPath;
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
    if (exeDir.isNull()) { return dataDirectoryPathWhenInstalled(); } //fallback to installed
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


QString Config::read(QString key) {
    key = key.trimmed(); //get rid of spaces around key
    if (key.isEmpty()) { return QString(); } //ignore empty keys; return null
    QMutexLocker locker(&_publicAccessMutex);
    QString value = getConfigFile()->readOne(key);
    return value;
}

QString Config::read(QString key, QString defaultValue) {
    key = key.trimmed(); //get rid of spaces around key
    if (key.isEmpty()) { return QString(); } //ignore empty keys; return null
    QString value = read(key);
    return !value.isNull() ? value : defaultValue;
}

void Config::write(QString key, QString value) {
    key = key.trimmed(); //get rid of spaces around key
    if (key.isEmpty()) { return; } //ignore empty keys
    QMutexLocker locker(&_publicAccessMutex);
    getConfigFile()->writeOne(key, value);
}

QString Config::read(QString key, const char* defaultValue) {
    return read(key, QString(defaultValue));
}

void Config::write(QString key, const char* value) {
    write(key, QString(value));
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

long long Config::read(QString key, long long defaultValue) {
    QString text = read(key, QString()).trimmed();
    bool isOK; long long value = text.toLongLong(&isOK);
    return isOK ? value : defaultValue;
}

void Config::write(QString key, long long value) {
    write(key, QString::number(value));
}

double Config::read(QString key, double defaultValue) {
    QString text = read(key, QString()).trimmed();
    if (text.compare("NAN", Qt::CaseInsensitive) == 0) { //compatibility with C#-based config
        return std::numeric_limits<double>::quiet_NaN();
    } else if (text.compare("Infinity", Qt::CaseInsensitive) == 0) { //compatibility with C#-based config
        return std::numeric_limits<double>::infinity();
    } else if (text.compare("-Infinity", Qt::CaseInsensitive) == 0) { //compatibility with C#-based config
        return -std::numeric_limits<double>::infinity();
    } else {
        bool isOK; double value = text.toDouble(&isOK);
        return isOK ? value : defaultValue;
    }
}

void Config::write(QString key, double value) {
    QString text = QString::number(value, 'G', 14);
    if (text.compare("NAN", Qt::CaseInsensitive) == 0) {
        write(key, "NaN"); //compatibility with C#-based config
    } else if (text.compare("INF", Qt::CaseInsensitive) == 0) {
        write(key, "Infinity"); //compatibility with C#-based config
    } else if (text.compare("-INF", Qt::CaseInsensitive) == 0) {
        write(key, "-Infinity"); //compatibility with C#-based config
    } else {
        write(key, text);
    }
}


QStringList Config::readMany(QString key) {
    key = key.trimmed(); //get rid of spaces around key
    if (key.isEmpty()) { return QStringList(); } //ignore empty keys; return empty list
    QMutexLocker locker(&_publicAccessMutex);
    QStringList values = getConfigFile()->readMany(key);
    return values;
}

QStringList Config::readMany(QString key, QStringList defaultValues) {
    QStringList values = readMany(key);
    return (values.length() > 0)? values : defaultValues;
}

void Config::writeMany(QString key, QStringList values) {
    key = key.trimmed(); //get rid of spaces around key
    if (key.isEmpty()) { return; } //ignore empty keys
    QMutexLocker locker(&_publicAccessMutex);
    if (values.length() > 0) {
        getConfigFile()->writeMany(key, values);
    } else {
        getConfigFile()->removeMany(key);
    }
}


void Config::remove(QString key) {
    key = key.trimmed(); //get rid of spaces around key
    if (key.isEmpty()) { return; } //ignore empty keys
    QMutexLocker locker(&_publicAccessMutex);
    getConfigFile()->removeMany(key);
}

void Config::removeAll() {
    QMutexLocker locker(&_publicAccessMutex);
    getConfigFile()->removeAll();
}


QString Config::stateRead(QString key, QString defaultValue) {
    key = key.trimmed(); //get rid of spaces around key
    if (key.isEmpty()) { return QString(); } //ignore empty keys; return null
    QMutexLocker locker(&_publicAccessMutex);
    QString value = getStateFile()->readOne(key);
    return !value.isNull() ? value : defaultValue;
}

void Config::stateWrite(QString key, QString value) {
    key = key.trimmed(); //get rid of spaces around key
    if (key.isEmpty()) { return; } //ignore empty keys
    QMutexLocker locker(&_publicAccessMutex);
    getStateFile()->writeOne(key, value);
}

QString Config::stateRead(QString key, const char* defaultValue) {
    return stateRead(key, QString(defaultValue));
}

void Config::stateWrite(QString key, const char* value) {
    stateWrite(key, QString(value));
}

bool Config::stateRead(QString key, bool defaultValue) {
    QString text = stateRead(key, QString()).trimmed();
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

void Config::stateWrite(QString key, bool value) {
    stateWrite(key, value ? QString("true") : QString("false"));
}


QStringList Config::stateReadMany(QString key) {
    key = key.trimmed(); //get rid of spaces around key
    if (key.isEmpty()) { return QStringList(); } //ignore empty keys; return empty list
    QMutexLocker locker(&_publicAccessMutex);
    QStringList values = getStateFile()->readMany(key);
    return values;
}

QStringList Config::stateReadMany(QString key, QStringList defaultValues) {
    QStringList values = stateReadMany(key);
    return (values.length() > 0)? values : defaultValues;
}

void Config::stateWriteMany(QString key, QStringList values) {
    key = key.trimmed(); //get rid of spaces around key
    if (key.isEmpty()) { return; } //ignore empty keys
    QMutexLocker locker(&_publicAccessMutex);
    if (values.length() > 0) {
        getStateFile()->writeMany(key, values);
    } else {
        getStateFile()->removeMany(key);
    }
}


Config::ConfigFile* Config::getConfigFile() {
    if (_configFile == nullptr) {
        _configFile = new ConfigFile(configurationFile());
    }
    return _configFile;
}

void Config::resetConfigFile() {
    if (_configFile != nullptr) {
        delete _configFile;
        _configFile = nullptr;
    }
}


Config::ConfigFile* Config::getStateFile() {
    if (_stateFile == nullptr) {
        _stateFile = new ConfigFile(stateFile());
    }
    return _stateFile;
}

void Config::resetStateFile() {
    if (_stateFile != nullptr) {
        delete _stateFile;
        _stateFile = nullptr;
    }
}


Config::ConfigFile::ConfigFile(QString filePath) {
    QString fileContent;
    QString lineEnding = QString();
    QFile file(filePath);
    if (file.open(QFile::ReadOnly)) {
        QTextStream in(&file);
        in.setCodec("UTF-8");
        fileContent = in.readAll(); //we'll handle new-line ourselves

        if (!fileContent.isNull()) {
            QString currLine = QString();
            bool lineEndingDetermined = false;

            QChar prevChar = '\0';
            for (QChar ch: fileContent) {
                if (ch == '\n') {
                    if (prevChar == '\r') { //CRLF pair
                        if (!lineEndingDetermined) {
                            lineEnding = "\r\n";
                            lineEndingDetermined = true;
                        }
                    } else {
                        if (!lineEndingDetermined) {
                            lineEnding = "\n";
                            lineEndingDetermined = true;
                        }
                        processLine(currLine);
                        currLine.clear();
                    }
                } else if (ch == '\r') {
                    processLine(currLine);
                    currLine.clear();
                    if (!lineEndingDetermined) { lineEnding = "\r"; } //do not set as determined as there is possibility of trailing LF
                } else {
                    if (!lineEnding.isNull()) { lineEndingDetermined = true; } //if there was a line ending before, mark it as determined
                    currLine.append(ch);
                }
                prevChar = ch;
            }
            _fileLoaded = true;

            processLine(currLine);
        }
    }

#ifdef Q_OS_WIN
    _lineEnding = !lineEnding.isNull() ? lineEnding : "\r\n";
#else
    _lineEnding = !lineEnding.isNull() ? lineEnding : "\n";
#endif

#ifdef QT_DEBUG
    qDebug().noquote() << "[Config]" << configurationFilePath();
    for(LineData line: _lines) {
        QString key = line.getKey();
        QString value = line.getValue();
        if (!key.isNull() && !key.isEmpty()) {
            qDebug().noquote() << "[Config]" << key + ":" << value;
        }
    }
#endif

    _filePath = filePath;
}

void Config::ConfigFile::processLine(QString lineText) {
    QString valueSeparator = QString();

    QString sbKey = QString();
    QString sbValue = QString();
    QString sbComment = QString();
    QString sbWhitespace = QString();
    QString sbEscapeLong = QString();
    QString separatorPrefix = QString();
    QString separatorSuffix = QString();
    QString commentPrefix = QString();

    ProcessState state = ProcessState::Default;
    ProcessState prevState = ProcessState::Default;
    for (QChar ch: lineText) {
        switch (state) {
            case ProcessState::Default:
                if (ch.isSpace()) {
                } else if (ch == '#') {
                    sbComment.append(ch);
                    state = ProcessState::Comment;
                } else if (ch == '\\') {
                    state = ProcessState::KeyEscape;
                } else {
                    sbKey.append(ch);
                    state = ProcessState::Key;
                }
                break;

            case ProcessState::Comment:
                sbComment.append(ch);
                break;

            case ProcessState::Key:
                if (ch.isSpace()) {
                    valueSeparator = ch;
                    state = ProcessState::SeparatorOrValue;
                } else if ((ch == ':') || (ch == '=')) {
                    valueSeparator = ch;
                    state = ProcessState::ValueOrWhitespace;
                } else if (ch == '#') {
                    sbComment.append(ch);
                    state = ProcessState::Comment;
                } else if (ch == '\\') {
                    state = ProcessState::KeyEscape;
                } else {
                    sbKey.append(ch);
                }
                break;

            case ProcessState::SeparatorOrValue:
                if (ch.isSpace()) {
                } else if ((ch == ':') || (ch == '=')) {
                    valueSeparator = ch;
                    state = ProcessState::ValueOrWhitespace;
                } else if (ch == '#') {
                    sbComment.append(ch);
                    state = ProcessState::Comment;
                } else if (ch == '\\') {
                    state = ProcessState::ValueEscape;
                } else {
                    sbValue.append(ch);
                    state = ProcessState::Value;
                }
                break;

            case ProcessState::ValueOrWhitespace:
                if (ch.isSpace()) {
                } else if (ch == '#') {
                    sbComment.append(ch);
                    state = ProcessState::Comment;
                } else if (ch == '\\') {
                    state = ProcessState::ValueEscape;
                } else {
                    sbValue.append(ch);
                    state = ProcessState::Value;
                }
                break;

            case ProcessState::Value:
                if (ch.isSpace()) {
                    state = ProcessState::ValueOrComment;
                } else if (ch == '#') {
                    sbComment.append(ch);
                    state = ProcessState::Comment;
                } else if (ch == '\\') {
                    state = ProcessState::ValueEscape;
                } else {
                    sbValue.append(ch);
                }
                break;

            case ProcessState::ValueOrComment:
                if (ch.isSpace()) {
                } else if (ch == '#') {
                    sbComment.append(ch);
                    state = ProcessState::Comment;
                } else if (ch == '\\') {
                    sbValue.append(sbWhitespace);
                    state = ProcessState::ValueEscape;
                } else {
                    sbValue.append(sbWhitespace);
                    sbValue.append(ch);
                    state = ProcessState::Value;
                }
                break;

            case ProcessState::KeyEscape:
            case ProcessState::ValueEscape:
                if (ch == 'u') {
                    state = (state == ProcessState::KeyEscape) ? ProcessState::KeyEscapeLong : ProcessState::ValueEscapeLong;
                } else {
                    QChar newCh;
                    switch (ch.unicode()) {
                        case '0': newCh = '\000'; break;
                        case 'a': newCh = '\a'; break;
                        case 'b': newCh = '\b'; break;
                        case 'f': newCh = '\f'; break;
                        case 'n': newCh = '\n'; break;
                        case 'r': newCh = '\r'; break;
                        case 't': newCh = '\t'; break;
                        case 'v': newCh = '\v'; break;
                        case '_': newCh = ' '; break;
                        default: newCh = ch; break;
                    }
                    if (state == ProcessState::KeyEscape) {
                        sbKey.append(newCh);
                    } else {
                        sbValue.append(newCh);
                    }
                    state = (state == ProcessState::KeyEscape) ? ProcessState::Key : ProcessState::Value;
                }
                break;

            case ProcessState::KeyEscapeLong:
            case ProcessState::ValueEscapeLong:
                sbEscapeLong.append(ch);
                if (sbEscapeLong.length() == 4) {
                    bool isOK;
                    int chValue = sbEscapeLong.toInt(&isOK, 16);
                    if (isOK) {
                        if (state == ProcessState::KeyEscape) {
                            sbKey.append(QChar(chValue));
                        } else {
                            sbValue.append(QChar(chValue));
                        }
                    }
                    state = (state == ProcessState::KeyEscapeLong) ? ProcessState::Key : ProcessState::Value;
                }
                break;
        }

        if (ch.isSpace() && (prevState != ProcessState::KeyEscape) && (prevState != ProcessState::ValueEscape) && (prevState != ProcessState::KeyEscapeLong) && (prevState != ProcessState::ValueEscapeLong)) {
            sbWhitespace.append(ch);
        } else if (state != prevState) { //on state change, clean comment prefix
            if ((state == ProcessState::ValueOrWhitespace) && (separatorPrefix.isNull())) {
                separatorPrefix = sbWhitespace;
                sbWhitespace.clear();
            } else if ((state == ProcessState::Value) && (separatorSuffix.isNull())) {
                separatorSuffix = sbWhitespace;
                sbWhitespace.clear();
            } else if ((state == ProcessState::Comment) && (commentPrefix.isNull())) {
                commentPrefix = sbWhitespace;
                sbWhitespace.clear();
            } else if ((state == ProcessState::Key) || (state == ProcessState::ValueOrWhitespace) || (state == ProcessState::Value)) {
                sbWhitespace.clear();
            }
        }

        prevState = state;
    }

    LineData line = LineData(sbKey, separatorPrefix, valueSeparator, separatorSuffix, sbValue, commentPrefix, sbComment);
    _lines.push_back(line);
}


bool Config::ConfigFile::save() {
    QString content;
    for(int i=0; i<_lines.length(); i++) {
        if (i>0) { content += _lineEnding; }
        content.append(_lines[i].toString());
    }

    QSaveFile file(_filePath);
    if (file.open(QIODevice::WriteOnly)) {
        QTextStream out(&file);
        out.setCodec("UTF-8");
        out << content;
        return file.commit();
    } else {
        qDebug().noquote() << "[Config]" << "Cannot write file!" << file.errorString();
        return false;
    }
}

QString Config::ConfigFile::readOne(QString key) {
    QMutexLocker locker(&_cacheMutex);
    QVariant cacheValue = _cache.value(key.toUpper());
    if (!cacheValue.isNull()) {
        QStringList cachedValues = cacheValue.toStringList();
        return !cachedValues.empty() ? cachedValues.last() : QString();
    }

    QStringList values;
    for(int i=0; i<_lines.length(); i++) {
        LineData line = _lines[i];
        if (key.compare(line.getKey(), Qt::CaseInsensitive) == 0) {
            values.append(line.getValue());
        }
    }

    _cache.insert(key.toUpper(), values);
    return !values.isEmpty() ? values.last() : QString();
}

QStringList Config::ConfigFile::readMany(QString key) {
    QMutexLocker locker(&_cacheMutex);
    QVariant cacheValue = _cache.value(key.toUpper());
    if (!cacheValue.isNull()) {
        QStringList cachedValues = cacheValue.toStringList();
        return !cachedValues.empty() ? cachedValues : QStringList();
    }

    QStringList values = QStringList();
    for(int i=0; i<_lines.length(); i++) {
        LineData line = _lines[i];
        if (key.compare(line.getKey(), Qt::CaseInsensitive) == 0) {
            values.push_back(line.getValue());
        }
    }

    _cache.insert(key.toUpper(), values);
    return values;
}

void Config::ConfigFile::writeOne(QString key, QString value) {
    QMutexLocker locker(&_cacheMutex);
    _cache.remove(key.toUpper()); //invalidate cache and deal with it during read

    int index = -1;
    for(int i=0; i<_lines.length(); i++) {
        LineData line = _lines[i];
        if (key.compare(line.getKey(), Qt::CaseInsensitive) == 0) {
            index = i; //last key takes precedence
        }
    }

    if (index >= 0) {
        _lines[index].setValue(value);
    } else {
        LineData* templateLine;
        if (_lines.length() > 0) {
            templateLine = &_lines[0];
        } else {
            templateLine = nullptr;
        }
        LineData newData = LineData(templateLine, key, value);

        if (_lines.length() == 0) {
            _lines.push_back(newData);
            _lines.push_back(LineData());
        } else if (!_lines[_lines.length() - 1].isEmpty()) {
            _lines.push_back(newData);
        } else {
            _lines.insert(_lines.length() - 1, newData);
        }
    }

    if (_immediateSave) { save(); }
}

void Config::ConfigFile::writeMany(QString key, QStringList values) {
    QMutexLocker locker(&_cacheMutex);
    _cache.remove(key.toUpper()); //invalidate cache and deal with it during read

    int lastIndex = -1;
    LineData lastLine;
    for (int i = _lines.length() - 1; i>=0; i--) { //find insertion point
        LineData line = _lines[i];
        if (key.compare(line.getKey(), Qt::CaseInsensitive) == 0) {
            if (lastLine.isEmpty()) {
                lastLine = line;
                lastIndex = i;
            } else {
                lastIndex--;
            }
            _lines.removeAt(i);
        }
    }

    if (lastIndex >= 0) {
        bool hasLines = (_lines.length() > 0);
        for(QString value: values) {
            LineData* templateLine;
            if (!lastLine.isEmpty()) {
                templateLine = &lastLine;
            } else if (hasLines) {
                templateLine = &_lines[0];
            } else {
                templateLine = nullptr;
            }
            _lines.insert(lastIndex, LineData(templateLine, key, value));
            lastIndex++;
        }
    } else {
        bool hasLines = (_lines.length() > 0);
        if (!hasLines) {
            for(QString value: values) {
                _lines.push_back(LineData(nullptr, key, value));
            }
            _lines.push_back(LineData());
        } else if (!_lines[_lines.length() - 1].isEmpty()) {
            for(QString value: values) {
                _lines.push_back(LineData(&_lines[0], key, value));
            }
        } else {
            for(QString value: values) {
                _lines.insert(_lines.length() - 1, LineData(&_lines[0], key, value));
            }
        }
    }

    if (_immediateSave) { save(); }
}

void Config::ConfigFile::removeMany(QString key) {
    QMutexLocker locker(&_cacheMutex);
    _cache.remove(key.toUpper()); //invalidate cache

    for(int i=_lines.length()-1; i>=0; i--) {
        LineData line = _lines[i];
        if (key.compare(line.getKey(), Qt::CaseInsensitive) == 0) {
            _lines.removeAt(i);
        }
    }
    if (_immediateSave) { save(); }
}

void Config::ConfigFile::removeAll() {
    QMutexLocker locker(&_cacheMutex);
    _cache.clear(); //invalidate cache

    _lines.clear();
    if (_immediateSave) { save(); }
}


Config::ConfigFile::LineData::LineData() {
}

Config::ConfigFile::LineData::LineData(LineData* lineTemplate, QString key, QString value)
    : LineData(key, QString(), QString(), QString(), value, QString(), QString()) {
    if (lineTemplate != nullptr) {
        _separatorPrefix = lineTemplate->_separatorPrefix;
        _separator = lineTemplate->_separator;
        _separatorSuffix = lineTemplate->_separatorSuffix;

        int firstKeyTotalLength = (lineTemplate->_key.length()) + lineTemplate->_separatorPrefix.length() + 1 + lineTemplate->_separatorSuffix.length();
        int totalLengthWithoutSuffix = key.length() + lineTemplate->_separatorPrefix.length() + 1;
        int maxSuffixLength = firstKeyTotalLength - totalLengthWithoutSuffix;
        if (maxSuffixLength < 1) { maxSuffixLength = 1; } //leave at least one space
        if (_separatorSuffix.length() > maxSuffixLength) {
            _separatorSuffix = _separatorSuffix.left(maxSuffixLength);
        }
    }

    if (_separatorPrefix.isNull()) { _separatorPrefix = ""; }
    if (_separator.isNull()) { _separator = ":"; }
    if (_separatorSuffix.isNull()) { _separatorSuffix = " "; }
}

Config::ConfigFile::LineData::LineData(QString key, QString separatorPrefix, QString separator, QString separatorSuffix, QString value, QString commentPrefix, QString comment) {
    _key = key;
    _separatorPrefix = separatorPrefix;
    _separator = separator;
    _separatorSuffix = separatorSuffix;
    _value = value;
    _commentPrefix = commentPrefix;
    _comment = comment;
}


QString Config::ConfigFile::LineData::getKey() {
    return _key;
}

QString Config::ConfigFile::LineData::getValue() {
    return _value;
}

void Config::ConfigFile::LineData::setValue(QString newValue) {
    _value = newValue;
}

bool Config::ConfigFile::LineData::isEmpty() {
    return _key.isEmpty() && _value.isEmpty() && _commentPrefix.isEmpty() && _comment.isEmpty();
}

QString Config::ConfigFile::LineData::toString() {
    QString sb = QString();
    if (!_key.isEmpty()) {
        escapeIntoStringBuilder(&sb, _key, true);

        if (!_value.isEmpty()) {
            if ((_separator == ':') || (_separator == '=')) {
                sb.append(_separatorPrefix);
                sb.append(_separator);
                sb.append(_separatorSuffix);
            } else {
                sb.append(_separatorSuffix.isEmpty() ? " " : _separatorSuffix);
            }
            escapeIntoStringBuilder(&sb, _value);
        } else { //try to preserve formatting in case of spaces (thus omitted)
            sb.append(_separatorPrefix);
            if (_separator == ':') {
                sb.append(":");
            } else if (_separator == '=') {
                sb.append("=");
            }
            sb.append(_separatorSuffix);
        }
    }

    if (!_comment.isEmpty()) {
        if (!_commentPrefix.isEmpty()) { sb.append(_commentPrefix); }
        sb.append(_comment);
    }

    return sb;
}

void Config::ConfigFile::LineData::escapeIntoStringBuilder(QString* sb, QString text, bool isKey) {
    for (int i=0; i < text.length(); i++) {
        QChar ch = text[i];
        switch (ch.unicode()) {
            case '\\': sb->append("\\\\"); break;
            case '\0': sb->append("\\0"); break;
            case '\a': sb->append("\\a"); break;
            case '\b': sb->append("\\b"); break;
            case '\f': sb->append("\\f"); break;
            case '\n': sb->append("\\n"); break;
            case '\r': sb->append("\\r"); break;
            case '\t': sb->append("\\t"); break;
            case '\v': sb->append("\\v"); break;
            case '#': sb->append("\\#"); break;
            default:
                if (!ch.isPrint()) {
                    sb->append(QString("%1").arg(ch.unicode(), 4, 16, QChar('0')));
                } else if (ch == ' ') {
                    if ((i == 0) || (i == (text.length() - 1)) || isKey) {
                        sb->append("\\_");
                    } else {
                        sb->append(ch);
                    }
                } else if (ch.isSpace()) {
                    switch (ch.unicode()) {
                        case '\0': sb->append("\\0"); break;
                        case '\a': sb->append("\\a"); break;
                        case '\b': sb->append("\\b"); break;
                        case '\f': sb->append("\\f"); break;
                        case '\n': sb->append("\\n"); break;
                        case '\r': sb->append("\\r"); break;
                        case '\t': sb->append("\\t"); break;
                        case '\v': sb->append("\\v"); break;
                        default: sb->append(QString("%1").arg(ch.unicode(), 4, 16, QChar('0'))); break;
                    }
                } else if (ch == '\\') {
                    sb->append("\\\\");
                } else {
                    sb->append(ch);
                }
                break;
        }
    }
}
