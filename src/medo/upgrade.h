/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

// 2021-12-12: Fixed TLS 1.3 connection
// 2020-10-23: Initial version

#pragma once
#include <QElapsedTimer>
#include <QFile>
#include <QNetworkRequest>
#include <QProgressBar>
#include <QUrl>
#include <QWidget>

namespace Medo {
    class Upgrade;
    class UpgradeFile;
}

class UpgradeFile;

class Upgrade {

    public:
        /*! Shows upgrade dialog. Returns true if upgrade has started. */
        static bool showDialog(QWidget* parent, QUrl serviceUrl);

        /*! Shows upgrade dialog. Returns true if upgrade has started. */
        static bool showDialog(QWidget* parent, QString serviceUrl);

        /*! Returns upgrade file. */
        static UpgradeFile upgradeFile(QUrl serviceUrl);

        /*! Returns upgrade file. */
        static UpgradeFile upgradeFile(QString serviceUrl);

        /*! Returns true if upgrade is available. */
        static bool available(QUrl url);

        /*! Returns true if upgrade is available. */
        static bool available(QString url);

    private:
        static UpgradeFile getUpgradeFileFromURL(QElapsedTimer* stopwatch, QUrl url);
        static QString processUrl(QUrl url, int* statusCode);

};

class UpgradeFile {
        friend class Upgrade;

    public:
        /*! Returns true if upgrade is available. */
        bool available();

        /*! Gets URL for upgrade file. Empty if no upgrade file is found. */
        QUrl fileUrl();

        /*! Gets upgrade file name. Empty if no upgrade file is found. */
        QString fileName();

        /*! Returns suggested file name in temporary directory. */
        QString suggestedLocalFileName();

        /*! Downloads full file and returns true if successful. */
        bool downloadFile(QString fileName, int timeout = 30000, QProgressBar* progressBar = nullptr);

    private:
        explicit UpgradeFile();
        explicit UpgradeFile(QUrl fileUrl);
        QUrl _downloadUrl;

};
