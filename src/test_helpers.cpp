#include "test_helpers.h"
#include "helpers.h"

void Test_Helpers::encodeTitle_colon() {
    auto actual = Helpers::getFSNameFromTitle("1:2");
    QCOMPARE(actual, QString("1~3a~2"));
}

void Test_Helpers::encodeTitle_allPrintable() {
    auto actual = Helpers::getFSNameFromTitle("A\"<>|:*?\\/Z");
    QCOMPARE(actual, QString("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z"));
}

void Test_Helpers::decodeTitle_allPrintable() {
    auto actual = Helpers::getTitleFromFSName("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z");
    QCOMPARE(actual, QString("A\"<>|:*?\\/Z"));
}

void Test_Helpers::encodeTitle_tab() {
    auto actual = Helpers::getFSNameFromTitle("A\tZ");
    QCOMPARE(actual, QString("A~09~Z"));
}

void Test_Helpers::decodeTitle_tab() {
    auto actual = Helpers::getTitleFromFSName("A~09~Z");
    QCOMPARE(actual, QString("A\tZ"));
}

void Test_Helpers::decodeTitle_accidental1() {
    auto actual = Helpers::getTitleFromFSName("~");
    QCOMPARE(actual, QString("~"));
}

void Test_Helpers::decodeTitle_accidental2() {
    auto actual = Helpers::getTitleFromFSName("~1");
    QCOMPARE(actual, QString("~1"));
}

void Test_Helpers::decodeTitle_accidental3() {
    auto actual = Helpers::getTitleFromFSName("~1~7c~~");
    QCOMPARE(actual, QString("~1|~"));
}

void Test_Helpers::decodeTitle_accidental4() {
    auto actual = Helpers::getTitleFromFSName("A~7c1~~");
    QCOMPARE(actual, QString("A~7c1~~"));
}

void Test_Helpers::decodeTitle_accidental5() {
    auto actual = Helpers::getTitleFromFSName("~77~");
    QCOMPARE(actual, QString("~77~"));
}
