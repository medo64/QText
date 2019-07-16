#ifndef STATE_H
#define STATE_H

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

    private:
        QString readValue(QString key);
        QString adjustValue(QString value, QString defaultValue);
        int adjustValue(QString value, int defaultValue);
        //void writeValue(QString key, QString value);
        //void writeValue(QString key, int value);

};

#endif // STATE_H
