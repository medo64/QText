#ifndef TEST_CONFIG_H
#define TEST_CONFIG_H

#include <QObject>
#include <QtTest>

class Test_Config : public QObject {
    Q_OBJECT

    private:
        void setup(QString applicationName, QString organizationName);
        void setup(QString applicationName, QString organizationName, QString testConfigFile);
        void cleanup(QString configFile, QString stateFile, QString dataDirectory);
        void cleanup();
        void verifyTestFile(QString testFileName, QString actualFileName);

    private slots:
        void paths();
        void pathsWithSpaces();
        void reset();
        void nullKey();
        void nullKeyState();
        void emptyKey();
        void emptyKeyState();
        void emptySave();
        void emptyLinesCrLf();
        void emptyLinesLf();
        void emptyLinesCr();
        void emptyLinesMixed();
        void commentsOnly();
        void commentsWithValues();
        void spacingEscape();
        void writeBasic();
        void writeBasicState();
        void writeNoEmptyLine();
        void writeSameSeparatorEquals();
        void writeSameSeparatorSpace();
        void replace();
        void replaceState();
        void spacingPreserved();
        void spacingPreservedOnAdd();
        void writeToEmpty();
        void replaceOnlyLast();
        void removeSingle();
        void removeMulti();
        void readMulti();
        void multiWrite();
        void multiReplace();
        void testConversionWrite();
        void testConversionRead();
        void keyWhitespace();
        void deleteAll();
        void twoReadsWithDifferentDefault();
        void writeOverridesReads();
        void removeCausesDefault();
        void removeAllCausesDefault();
        void writeChangesOnlyLastEntry();
        void caseInsensitiveDefaults();
        void caseInsensitiveWrites();

};

#endif // TEST_CONFIG_H
