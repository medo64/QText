#include "test_config.h"
#include <QDir>
#include <QFile>

#include "config.h"

void Test_Config::paths() {
    Config::reset();
#if defined(Q_OS_WIN)
    QDir baseDir(QDir::homePath() + "/AppData/Roaming/Testing");
    if (baseDir.exists()) { baseDir.removeRecursively(); }
    QVERIFY(!baseDir.exists());
#else
    QDir dataDir(QDir::homePath() + "/.local/share/test");
    if (dataDir.exists()) { dataDir.removeRecursively(); }
    QVERIFY(!dataDir.exists());

    QFile configFile(QDir::homePath() + "/.config/test.conf");
    if (configFile.exists()) { configFile.remove(); }
    QVERIFY(!configFile.exists());
#endif

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

    QFile finalConfigFile (expectedConfigurationFile);
    QVERIFY(finalConfigFile.exists());

    QDir finalDataDir (expectedDataDirectory);
    QVERIFY(finalDataDir.exists());
}

void Test_Config::pathsWithSpaces() {
    Config::reset();
#if defined(Q_OS_WIN)
    QDir baseDir(QDir::homePath() + "/AppData/Roaming/Testing");
    if (baseDir.exists()) { baseDir.removeRecursively(); }
    QVERIFY(!baseDir.exists());
#else
    QDir dataDir(QDir::homePath() + "/.local/share/testx");
    if (dataDir.exists()) { dataDir.removeRecursively(); }
    QVERIFY(!dataDir.exists());

    QFile configFile(QDir::homePath() + "/.config/testx.conf");
    if (configFile.exists()) { configFile.remove(); }
    QVERIFY(!configFile.exists());
#endif

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

    QFile finalConfigFile (expectedConfigurationFile);
    QVERIFY(finalConfigFile.exists());

    QDir finalDataDir (expectedDataDirectory);
    QVERIFY(finalDataDir.exists());
}
