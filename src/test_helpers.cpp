#include <QtTest>

#include "helpers.h"

class test_helpers : public QObject {
    Q_OBJECT

    private slots:
        void test_encodeTitle_colon();
        void test_encodeTitle_allPrintable();
        void test_decodeTitle_allPrintable();
        void test_encodeTitle_tab();
        void test_decodeTitle_tab();
        void test_decodeTitle_accidental1();
        void test_decodeTitle_accidental2();
        void test_decodeTitle_accidental3();
        void test_decodeTitle_accidental4();
        void test_decodeTitle_accidental5();

};

void test_helpers::test_encodeTitle_colon() {
    auto actual = Helpers::getFSNameFromTitle("1:2");
    QCOMPARE(actual, QString("1~3a~2"));
}

void test_helpers::test_encodeTitle_allPrintable() {
    auto actual = Helpers::getFSNameFromTitle("A\"<>|:*?\\/Z");
    QCOMPARE(actual, QString("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z"));
}

void test_helpers::test_decodeTitle_allPrintable() {
    auto actual = Helpers::getTitleFromFSName("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z");
    QCOMPARE(actual, QString("A\"<>|:*?\\/Z"));
}

void test_helpers::test_encodeTitle_tab() {
    auto actual = Helpers::getFSNameFromTitle("A\tZ");
    QCOMPARE(actual, QString("A~09~Z"));
}

void test_helpers::test_decodeTitle_tab() {
    auto actual = Helpers::getTitleFromFSName("A~09~Z");
    QCOMPARE(actual, QString("A\tZ"));
}

void test_helpers::test_decodeTitle_accidental1() {
    auto actual = Helpers::getTitleFromFSName("~");
    QCOMPARE(actual, QString("~"));
}

void test_helpers::test_decodeTitle_accidental2() {
    auto actual = Helpers::getTitleFromFSName("~1");
    QCOMPARE(actual, QString("~1"));
}

void test_helpers::test_decodeTitle_accidental3() {
    auto actual = Helpers::getTitleFromFSName("~1~7c~~");
    QCOMPARE(actual, QString("~1|~"));
}

void test_helpers::test_decodeTitle_accidental4() {
    auto actual = Helpers::getTitleFromFSName("A~7c1~~");
    QCOMPARE(actual, QString("A~7c1~~"));
}

void test_helpers::test_decodeTitle_accidental5() {
    auto actual = Helpers::getTitleFromFSName("~77~");
    QCOMPARE(actual, QString("~77~"));
}

QTEST_APPLESS_MAIN(test_helpers)

#include "test_helpers.moc"
