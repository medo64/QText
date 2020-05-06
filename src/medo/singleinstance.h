/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

// 2019-07-06: Initial version

#ifndef MEDO_SINGLEINSTANCE_H
#define MEDO_SINGLEINSTANCE_H

#include <QLocalServer>
#include <QMutex>
#include <QObject>

namespace Medo { class SingleInstance; }

class SingleInstance : public QObject {
    Q_OBJECT

    public:

        /*! Returns singleton instance. */
        static SingleInstance* instance();

        /*! Returns true if current application is the first instance. */
        static bool attach();

        /*! Returns true if there is another instance running. */
        static bool isOtherInstanceRunning();

    signals:

        /*! Signals that a new instance attempted attach operation. */
        void newInstanceDetected();


    private:
        explicit SingleInstance();
        ~SingleInstance() override;
        SingleInstance(const SingleInstance&) = delete;
        SingleInstance& operator=(const SingleInstance&) = delete;
        static SingleInstance _instance;
        static QMutex _mutex;
        static QLocalServer* _server;
        static bool _isFirstInstance;

    private slots:
        void onNewConnection();

};

#endif // MEDO_SINGLEINSTANCE_H
