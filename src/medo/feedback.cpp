#include <QCoreApplication>
#include <QCheckBox>
#include <QDesktopWidget>
#include <QDialog>
#include <QDialogButtonBox>
#include <QLabel>
#include <QLineEdit>
#include <QMessageBox>
#include <QNetworkRequest>
#include <QNetworkAccessManager>
#include <QObject>
#include <QPlainTextEdit>
#include <QProgressBar>
#include <QGuiApplication>
#include <QScreen>
#include <QSslConfiguration>
#include <QTimer>
#include <QUrl>
#include <QUrlQuery>
#include <QVBoxLayout>
#include "feedback.h"

QString Feedback::_errorMessage;

bool Feedback::showDialog(QWidget* parent, QUrl url) {
    QRect geometry = (parent != nullptr)
                     ? QDesktopWidget().availableGeometry(parent)
                     : QGuiApplication::primaryScreen()->availableGeometry();
    int dialogWidth = geometry.width() / 4;
    if (dialogWidth < 480) { dialogWidth = 480; }

    QDialog dialog(parent);
    dialog.resize(dialogWidth, 1);
    dialog.setWindowTitle("Send feedback");

    QVBoxLayout layout;
    dialog.setLayout(&layout);

    QLabel labelMessage;
    labelMessage.setText("What do you wish to report?");
    layout.addWidget(&labelMessage);

    QPlainTextEdit textMessage;
    textMessage.setTabChangesFocus(true);
    layout.addWidget(&textMessage);

    QGridLayout layoutSender;
    layout.addLayout(&layoutSender);

    QLabel labelEmail;
    labelEmail.setText("E-mail (optional):");
    layoutSender.addWidget(&labelEmail, 1, 1);

    QLineEdit textEmail;
    layoutSender.addWidget(&textEmail, 1, 2);

    QLabel labelDisplayName;
    labelDisplayName.setText("Name (optional):");
    layoutSender.addWidget(&labelDisplayName, 2, 1);

    QLineEdit textDisplayName;
    layoutSender.addWidget(&textDisplayName, 2, 2);

    QLabel labelAdditionalData;
    labelAdditionalData.setText("Additional data to be sent:");
    layout.addWidget(&labelAdditionalData);

    QPlainTextEdit textAdditionalData;
    textMessage.setTabChangesFocus(true);
    textAdditionalData.setReadOnly(true);
    QPalette readOnlyPalette = textAdditionalData.palette();
    readOnlyPalette.setColor(QPalette::Base, textAdditionalData.palette().color(QPalette::Window));
    textAdditionalData.setPalette(readOnlyPalette);
    textAdditionalData.appendPlainText("Environment:");
#ifndef QT_DEBUG
    textAdditionalData.appendPlainText("o " + QCoreApplication::applicationName() + " " + QCoreApplication::applicationVersion());
#else
    textAdditionalData.appendPlainText("o " + QCoreApplication::applicationName() + " " + QCoreApplication::applicationVersion() + " DEBUG");
#endif
    textAdditionalData.appendPlainText("o Qt " + QString(qVersion()) + " (" + APP_QT_VERSION + ")");
    textAdditionalData.appendPlainText("o " + QSysInfo::prettyProductName() + " (" + QSysInfo::kernelType() + " " + QSysInfo::kernelVersion() + ")");
    textAdditionalData.setTextCursor(QTextCursor());
    layout.addWidget(&textAdditionalData);

    QCheckBox checkAdditionalData;
    checkAdditionalData.setText("Send additional data above");
    checkAdditionalData.setCheckState(Qt::Checked);
    layout.addWidget(&checkAdditionalData);

    QDialogButtonBox buttonBox;
    buttonBox.setOrientation(Qt::Horizontal);
    buttonBox.setStandardButtons(QDialogButtonBox::Cancel | QDialogButtonBox::Ok);
    layout.addWidget(&buttonBox);

    QObject::connect(&buttonBox, &QDialogButtonBox::accepted, &dialog, &QDialog::accept);
    QObject::connect(&buttonBox, &QDialogButtonBox::rejected, &dialog, &QDialog::reject);

    if (dialog.exec() == QDialog::Accepted) {
        QString message = textMessage.toPlainText();
        QString additionalData = textAdditionalData.toPlainText();
        if (checkAdditionalData.isChecked() && (additionalData.length() > 0)) {
            if (message.length() > 0) { message += "\n\n"; }
            message += additionalData;
        }
        if (message.length() > 0) {
            QDialog progressDialog(parent);
            progressDialog.resize(dialogWidth / 2, 1);
            progressDialog.setWindowTitle("Send feedback");

            QVBoxLayout layoutProgress;
            progressDialog.setLayout(&layoutProgress);

            QProgressBar progress;
            progress.setTextVisible(false);
            progress.setRange(0, 0);
            layoutProgress.addWidget(&progress);

            progressDialog.show();

            bool successful = sendFeedback(message, textEmail.text(), textDisplayName.text(), url);

            progress.setMaximum(100);
            progress.setValue(100);

            QLabel labelResult;
            layoutProgress.addWidget(&labelResult);

            if (successful) {
                labelResult.setText("Feedback successfully sent.");
            } else {
                progress.setStyleSheet("QProgressBar::chunk { background-color: red; }");
                labelResult.setText("Error sending feedback.\n" + _errorMessage);
            }

            QDialogButtonBox buttonsProgress;
            buttonsProgress.setOrientation(Qt::Horizontal);
            buttonsProgress.setStandardButtons(successful ? QDialogButtonBox::Ok : QDialogButtonBox::Close);
            layoutProgress.addWidget(&buttonsProgress);

            QObject::connect(&buttonsProgress, &QDialogButtonBox::accepted, &progressDialog, &QDialog::accept);
            QObject::connect(&buttonsProgress, &QDialogButtonBox::rejected, &progressDialog, &QDialog::reject);

            progressDialog.exec();
            return successful;
        }
    }

    return false;
}

bool Feedback::sendFeedback(QString message, QString senderEmail, QString senderDisplayName, QUrl url) {
    QUrlQuery postData;
    postData.addQueryItem("Product", QCoreApplication::applicationName());
    postData.addQueryItem("Version", QCoreApplication::applicationVersion());
    postData.addQueryItem("Message", message);
    if (!senderEmail.isEmpty()) { postData.addQueryItem("Email", senderEmail); }
    if (!senderDisplayName.isEmpty()) { postData.addQueryItem("DisplayName", senderDisplayName); }

    QSslConfiguration sslConfig;
    sslConfig.setProtocol(QSsl::TlsV1_2OrLater);

    QNetworkRequest request(url);
    request.setHeader(QNetworkRequest::ContentTypeHeader, "application/x-www-form-urlencoded");
    request.setSslConfiguration(sslConfig);

    QTimer timer;
    timer.setSingleShot(true);
    timer.start(7000);

    QNetworkAccessManager network;
    QNetworkReply* reply = network.post(request,  postData.toString(QUrl::FullyEncoded).toUtf8());

    QEventLoop loop;
    loop.connect(reply, &QNetworkReply::finished, &loop, &QEventLoop::quit);
    loop.connect(&timer, &QTimer::timeout, &loop, &QEventLoop::quit);
    loop.exec();

    bool successful = (reply->error() == QNetworkReply::NoError);
    if (successful) {
        qDebug().noquote().nospace() << "[Feedback] Post result is" << reply->error();
    } else {
        qDebug().noquote().nospace() << "[Feedback] Post result is" << reply->error() << " (" << reply->errorString() << ")";
    }
    _errorMessage = reply->errorString();
    if (successful) {
        auto statusCode = reply->attribute(QNetworkRequest::HttpStatusCodeAttribute);
        if (statusCode.isValid()) {
            auto statusCodeReason = reply->attribute(QNetworkRequest::HttpReasonPhraseAttribute);
            if (statusCodeReason.isValid()){
                _errorMessage = "Status code: " + statusCodeReason.toString() + ".";
            } else {
                _errorMessage = "Status code: " + QString::number(statusCode.toInt()) + ".";
            }
            successful = false;
        }
    }
    delete reply;

    return successful;
}
