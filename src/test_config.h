#ifndef TEST_CONFIG_H
#define TEST_CONFIG_H

#include <QObject>
#include <QtTest>

class Test_Config : public QObject {
    Q_OBJECT

    private slots:
        void paths();
        void pathsWithSpaces();

};

#endif // TEST_CONFIG_H
