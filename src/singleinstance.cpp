#include "singleinstance.h"
#include <QCoreApplication>
#include <QCryptographicHash>
#include <QDebug>
#include <QDir>
#include <QLocalSocket>
#if defined(Q_OS_WIN)
    #include <windows.h>
#endif

SingleInstance SingleInstance::_instance;
QMutex SingleInstance::_mutex(QMutex::NonRecursive);
QLocalServer* SingleInstance::_server(nullptr);
bool SingleInstance::_isFirstInstance(false);


SingleInstance::SingleInstance()
    : QObject(nullptr) {
}

SingleInstance::~SingleInstance() {
    QMutexLocker locker(&_mutex);

    if (_server != nullptr) {
        _server->close();
        delete _server;
    }
}

SingleInstance* SingleInstance::instance() {
    return &_instance;
}


bool SingleInstance::attach() {
    QMutexLocker locker(&_mutex);

    if (_server != nullptr) { return _isFirstInstance; }

    QString serverNameSource = QCoreApplication::applicationFilePath();
    serverNameSource += QDir::home().dirName(); //instead of user name
    serverNameSource += QSysInfo::machineHostName();
    QString serverName = QString(QCryptographicHash::hash(serverNameSource.toUtf8(), QCryptographicHash::Sha256)
                                 .toBase64(QByteArray::Base64UrlEncoding | QByteArray::OmitTrailingEquals));

    _server = new QLocalServer();
    bool serverListening = _server->listen(serverName);
    if (!serverListening && (_server->serverError() == QAbstractSocket::AddressInUseError)) {
        QLocalSocket* client = new QLocalSocket();
        client->connectToServer(serverName);
        if (!client->waitForConnected(250)) { //no answer - assume cleanup is needed
            QLocalServer::removeServer(serverName);
            serverListening = _server->listen(serverName);
        }
        delete client;
    }

    bool isFirstInstance = false;
    if (serverListening) { //only the first instance can listen (on Linux)
        isFirstInstance = true;
        connect(_server, SIGNAL(newConnection()), SingleInstance::instance(), SLOT(onNewConnection()));

#if defined(Q_OS_WIN) //check if there is a server running - needed on Windows as two local servers can run there
        CreateMutexW(nullptr, true, reinterpret_cast<LPCWSTR>(serverName.utf16()));
        if (GetLastError() == ERROR_ALREADY_EXISTS) { //someone has this Mutex
            _server->close(); //disable server
            isFirstInstance = false;
        }
#endif
    }

    if (!isFirstInstance) { //contact main instance if we're not the one
        qDebug() << "Another instance is running.";
        QLocalSocket* client = new QLocalSocket();
        client->connectToServer(serverName);
        delete client;
    }

    _isFirstInstance = isFirstInstance;
    return _isFirstInstance;
}

bool SingleInstance::isOtherInstanceRunning() {
    QMutexLocker locker(&_mutex);
    return !_isFirstInstance;
}


void SingleInstance::onNewConnection() {
    QLocalSocket* client = _server->nextPendingConnection();
    connect(client, &QLocalSocket::disconnected, client, &QLocalSocket::deleteLater);
    delete client;
    emit newInstanceDetected();
}
