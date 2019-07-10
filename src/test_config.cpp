#include "test_config.h"
#include <QDir>
#include <QFile>

//#define private public
#include "config.h"

void Test_Config::paths() {
#if defined(Q_OS_WIN)
    QDir baseDirWhenInstalled(QDir::homePath() + "/AppData/Roaming/Testing");
    if (baseDirWhenInstalled.exists()) { baseDirWhenInstalled.removeRecursively(); }
    assert(!baseDirWhenInstalled.exists());

    QFile configFileWhenPortable(QCoreApplication::applicationDirPath() + "/.testx.conf");
    if (configFileWhenPortable.exists()) { configFileWhenPortable.remove(); }
    assert(!configFileWhenPortable.exists());

    QDir dataDirWhenPortable(QCoreApplication::applicationDirPath() + "/.testx.data");
    if (dataDirWhenPortable.exists()) { dataDirWhenPortable.removeRecursively(); }
    assert(!dataDirWhenPortable.exists());
#elif defined(Q_OS_LINUX)
    QFile configFileWhenInstalled(QDir::homePath() + "/.config/test.conf");
    if (configFileWhenInstalled.exists()) { configFileWhenInstalled.remove(); }
    assert(!configFileWhenInstalled.exists());

    QDir dataDirWhenInstalled(QDir::homePath() + "/.local/share/test");
    if (dataDirWhenInstalled.exists()) { dataDirWhenInstalled.removeRecursively(); }
    assert(!dataDirWhenInstalled.exists());

    QFile configFileWhenPortable(QCoreApplication::applicationDirPath() + "/.test.conf");
    if (configFileWhenPortable.exists()) { configFileWhenPortable.remove(); }
    assert(!configFileWhenPortable.exists());

    QDir dataDirWhenPortable(QCoreApplication::applicationDirPath() + "/.test.data");
    if (dataDirWhenPortable.exists()) { dataDirWhenPortable.removeRecursively(); }
    assert(!dataDirWhenPortable.exists());
#endif

    QCoreApplication::setApplicationName("Test");
    QCoreApplication::setOrganizationName("Testing");

#if defined(Q_OS_WIN)
    QString expectedConfigurationFileWhenInstalled = QDir::homePath() + "/AppData/Roaming/Testing/Test/Test.cfg";
    QString expectedDataDirectoryWhenInstalled = QDir::homePath() + "/AppData/Roaming/Testing/Test/Data";
    QString expectedConfigurationFileWhenPortable = QCoreApplication::applicationDirPath() + "/Test.cfg";
    QString expectedDataDirectoryWhenPortable = QCoreApplication::applicationDirPath() + "/Test.Data";
#elif defined(Q_OS_LINUX)
    QString expectedConfigurationFileWhenInstalled = QDir::homePath() + "/.config/test.conf";
    QString expectedDataDirectoryWhenInstalled = QDir::homePath() + "/.local/share/test";
    QString expectedConfigurationFileWhenPortable = QCoreApplication::applicationDirPath() + "/.test";
    QString expectedDataDirectoryWhenPortable = QCoreApplication::applicationDirPath() + "/.test.data";
#endif

    {
        Config::setPortable(false);

        QCOMPARE(Config::configurationFile(), expectedConfigurationFileWhenInstalled);
        QCOMPARE(Config::dataDirectory(), expectedDataDirectoryWhenInstalled);
        QCOMPARE(Config::isPortable(), false);

        QFile finalConfigFile (expectedConfigurationFileWhenInstalled);
        QVERIFY(finalConfigFile.exists());

        QDir finalDataDir (expectedDataDirectoryWhenInstalled);
        QVERIFY(finalDataDir.exists());

        baseDirWhenInstalled.removeRecursively(); //cleanup
    }

    {
        Config::setPortable(true);

        QCOMPARE(Config::configurationFile(), expectedConfigurationFileWhenPortable);
        QCOMPARE(Config::dataDirectory(), expectedDataDirectoryWhenPortable);
        QCOMPARE(Config::isPortable(), true);

        QFile finalConfigFile (expectedConfigurationFileWhenPortable);
        QVERIFY(finalConfigFile.exists());
        configFileWhenPortable.remove(); //cleanup

        QDir finalDataDir (expectedDataDirectoryWhenPortable);
        QVERIFY(finalDataDir.exists());
        dataDirWhenPortable.removeRecursively(); //cleanup
    }
}

void Test_Config::pathsWithSpaces() {
#if defined(Q_OS_WIN)
    QDir baseDirWhenInstalled(QDir::homePath() + "/AppData/Roaming/Testing");
    if (baseDirWhenInstalled.exists()) { baseDirWhenInstalled.removeRecursively(); }
    QVERIFY(!baseDirWhenInstalled.exists());

    QFile configFileWhenPortable(QCoreApplication::applicationDirPath() + "/.testx.conf");
    if (configFileWhenPortable.exists()) { configFileWhenPortable.remove(); }
    assert(!configFileWhenPortable.exists());

    QDir dataDirWhenPortable(QCoreApplication::applicationDirPath() + "/.testx.data");
    if (dataDirWhenPortable.exists()) { dataDirWhenPortable.removeRecursively(); }
    assert(!dataDirWhenPortable.exists());
#elif defined(Q_OS_LINUX)
    QFile configFileWhenInstalled(QDir::homePath() + "/.config/testx.conf");
    if (configFileWhenInstalled.exists()) { configFileWhenInstalled.remove(); }
    assert(!configFileWhenInstalled.exists());

    QDir dataDirWhenInstalled(QDir::homePath() + "/.local/share/testx");
    if (dataDirWhenInstalled.exists()) { dataDirWhenInstalled.removeRecursively(); }
    assert(!dataDirWhenInstalled.exists());

    QFile configFileWhenPortable(QCoreApplication::applicationDirPath() + "/.testx.conf");
    if (configFileWhenPortable.exists()) { configFileWhenPortable.remove(); }
    assert(!configFileWhenPortable.exists());

    QDir dataDirWhenPortable(QCoreApplication::applicationDirPath() + "/.testx.data");
    if (dataDirWhenPortable.exists()) { dataDirWhenPortable.removeRecursively(); }
    assert(!dataDirWhenPortable.exists());
#endif

    QCoreApplication::setApplicationName("Test X");
    QCoreApplication::setOrganizationName("Testing");

#if defined(Q_OS_WIN)
    QString expectedConfigurationFileWhenInstalled = QDir::homePath() + "/AppData/Roaming/Testing/Test X/Test X.cfg";
    QString expectedDataDirectoryWhenInstalled = QDir::homePath() + "/AppData/Roaming/Testing/Test X/Data";
    QString expectedConfigurationFileWhenPortable = QCoreApplication::applicationDirPath() + "/Test X.cfg";
    QString expectedDataDirectoryWhenPortable = QCoreApplication::applicationDirPath() + "/Test X.Data";
#elif defined(Q_OS_LINUX)
    QString expectedConfigurationFileWhenInstalled = QDir::homePath() + "/.config/testx.conf";
    QString expectedDataDirectoryWhenInstalled = QDir::homePath() + "/.local/share/testx";
    QString expectedConfigurationFileWhenPortable = QCoreApplication::applicationDirPath() + "/.testx";
    QString expectedDataDirectoryWhenPortable = QCoreApplication::applicationDirPath() + "/.testx.data";
#endif

    {
        Config::setPortable(true);

        QCOMPARE(Config::dataDirectory(), expectedDataDirectoryWhenPortable);
        QCOMPARE(Config::configurationFile(), expectedConfigurationFileWhenPortable);
        QCOMPARE(Config::isPortable(), true);

        QFile finalConfigFile (expectedConfigurationFileWhenPortable);
        QVERIFY(finalConfigFile.exists());
        configFileWhenPortable.remove(); //cleanup

        QDir finalDataDir (expectedDataDirectoryWhenPortable);
        QVERIFY(finalDataDir.exists());
        dataDirWhenPortable.removeRecursively(); //cleanup
    }

    {
        Config::setPortable(false);

        QCOMPARE(Config::dataDirectory(), expectedDataDirectoryWhenInstalled);
        QCOMPARE(Config::configurationFile(), expectedConfigurationFileWhenInstalled);
        QCOMPARE(Config::isPortable(), false);

        QFile finalConfigFile (expectedConfigurationFileWhenInstalled);
        QVERIFY(finalConfigFile.exists());

        QDir finalDataDir (expectedDataDirectoryWhenInstalled);
        QVERIFY(finalDataDir.exists());

        baseDirWhenInstalled.removeRecursively(); //cleanup
    }
}
