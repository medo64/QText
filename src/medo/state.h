/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

// 2019-07-15: Initial version
// 2020-01-01: Added widget methods

#ifndef MEDO_STATE_H
#define MEDO_STATE_H

#include <QMainWindow>
#include <QMutex>
#include <QObject>
#include <QString>

namespace Medo { class State; }

class State  : public QObject {
    Q_OBJECT

    public:

        /*! Returns singleton instance. */
        static State* instance();


        /*! Loads window state if one exists.
         * \param window Window. */
        static void load(QMainWindow* window);

        /*! Loads window state if one exists.
         * \param objectName Object name.
         * \param window Window. */
        static void load(QString objectName, QMainWindow* window);

        /*! Saves window state.
         * \param window Window. */
        static void save(QMainWindow* window);

        /*! Saves window state.
         * \param objectName Object name.
         * \param window Window. */
        static void save(QString objectName, QMainWindow* window);


        /*! Loads widget state if one exists.
         * \param widget Widget. */
        static void load(QWidget* widget);

        /*! Loads widget state if one exists.
         * \param objectName Object name.
         * \param widget Widget. */
        static void load(QString objectName, QWidget* widget);

        /*! Saves widget state.
         * \param widget Widget. */
        static void save(QWidget* widget);

        /*! Saves widget state.
         * \param objectName Object name.
         * \param widget Widget. */
        static void save(QString objectName, QWidget* widget);


    signals:

        /*! Signals a read from config is needed. If key cannot be found, null QString() should be returned. */
        QString readFromConfig(QString key);

        /*! Signals a write to config */
        void writeToConfig(QString key, QString value);


    private:
        explicit State();
        ~State() override;
        State(const State&) = delete;
        State& operator=(const State&) = delete;
        static State _instance;

    private:
        void loadEx(QString objectName, QMainWindow* window);
        void saveEx(QString objectName, QMainWindow* window);
        void loadEx(QString objectName, QWidget* widget);
        void saveEx(QString objectName, QWidget* widget);

};

#endif // MEDO_STATE_H
