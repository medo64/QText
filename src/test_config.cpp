#include "test_config.h"
#include "config.h"

void Test_Config::paths() {
    Config::reset();
    QCoreApplication::setApplicationName("Test");
    QCoreApplication::setOrganizationName("Testing");

#if defined(Q_OS_WIN)
    QString expectedConfigurationFile = QDir::homePath() + "/AppData/Roaming/Testing/Test/Test.cfg";
    QString expectedDataDirectory = QDir::homePath() + "/AppData/Roaming/Testing/Test/Data";
#else
    QString expectedConfigurationFile = QDir::homePath() + "/.config/test.conf";
    QString expectedDataDirectory = QDir::homePath() + "/.local/share/test";
#endif

    QCOMPARE(Config::getConfigurationFile(), expectedConfigurationFile);
    QCOMPARE(Config::getDataDirectory(), expectedDataDirectory);
}

void Test_Config::pathsWithSpaces() {
    Config::reset();
    QCoreApplication::setApplicationName("Test X");
    QCoreApplication::setOrganizationName("Testing");

#if defined(Q_OS_WIN)
    QString expectedConfigurationFile = QDir::homePath() + "/AppData/Roaming/Testing/Test X/Test X.cfg";
    QString expectedDataDirectory = QDir::homePath() + "/AppData/Roaming/Testing/Test X/Data";
#else
    QString expectedConfigurationFile = QDir::homePath() + "/.config/testx.conf";
    QString expectedDataDirectory = QDir::homePath() + "/.local/share/testx";
#endif

    QCOMPARE(Config::getConfigurationFile(), expectedConfigurationFile);
    QCOMPARE(Config::getDataDirectory(), expectedDataDirectory);
}
