#include <QApplication>
#include <QDesktopServices>
#include <QDesktopWidget>
#include <QDialog>
#include <QDialogButtonBox>
#include <QDir>
#include <QGuiApplication>
#include <QLabel>
#include <QNetworkReply>
#include <QNetworkRequest>
#include <QNetworkAccessManager>
#include <QProcess>
#include <QPushButton>
#include <QScreen>
#include <QSslConfiguration>
#include <QTimer>
#include <QVBoxLayout>
#include "upgrade.h"

bool Upgrade::showDialog(QWidget* parent, QUrl serviceUrl) {
    QRect geometry = (parent != nullptr)
                     ? QDesktopWidget().availableGeometry(parent)
                     : QGuiApplication::primaryScreen()->availableGeometry();
    int dialogWidth = geometry.width() / 6;
    if (dialogWidth < 400) { dialogWidth = 400; }

    QDialog dialog(parent);
    dialog.resize(dialogWidth, 1);
    dialog.setWindowTitle("Check for upgrade");

    QVBoxLayout layout;
    dialog.setLayout(&layout);

    QProgressBar progress;
    progress.setTextVisible(false);
    progress.setRange(0, 0);
    layout.addWidget(&progress);

    QLabel labelMessage;
    labelMessage.setText("Checking for upgrade...");
    layout.addWidget(&labelMessage);

    QDialogButtonBox buttonBox;
    buttonBox.setOrientation(Qt::Horizontal);
    buttonBox.setStandardButtons(QDialogButtonBox::Cancel);
    layout.addWidget(&buttonBox);

    QPushButton* buttonDownload;

    QObject::connect(&buttonBox, &QDialogButtonBox::rejected, [&]() {
        QApplication::restoreOverrideCursor();
        dialog.reject();
    });

    dialog.setFixedSize(dialog.geometry().width(), dialog.sizeHint().height());
    dialog.show();

    QApplication::setOverrideCursor(Qt::WaitCursor);
    UpgradeFile upgrade = Upgrade::upgradeFile(serviceUrl);
    QApplication::restoreOverrideCursor();
    if (!dialog.isVisible()) { return false; }

    progress.setRange(0, 1);
    if (upgrade.available()) {
        labelMessage.setText("Upgrade is available.");
#if defined(Q_OS_WIN) //on Windows offer to download file
        buttonDownload = buttonBox.addButton("&Download", QDialogButtonBox::YesRole);
        QObject::connect(buttonDownload, &QAbstractButton::clicked, [ & ]() {
            labelMessage.setText("Downloading upgrade...");
            buttonBox.removeButton(buttonDownload);

            QApplication::setOverrideCursor(Qt::WaitCursor);
            QString fileName = upgrade.suggestedLocalFileName();
            if (upgrade.downloadFile(fileName, 0, &progress)) {
                labelMessage.setText("Press OK to proceed with upgrade.");

                QObject::connect(&buttonBox, &QDialogButtonBox::accepted, &dialog, &QDialog::accept);
                buttonBox.setStandardButtons(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
            } else {
                labelMessage.setText("Error downloading upgrade.");
                buttonBox.setStandardButtons(QDialogButtonBox::Close);
            }
            QApplication::restoreOverrideCursor();
        });
#else //just show Close button if not Windows
        buttonBox.setStandardButtons(QDialogButtonBox::Close);
#endif

        if (dialog.exec() == QDialog::Accepted) {
            QProcess process;
            process.setProgram(upgrade.suggestedLocalFileName());
            if (process.startDetached()) {
                return true;
            } else {
                return false;
            }
        }
    } else {
        progress.setValue(100);
        labelMessage.setText("Upgrade not available.");
        buttonBox.setStandardButtons(QDialogButtonBox::Close);
        dialog.exec();
    }

    return false;
}

bool Upgrade::showDialog(QWidget* parent, QString serviceUrl) {
    return showDialog(parent, QUrl(serviceUrl));
}

UpgradeFile Upgrade::upgradeFile(QUrl serviceUrl) {
    if (serviceUrl.isEmpty()) { return UpgradeFile(); }

    QString appName;
    for (QChar c : QCoreApplication::applicationName()) {
        if (c.isLetterOrNumber()) { appName += c.toLower(); }
    }
    if (appName.isEmpty()) { return UpgradeFile(); }

    QString url = serviceUrl.toString();
    if (!url.endsWith("/")) { url += "/"; }
    url += appName;
    url += "/";
    url += APP_VERSION;
    url += "/";

    QElapsedTimer stopwatch;
    stopwatch.start();
    return getUpgradeFileFromURL(&stopwatch, url);
}

UpgradeFile Upgrade::upgradeFile(QString serviceUrl) {
    return upgradeFile(QUrl(serviceUrl));
}

bool Upgrade::available(QUrl serviceUrl) {
    return upgradeFile(serviceUrl).available();
}

bool Upgrade::available(QString serviceUrl) {
    return available(QUrl(serviceUrl));
}

UpgradeFile Upgrade::getUpgradeFileFromURL(QElapsedTimer* stopwatch, QUrl url) {
    if (stopwatch->elapsed() > 7000) { //give up if already running more than 7 seconds
        qDebug().noquote().nospace() << "[Upgrade] Upgrade timed out (took " << stopwatch->elapsed() << " ms)";
        return UpgradeFile();
    }

    int statusCode;
    QString redirectUrl = processUrl(url, &statusCode);

    switch (statusCode) {
        // No update at this time
        case 204: //NoContent
        case 410: //Gone (old)
            qDebug().noquote().nospace() << "[Upgrade] Upgrade not available (took " << stopwatch->elapsed() << " ms)";
            return UpgradeFile();

        // Upgrade found
        case 303: { //SeeOther
                QString newRedirect = redirectUrl;
                for (int i = 0; i < 3; i++) { //check for additional redirects
                    newRedirect = processUrl(newRedirect, &statusCode);
                    if (statusCode == 200) { break; }
                    if (newRedirect.isEmpty()) {
                        qDebug().noquote().nospace() << "[Upgrade] Cannot resolve upgrade at " << redirectUrl;
                        return UpgradeFile();
                    }
                }
                qDebug().noquote().nospace() << "[Upgrade] Found upgrade at " << redirectUrl << " (took " << stopwatch->elapsed() << " ms)";
                return UpgradeFile(redirectUrl);
            }

        // Redirect
        case 301: { //Redirect
                return getUpgradeFileFromURL(stopwatch, redirectUrl);
            }

        // Unknown
        case 404: //NotFound
        case 501: //NotImplemented (old)
        default:
            qDebug().noquote().nospace() << "[Upgrade] Cannot check for upgrade (took " << stopwatch->elapsed() << " ms)";
            return UpgradeFile();
    }
}

QString Upgrade::processUrl(QUrl url, int* statusCode) {
    QElapsedTimer stopwatch;
    stopwatch.start();

    QNetworkAccessManager network;
    network.setRedirectPolicy(QNetworkRequest::NoLessSafeRedirectPolicy);

    QNetworkRequest request(url);
    request.setAttribute(QNetworkRequest::CacheLoadControlAttribute, QNetworkRequest::AlwaysNetwork);
    request.setAttribute(QNetworkRequest::FollowRedirectsAttribute, false);
    request.setMaximumRedirectsAllowed(0);
    if (url.scheme() == "https") {
        QSslConfiguration sslConfig;
        sslConfig.setProtocol(QSsl::TlsV1_2OrLater);
        request.setSslConfiguration(sslConfig);
    }

    request.setHeader(QNetworkRequest::ContentTypeHeader, "application/octet-stream");

    QTimer timer;
    timer.setSingleShot(true);
    timer.start(2000);

    QNetworkReply* reply = network.get(request);

    QEventLoop loop;
    loop.connect(reply, &QNetworkReply::finished, &loop, &QEventLoop::quit);
    loop.connect(&timer, &QTimer::timeout, &loop, &QEventLoop::quit);
    loop.exec();

    QString redirectUrl;
    *statusCode = reply->attribute(QNetworkRequest::HttpStatusCodeAttribute).toInt();
    if (*statusCode > 0) {
        qDebug().noquote().nospace() << "[Upgrade] Request to " << url.toString() << " resulted in status code " << *statusCode << ": " << reply->attribute(QNetworkRequest::HttpReasonPhraseAttribute).toString() << " (took " << stopwatch.elapsed() << " ms)";
        redirectUrl = reply->header(QNetworkRequest::LocationHeader).toString();
    } else {
        qDebug().noquote().nospace() << "[Upgrade] Request to " << url.toString() << " resulted in " << QVariant::fromValue(reply->error()).toString() << ": " << reply->errorString() << " (took " << stopwatch.elapsed() << " ms)";
    }

    delete  reply;
    return redirectUrl;
}


UpgradeFile::UpgradeFile() {
    _downloadUrl = QUrl();
}

UpgradeFile::UpgradeFile(QUrl fileUrl) {
    _downloadUrl = fileUrl;
}

bool UpgradeFile::available() {
    return !_downloadUrl.isEmpty();
}

QUrl UpgradeFile::fileUrl() {
    return _downloadUrl;
}

QString UpgradeFile::fileName() {
    return !_downloadUrl.isEmpty() ? _downloadUrl.fileName() : QString();
}

QString UpgradeFile::suggestedLocalFileName() {
    if (_downloadUrl.isEmpty()) { return QString(); }
    QStringList tempPaths = QStandardPaths::standardLocations(QStandardPaths::TempLocation);
    return QDir::cleanPath(tempPaths[0] + "/" + fileName());
}

bool UpgradeFile::downloadFile(QString fileName, int timeout, QProgressBar* progressBar) {
    if (_downloadUrl.isEmpty()) { return false; }

    QElapsedTimer stopwatch;
    stopwatch.start();

    QTimer timer;
    timer.setSingleShot(true);
    if (timeout > 0) { //don't both starting timer for 0 or negative
        timer.start(timeout);
    }

    QNetworkAccessManager network;
    network.setRedirectPolicy(QNetworkRequest::NoLessSafeRedirectPolicy);

    QNetworkRequest request(_downloadUrl);
    request.setAttribute(QNetworkRequest::CacheLoadControlAttribute, QNetworkRequest::AlwaysNetwork);
    request.setAttribute(QNetworkRequest::FollowRedirectsAttribute, true);
    request.setMaximumRedirectsAllowed(3);
    if (_downloadUrl.scheme() == "https") {
        QSslConfiguration sslConfig;
        sslConfig.setProtocol(QSsl::TlsV1_2OrLater);
        request.setSslConfiguration(sslConfig);
    }

    request.setHeader(QNetworkRequest::ContentTypeHeader, "application/octet-stream");

    QNetworkReply* reply = network.get(request);

    QEventLoop loop;
    loop.connect(reply, &QNetworkReply::finished, &loop, &QEventLoop::quit);
    loop.connect(&timer, &QTimer::timeout, &loop, &QEventLoop::quit);
    if (progressBar != nullptr) {
        loop.connect(reply, &QNetworkReply::downloadProgress, [progressBar](qint64 bytesReceived, qint64 bytesTotal) {
            progressBar->setRange(0, bytesTotal);
            progressBar->setValue(bytesReceived);
            progressBar->show();
        });
    }
    loop.exec();

    if (reply->error() == QNetworkReply::NoError) {
        if (!reply->isFinished()) {
            qDebug().noquote().nospace() << "[Upgrade] Timed out waiting on " << _downloadUrl.toString() << " (took " << stopwatch.elapsed() << " ms)";
            delete reply;
            return false;
        }

        QFile localFile(fileName);
        if (!localFile.open(QIODevice::WriteOnly)) {
            qDebug().noquote().nospace() << "[Upgrade] Cannot open " << localFile.fileName() << " for writing (took " << stopwatch.elapsed() << " ms)";
            delete reply;
            return false;
        }

        localFile.write(reply->readAll());
        localFile.close();
        qDebug().noquote().nospace() << "[Upgrade] Downloaded upgrade to " << localFile.fileName() << " (took " << stopwatch.elapsed() << " ms)";

        delete reply;
        return true;
    } else {
        qDebug().noquote().nospace() << "[Upgrade] Download attempt resulted in " << QVariant::fromValue(reply->error()).toString() << ": " << reply->errorString() << " (took " << stopwatch.elapsed() << " ms)";
    }

    delete reply;
    return false;
}
