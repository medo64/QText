#include <QDir>
#include <QFile>
#include "test_config.h"

#include "medo/config.h"

void Test_Config::setup(QString applicationName, QString organizationName) {
    QCoreApplication::setApplicationName(applicationName);
    QCoreApplication::setOrganizationName(organizationName);
    Config::setPortable(true);
    cleanup();
}

void Test_Config::setup(QString applicationName, QString organizationName, QString testConfigFile) {
    setup(applicationName, organizationName);

    QString fileName(":/test/Config/" + testConfigFile);
    QFile source(fileName);
    QFile destination(Config::configurationFilePath());
    if (source.open(QFile::ReadOnly)) {
        QTextStream in(&source);
        QString sourceContent = in.readAll(); //we'll handle new-line ourselves
        if (destination.open(QFile::WriteOnly)) {
            QTextStream out(&destination);
            out << sourceContent;
        } else {
            QFAIL("Cannot setup destination file.");
        }
    } else {
        QFAIL("Cannot setup source file.");
    }
}

void Test_Config::cleanup() {
    cleanup(Config::configurationFilePath(), Config::dataDirectoryPath());
}

void Test_Config::cleanup(QString configFile, QString dataDirectory) {
    QFile file(configFile);
    if (file.exists()) { file.remove(); }

    QDir dir(dataDirectory);
    if (dir.exists()) { dir.removeRecursively(); }
}

void Test_Config::verifyTestConfig(QString testConfigFile) {
    QFile expectedFile(":/test/Config/" + testConfigFile);
    QStringList expectedFileLines;
    if (expectedFile.open(QFile::ReadOnly)) {
        QTextStream in(&expectedFile);
        in.setCodec("UTF-8");
        while(!in.atEnd()) {
            expectedFileLines.append(in.readLine());
        }
    } else {
        QFAIL("Cannot open source file!");
    }

    QFile actualFile(Config::configurationFilePath());
    QStringList actualFileLines;
    if (actualFile.open(QFile::ReadOnly)) {
        QTextStream in(&actualFile);
        in.setCodec("UTF-8");
        while(!in.atEnd()) {
            actualFileLines.append(in.readLine());
        }
    } else {
        QFAIL("Cannot open config file!");
    }

    for (int i=0; i<std::min(actualFileLines.length(), expectedFileLines.length()); i++) {
        QCOMPARE(actualFileLines[i], expectedFileLines[i]);
    }

    QCOMPARE(actualFileLines.length(), expectedFileLines.length());
}


void Test_Config::paths() {
    setup("Test", "Testing");
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
    cleanup(expectedConfigurationFileWhenInstalled, expectedDataDirectoryWhenInstalled);
    cleanup(expectedConfigurationFileWhenPortable, expectedDataDirectoryWhenPortable);

    {
        Config::setPortable(false);

        QCOMPARE(Config::configurationFile(), expectedConfigurationFileWhenInstalled);
        QCOMPARE(Config::dataDirectory(), expectedDataDirectoryWhenInstalled);
        QCOMPARE(Config::isPortable(), false);

        QFile finalConfigFile (expectedConfigurationFileWhenInstalled);
        QVERIFY(finalConfigFile.exists());
        finalConfigFile.remove(); //cleanup

        QDir finalDataDir (expectedDataDirectoryWhenInstalled);
        QVERIFY(finalDataDir.exists());
        finalDataDir.removeRecursively(); //cleanup
    }

    {
        Config::setPortable(true);

        QCOMPARE(Config::configurationFile(), expectedConfigurationFileWhenPortable);
        QCOMPARE(Config::dataDirectory(), expectedDataDirectoryWhenPortable);
        QCOMPARE(Config::isPortable(), true);

        QFile finalConfigFile (expectedConfigurationFileWhenPortable);
        QVERIFY(finalConfigFile.exists());
        finalConfigFile.remove(); //cleanup

        QDir finalDataDir (expectedDataDirectoryWhenPortable);
        QVERIFY(finalDataDir.exists());
        finalDataDir.removeRecursively(); //cleanup
    }
}

void Test_Config::pathsWithSpaces() {
    setup("Test X", "Testing");
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
    cleanup(expectedConfigurationFileWhenInstalled, expectedDataDirectoryWhenInstalled);
    cleanup(expectedConfigurationFileWhenPortable, expectedDataDirectoryWhenPortable);

    {
        Config::setPortable(true);

        QCOMPARE(Config::dataDirectory(), expectedDataDirectoryWhenPortable);
        QCOMPARE(Config::configurationFile(), expectedConfigurationFileWhenPortable);
        QCOMPARE(Config::isPortable(), true);

        QFile finalConfigFile (expectedConfigurationFileWhenPortable);
        QVERIFY(finalConfigFile.exists());
        finalConfigFile.remove(); //cleanup

        QDir finalDataDir (expectedDataDirectoryWhenPortable);
        QVERIFY(finalDataDir.exists());
        finalDataDir.removeRecursively(); //cleanup
    }

    {
        Config::setPortable(false);

        QCOMPARE(Config::dataDirectory(), expectedDataDirectoryWhenInstalled);
        QCOMPARE(Config::configurationFile(), expectedConfigurationFileWhenInstalled);
        QCOMPARE(Config::isPortable(), false);

        QFile finalConfigFile (expectedConfigurationFileWhenInstalled);
        QVERIFY(finalConfigFile.exists());
        finalConfigFile.remove(); //cleanup

        QDir finalDataDir (expectedDataDirectoryWhenInstalled);
        QVERIFY(finalDataDir.exists());
        finalDataDir.removeRecursively(); //cleanup
    }
}

void Test_Config::reset() {
    setup("Test Reset", "Testing");
    cleanup();

    Config::reset();
    bool originalIsPortable = Config::isPortable();
    Config::setPortable(!originalIsPortable);
    QCOMPARE(Config::isPortable(), !originalIsPortable);

    Config::reset();
    QCOMPARE(Config::isPortable(), originalIsPortable);
}

void Test_Config::nullKey() {
    setup("Test", "Testing");
    QString value = Config::read(QString(), "default");
    QVERIFY(value.isNull());
}

void Test_Config::emptyKey() {
    setup("Test", "Testing");
    QString value = Config::read("", "default");
    QVERIFY(value.isNull());
}

void Test_Config::emptySave() { //Empty file Load/Save
    setup("Test", "Testing", "Empty.cfg");
    QVERIFY2(Config::load(), "File should exist before load.");
    QVERIFY2(Config::save(), "Save should succeed.");
    verifyTestConfig("Empty.cfg");
}

void Test_Config::emptyLinesCrLf() { //CRLF preserved on Save
    setup("Test", "Testing", "EmptyLinesCRLF.cfg");
    QVERIFY2(Config::save(), "Save should succeed.");
    verifyTestConfig("EmptyLinesCRLF.cfg");
}

void Test_Config::emptyLinesLf() { //LF preserved on Save
    setup("Test", "Testing", "EmptyLinesLF.cfg");
    QVERIFY2(Config::save(), "Save should succeed.");
    verifyTestConfig("EmptyLinesLF.cfg");
}

void Test_Config::emptyLinesCr() { //CR preserved on Save
    setup("Test", "Testing", "EmptyLinesCR.cfg");
    QVERIFY2(Config::save(), "Save should succeed.");
    verifyTestConfig("EmptyLinesCR.cfg");
}

void Test_Config::emptyLinesMixed() { //Mixed line ending gets normalized on Save
    setup("Test", "Testing", "EmptyLinesMixed.cfg");
    QVERIFY2(Config::save(), "Save should succeed.");
    verifyTestConfig("EmptyLinesMixed.Good.cfg");
}

void Test_Config::commentsOnly() { //Comments are preserved on Save
    setup("Test", "Testing", "CommentsOnly.cfg");
    QVERIFY2(Config::save(), "Save should succeed.");
    verifyTestConfig("CommentsOnly.cfg");
}

void Test_Config::commentsWithValues() { //Values with comments are preserved on Save
    setup("Test", "Testing", "CommentsWithValues.cfg");
    QVERIFY2(Config::save(), "Save should succeed.");
    verifyTestConfig("CommentsWithValues.cfg");
}

void Test_Config::spacingEscape() { //Leading spaces are preserved on Save
    setup("Test", "Testing", "SpacingEscape.cfg");
    QCOMPARE(Config::read("Key1", QString()), " Value 1");
    QCOMPARE(Config::read("Key2", QString()), "Value 2 ");
    QCOMPARE(Config::read("Key3", QString()), " Value 3 ");
    QCOMPARE(Config::read("Key4", QString()), "  Value 4  ");
    QCOMPARE(Config::read("Key5", QString()), "\tValue 5\t");
    QCOMPARE(Config::read("Key6", QString()), "\tValue 6");
    QCOMPARE(Config::read("Null", QString()), QString(QChar('\0')));
    Config::save();

    Config::write("Null", QString(QChar('\0')) + "Null" + QString(QChar('\0')));
    Config::save();
    verifyTestConfig("SpacingEscape.Good.cfg");
}

void Test_Config::writeBasic() { //Basic write
    setup("Test", "Testing", "Empty.cfg");
    Config::write("Key1", "Value 1");
    Config::write("Key2", "Value 2");
    Config::save();
    verifyTestConfig("WriteBasic.Good.cfg");
}

void Test_Config::writeNoEmptyLine() { //Basic write (without empty line ending)
    setup("Test", "Testing", "WriteNoEmptyLine.cfg");
    Config::write("Key1", "Value 1");
    Config::write("Key2", "Value 2");
    Config::save();
    verifyTestConfig("WriteNoEmptyLine.Good.cfg");
}

void Test_Config::writeSameSeparatorEquals() { //Separator equals (=) is preserved upon save
    setup("Test", "Testing", "WriteSameSeparatorEquals.cfg");
    Config::write("Key1", "Value 1");
    Config::write("Key2", "Value 2");
    Config::save();
    verifyTestConfig("WriteSameSeparatorEquals.Good.cfg");
}

void Test_Config::writeSameSeparatorSpace() { //Separator space ( ) is preserved upon save
    setup("Test", "Testing", "WriteSameSeparatorSpace.cfg");
    Config::write("Key1", "Value 1");
    Config::write("Key2", "Value 2");
    Config::save();
    verifyTestConfig("WriteSameSeparatorSpace.Good.cfg");
}

void Test_Config::replace() { //Write replaces existing entry
    setup("Test", "Testing", "Replace.cfg");
    Config::write("Key1", "Value 1a");
    Config::write("Key2", "Value 2a");
    Config::save();
    QCOMPARE(Config::read("Key1", QString()), "Value 1a");
    QCOMPARE(Config::read("Key2", QString()), "Value 2a");
    verifyTestConfig("Replace.Good.cfg");
}

void Test_Config::spacingPreserved() { //Write preserves spacing
    setup("Test", "Testing", "SpacingPreserved.cfg");
    Config::write("KeyOne", "Value 1a");
    Config::write("KeyTwo", "Value 2b");
    Config::write("KeyThree", "Value 3c");
    Config::save();
    verifyTestConfig("SpacingPreserved.Good.cfg");
}

void Test_Config::spacingPreservedOnAdd() { //Write preserves spacing on add
    setup("Test", "Testing", "SpacingPreservedOnAdd.cfg");
    Config::write("One", "Value 1a");
    Config::writeMany("Two", QStringList({ "Value 2a", "Value 2b" }));
    Config::write("Three", "Value 3a");
    Config::write("Four", "Value 4a");
    Config::writeMany("Five", QStringList({ "Value 5a", "Value 5b", "Value 5c" }));
    Config::write("FourtyTwo", 42);
    Config::save();
    verifyTestConfig("SpacingPreservedOnAdd.Good.cfg");
}

void Test_Config::writeToEmpty() { //Write without preexisting file
    setup("Test", "Testing");
    Config::write("Key1", "Value 1a");
    Config::write("Key2", "Value 2a");

    QFile configFile(Config::configurationFilePath());
    configFile.remove();

    Config::save();
    verifyTestConfig("Replace.Good.cfg");
}

void Test_Config::replaceOnlyLast() { //Write replaces only the last instance of same key
    setup("Test", "Testing", "ReplaceOnlyLast.cfg");
    Config::write("Key1", "Value 1a");
    Config::write("Key2", "Value 2a");
    Config::save();

    QCOMPARE(Config::read("Key1", QString()), "Value 1a");
    QCOMPARE(Config::read("Key2", QString()), "Value 2a");
    QCOMPARE(Config::read("Key3", QString()), "Value 3");

    verifyTestConfig("ReplaceOnlyLast.Good.cfg");
}

void Test_Config::removeSingle() { //Removing entry
    setup("Test", "Testing", "Remove.cfg");
    Config::remove("Key1");
    Config::save();

    verifyTestConfig("Remove.Good.cfg");
}

void Test_Config::removeMulti() { //Removing multiple entries
    setup("Test", "Testing", "RemoveMulti.cfg");
    Config::remove("Key2");
    Config::save();

    verifyTestConfig("RemoveMulti.Good.cfg");
}

void Test_Config::readMulti() { //Reading multiple entries
    setup("Test", "Testing", "ReplaceOnlyLast.Good.cfg");
    QStringList values = Config::readMany("Key2");
    QCOMPARE(values.length(), 2);
    QCOMPARE(values[0], "Value 2");
    QCOMPARE(values[1], "Value 2a");
}

void Test_Config::multiWrite() { //Multi-value write
    setup("Test", "Testing");
    Config::write("Key1", "Value 1");
    Config::writeMany("Key2", QStringList({ "Value 2a", "Value 2b", "Value 2c" }));
    Config::write("Key3", "Value 3");
    Config::save();
    verifyTestConfig("WriteMulti.Good.cfg");

    QCOMPARE(Config::read("Key1"), "Value 1");
    QCOMPARE(Config::read("Key3"), "Value 3");

    QStringList values = Config::readMany("Key2");
    QCOMPARE(values.length(), 3);
    QCOMPARE(values[0], "Value 2a");
    QCOMPARE(values[1], "Value 2b");
    QCOMPARE(values[2], "Value 2c");
}

void Test_Config::multiReplace() { //Multi-value replace
    setup("Test", "Testing", "WriteMulti.cfg");
    Config::writeMany("Key2", QStringList({ "Value 2a", "Value 2b", "Value 2c" }));
    Config::save();
    verifyTestConfig("WriteMulti.Good.cfg");

    QCOMPARE(Config::read("Key1"), "Value 1");
    QCOMPARE(Config::read("Key3"), "Value 3");

    QStringList values = Config::readMany("Key2");
    QCOMPARE(values.length(), 3);
    QCOMPARE(values[0], "Value 2a");
    QCOMPARE(values[1], "Value 2b");
    QCOMPARE(values[2], "Value 2c");
}

void Test_Config::testConversionWrite() { //Test conversion
    setup("Test", "Testing");

    Config::write("Integer", 42);
    Config::write("Integer Min", std::numeric_limits<int32_t>::min());
    Config::write("Integer Max", std::numeric_limits<int32_t>::max());
    Config::write("Long", static_cast<long long>(42));
    Config::write("Long Min", static_cast<long long>(std::numeric_limits<int64_t>::min()));
    Config::write("Long Max", static_cast<long long>(std::numeric_limits<int64_t>::max()));
    Config::write("Boolean", true);
    Config::write("Double", static_cast<double>(42.42));
    Config::write("Double Pi", static_cast<double>(3.1415926535897931));
    Config::write("Double Third", static_cast<double>(1.0) / 3);
    Config::write("Double Seventh", static_cast<double>(1.0) / 7);
    Config::write("Double Min", std::numeric_limits<double>::lowest());
    Config::write("Double Max", std::numeric_limits<double>::max());
    Config::write("Double NaN", std::numeric_limits<double>::quiet_NaN());
    Config::write("Double Infinity+", std::numeric_limits<double>::infinity());
    Config::write("Double Infinity-", -std::numeric_limits<double>::infinity());

    Config::save();
    verifyTestConfig("WriteConvertedCpp.Good.cfg");
}

void Test_Config::testConversionRead() { //Test conversion
    setup("Test", "Testing", "WriteConvertedCpp.Good.cfg");

    QCOMPARE(Config::read("Integer", 0), 42);
    QCOMPARE(Config::read("Integer Min", 0), std::numeric_limits<int32_t>::min());
    QCOMPARE(Config::read("Integer Max", 0), std::numeric_limits<int32_t>::max());
    QCOMPARE(Config::read("Long", static_cast<long long>(0)), static_cast<long long>(42));
    QCOMPARE(Config::read("Long Min", static_cast<long long>(0)), std::numeric_limits<int64_t>::min());
    QCOMPARE(Config::read("Long Max", static_cast<long long>(0)), std::numeric_limits<int64_t>::max());
    QCOMPARE(Config::read("Boolean", false), true);
    QCOMPARE(Config::read("Double", 0.0), 42.42);
    QCOMPARE(Config::read("Double Pi", 0.0), static_cast<double>(3.1415926535897931));
    QCOMPARE(Config::read("Double Third", 0.0), static_cast<double>(1.0) / 3);
    QCOMPARE(Config::read("Double Seventh", 0.0), static_cast<double>(1.0) / 7);
    QCOMPARE(Config::read("Double Min", 0.0), std::numeric_limits<double>::lowest());
    QCOMPARE(Config::read("Double Max", 0.0), std::numeric_limits<double>::max());
    QCOMPARE(Config::read("Double NaN", 0.0), std::numeric_limits<double>::quiet_NaN());
    QCOMPARE(Config::read("Double Infinity+", 0.0), std::numeric_limits<double>::infinity());
    QCOMPARE(Config::read("Double Infinity-", 0.0), -std::numeric_limits<double>::infinity());
}

void Test_Config::keyWhitespace() { //Key whitespace reading and saving
    setup("Test", "Testing", "KeyWhitespace.cfg");

    Config::save();
    verifyTestConfig("KeyWhitespace.Good.cfg");

    QCOMPARE(Config::read("Key 1"), "Value 1");
    QCOMPARE(Config::read("Key 3"), "Value 3");

    QStringList values = Config::readMany("Key 2");
    QCOMPARE(values.length(), 3);
    QCOMPARE(values[0], "Value 2a");
    QCOMPARE(values[1], "Value 2b");
    QCOMPARE(values[2], "Value 2c");
}

void Test_Config::deleteAll() { //Delete all values
    setup("Test", "Testing", "WriteBasic.Good.cfg");
    Config::setImmediateSave(false);

    QCOMPARE(Config::read("Key1"), "Value 1");
    QCOMPARE(Config::read("Key2"), "Value 2");
    Config::removeAll();

    QVERIFY(Config::read("Key1").isNull());
    QVERIFY(Config::read("Key2").isNull());

    verifyTestConfig("WriteBasic.Good.cfg");

    Config::save();
    verifyTestConfig("Empty.cfg");
}

void Test_Config::twoReadsWithDifferentDefault() { //Each read must independently define default
    setup("Test", "Testing", "Empty.cfg");

    QCOMPARE(Config::read("NonexistentKey", "Default 1"), "Default 1");
    QCOMPARE(Config::read("NonexistentKey", "Default 2"), "Default 2");
}

void Test_Config::writeOverridesReads() { //Each read must not return default if there was a write
    setup("Test", "Testing", "Empty.cfg");

    Config::write("SomeKey", "SomeValue");

    QCOMPARE(Config::read("SomeKey", "Default 1"), "SomeValue");
    QCOMPARE(Config::read("SomeKey", "Default 2"), "SomeValue");
}

void Test_Config::writeChangesOnlyLastEntry() { //write a single entry will override only the last member
    setup("Test", "Testing", "Empty.cfg");

    Config::writeMany("SomeKey", QStringList({ "Value 1", "Value 2" }));
    QCOMPARE(Config::readMany("SomeKey")[0], "Value 1");
    QCOMPARE(Config::readMany("SomeKey")[1], "Value 2");

    Config::write("SomeKey", "Value New");
    QCOMPARE(Config::readMany("SomeKey")[0], "Value 1");
    QCOMPARE(Config::readMany("SomeKey")[1], "Value New");
}

void Test_Config::removeCausesDefault() { //Each read must return default if previous write has been removed
    setup("Test", "Testing", "Empty.cfg");

    Config::write("SomeKey", "SomeValue");
    QCOMPARE(Config::read("SomeKey"), "SomeValue");

    Config::remove("SomeKEY");

    QCOMPARE(Config::read("SomeKey", "Default 1"), "Default 1");
    QCOMPARE(Config::read("SomeKey", "Default 2"), "Default 2");
}

void Test_Config::removeAllCausesDefault() { //Each read must return default if previous write has been removed
    setup("Test", "Testing", "Empty.cfg");

    Config::write("SomeKey", "SomeValue");
    QCOMPARE(Config::read("SomeKey"), "SomeValue");

    Config::removeAll();

    QCOMPARE(Config::read("SomeKey", "Default 1"), "Default 1");
    QCOMPARE(Config::read("SomeKey", "Default 2"), "Default 2");
}

void Test_Config::caseInsensitiveDefaults() {
    setup("Test", "Testing", "Empty.cfg");

    QCOMPARE(Config::read("NON-EXISTENT KEY", "Default 1"), "Default 1");
    QCOMPARE(Config::read("non-existent key", "Default 2"), "Default 2");
}

void Test_Config::caseInsensitiveWrites() {
    setup("Test", "Testing", "Empty.cfg");

    Config::write("Some Key", "SomeValue");

    QCOMPARE(Config::read("SOME KEY", "Default 1"), "SomeValue");
    QCOMPARE(Config::read("some key", "Default 2"), "SomeValue");
}
