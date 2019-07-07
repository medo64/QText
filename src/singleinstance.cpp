#include "singleinstance.h"
#include <QCoreApplication>
#include <QCryptographicHash>
#include <QDebug>
#include <QDir>
#include <QLocalSocket>

SingleInstance SingleInstance::_instance(nullptr);
QMutex SingleInstance::_mutex(QMutex::NonRecursive);
QLocalServer* SingleInstance::_server(nullptr);
bool SingleInstance::_isFirstInstance(false);


SingleInstance::SingleInstance(QObject *parent)
    : QObject(parent) {
}

SingleInstance& SingleInstance::instance() {
    return _instance;
}


bool SingleInstance::attach() {
    QMutexLocker locker(&_mutex);

    if (_server != nullptr) { return _isFirstInstance; }

    QString serverNameSource = QCoreApplication::applicationFilePath();
    serverNameSource += QDir::home().dirName(); //instead of user name
    serverNameSource += QSysInfo::machineHostName();
    QString serverName = QString(QCryptographicHash::hash(serverNameSource.toUtf8(), QCryptographicHash::Sha256).toBase64());

    _server = new QLocalServer();
    _server->setSocketOptions(QLocalServer::WorldAccessOption);
    bool serverListening = _server->listen(serverName);
    if (!serverListening && (_server->serverError() == QAbstractSocket::AddressInUseError)) {
        QLocalServer::removeServer(serverName); //cleanup for Linux; can stay on Windows too
        serverListening = _server->listen(serverName);
    }

    bool isFirstInstance = false;
    if (serverListening) { //only the first instance can listen (on Linux)
        isFirstInstance = true;
        connect(_server, SIGNAL(newConnection()), &SingleInstance::instance(), SLOT(onNewConnection()));

#if defined(Q_OS_WIN) //check if there is a server running - needed on Windows as two local servers can run there
        _server->close(); //disable server to test client
        QLocalSocket* client = new QLocalSocket();
        client->connectToServer(serverName);
        if (client->waitForConnected(100)) { //we're not the first - there is another server listening (max 100 ms delay - but generally an immediate response)
            isFirstInstance = false;
        } else {
            isFirstInstance = _server->listen(serverName); //turn on server again
        }
#endif
    }

    if (!isFirstInstance) { //contact main instance if we're not the one
        qDebug() << "Another instance is running.";
        QLocalSocket* client = new QLocalSocket();
        client->connectToServer(serverName);
    }

    _isFirstInstance = isFirstInstance;
    return _isFirstInstance;
}

bool SingleInstance::isOtherInstanceRunning() {
    QMutexLocker locker(&_mutex);
    return !_isFirstInstance;
}


void SingleInstance::onNewConnection() {
    QLocalSocket *client = _server->nextPendingConnection();
    connect(client, &QLocalSocket::disconnected, client, &QLocalSocket::deleteLater);
    emit newInstanceDetected();
}
