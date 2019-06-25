#include "test_helpers.h"
#include "helpers.h"

void Test_Helpers::encodeTitle_colon() {
    auto actualFile = Helpers::getFileNameFromTitle("1:2");
    QCOMPARE(actualFile, QString("1~3a~2"));

    auto actualFolder = Helpers::getFolderNameFromTitle("1:2");
    QCOMPARE(actualFolder, QString("1~3a~2"));
}

void Test_Helpers::encodeTitle_allPrintable() {
    auto actualFile = Helpers::getFileNameFromTitle("A\"<>|:*?\\/Z");
    QCOMPARE(actualFile, QString("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z"));

    auto actualFolder = Helpers::getFolderNameFromTitle("A\"<>|:*?\\/Z");
    QCOMPARE(actualFolder, QString("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z"));
}

void Test_Helpers::decodeTitle_allPrintable() {
    auto actualFile = Helpers::getFileTitleFromName("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z");
    QCOMPARE(actualFile, QString("A\"<>|:*?\\/Z"));

    auto actualFolder = Helpers::getFolderTitleFromName("A~22~~3c~~3e~~7c~~3a~~2a~~3f~~5c~~2f~Z");
    QCOMPARE(actualFolder, QString("A\"<>|:*?\\/Z"));
}

void Test_Helpers::encodeTitle_tab() {
    auto actualFile = Helpers::getFileNameFromTitle("A\tZ");
    QCOMPARE(actualFile, QString("A~09~Z"));

    auto actualFolder = Helpers::getFolderNameFromTitle("A\tZ");
    QCOMPARE(actualFolder, QString("A~09~Z"));
}

void Test_Helpers::decodeTitle_tab() {
    auto actualFile = Helpers::getFileTitleFromName("A~09~Z");
    QCOMPARE(actualFile, QString("A\tZ"));

    auto actualFolder = Helpers::getFolderTitleFromName("A~09~Z");
    QCOMPARE(actualFolder, QString("A\tZ"));
}


void Test_Helpers::encodeTitle_dotInMiddle() {
    auto actualFile = Helpers::getFileNameFromTitle("A.Z");
    QCOMPARE(actualFile, QString("A.Z"));

    auto actualFolder = Helpers::getFolderNameFromTitle("A.Z");
    QCOMPARE(actualFolder, QString("A.Z"));
}

void Test_Helpers::decodeTitle_dotInMiddle() {
    auto actualFile = Helpers::getFileTitleFromName("A.Z");
    QCOMPARE(actualFile, QString("A.Z"));

    auto actualFolder = Helpers::getFolderTitleFromName("A.Z");
    QCOMPARE(actualFolder, QString("A.Z"));
}

void Test_Helpers::encodeTitle_dotAtEnd() {
    auto actualFile = Helpers::getFileNameFromTitle("AZ.");
    QCOMPARE(actualFile, QString("AZ."));

    auto actualFolder = Helpers::getFolderNameFromTitle("AZ.");
    QCOMPARE(actualFolder, QString("AZ~2e~"));
}

void Test_Helpers::decodeTitle_dotAtEnd() {
    auto actualFile = Helpers::getFileTitleFromName("AZ.");
    QCOMPARE(actualFile, QString("AZ."));

    auto actualFolder = Helpers::getFolderTitleFromName("AZ~2e~");
    QCOMPARE(actualFolder, QString("AZ."));
}

void Test_Helpers::decodeTitle_accidentalDotAtEnd() {
    auto actualFile = Helpers::getFileTitleFromName("AZ~2e~");
    QCOMPARE(actualFile, QString("AZ."));

    auto actualFolder = Helpers::getFolderTitleFromName("AZ.");
    QCOMPARE(actualFolder, QString("AZ."));
}

void Test_Helpers::encodeTitle_doubleDotAtEnd() {
    auto actualFile = Helpers::getFileNameFromTitle("AZ..");
    QCOMPARE(actualFile, QString("AZ.."));

    auto actualFolder = Helpers::getFolderNameFromTitle("AZ..");
    QCOMPARE(actualFolder, QString("AZ.~2e~"));
}

void Test_Helpers::decodeTitle_doubleDotAtEnd() {
    auto actualFile = Helpers::getFileTitleFromName("AZ..");
    QCOMPARE(actualFile, QString("AZ.."));

    auto actualFolder = Helpers::getFolderTitleFromName("AZ.~2e~");
    QCOMPARE(actualFolder, QString("AZ.."));
}

void Test_Helpers::decodeTitle_accidentalDoubleDotAtEnd() {
    auto actualFile = Helpers::getFileTitleFromName("AZ.~2e~");
    QCOMPARE(actualFile, QString("AZ.."));

    auto actualFolder = Helpers::getFolderTitleFromName("AZ..");
    QCOMPARE(actualFolder, QString("AZ.."));
}


void Test_Helpers::encodeTitle_spaceAtStart() {
    auto actualFile = Helpers::getFileNameFromTitle(" AZ");
    QCOMPARE(actualFile, QString("~20~AZ"));

    auto actualFolder = Helpers::getFolderNameFromTitle(" AZ");
    QCOMPARE(actualFolder, QString("~20~AZ"));
}

void Test_Helpers::decodeTitle_spaceAtStart() {
    auto actualFile = Helpers::getFileTitleFromName("~20~AZ");
    QCOMPARE(actualFile, QString(" AZ"));

    auto actualFolder = Helpers::getFolderTitleFromName("~20~AZ");
    QCOMPARE(actualFolder, QString(" AZ"));
}

void Test_Helpers::decodeTitle_accidentalSpaceAtStart() {
    auto actualFile = Helpers::getFileTitleFromName(" AZ");
    QCOMPARE(actualFile, QString(" AZ"));

    auto actualFolder = Helpers::getFolderTitleFromName(" AZ");
    QCOMPARE(actualFolder, QString(" AZ"));
}

void Test_Helpers::encodeTitle_doubleSpaceAtStart() {
    auto actualFile = Helpers::getFileNameFromTitle("  AZ");
    QCOMPARE(actualFile, QString("~20~ AZ"));

    auto actualFolder = Helpers::getFolderNameFromTitle("  AZ");
    QCOMPARE(actualFolder, QString("~20~ AZ"));
}

void Test_Helpers::decodeTitle_doubleSpaceAtStart() {
    auto actualFile = Helpers::getFileTitleFromName("~20~ AZ");
    QCOMPARE(actualFile, QString("  AZ"));

    auto actualFolder = Helpers::getFolderTitleFromName("~20~ AZ");
    QCOMPARE(actualFolder, QString("  AZ"));
}

void Test_Helpers::decodeTitle_accidentalDoubleSpaceAtStart() {
    auto actualFile = Helpers::getFileTitleFromName("  AZ");
    QCOMPARE(actualFile, QString("  AZ"));

    auto actualFolder = Helpers::getFolderTitleFromName("  AZ");
    QCOMPARE(actualFolder, QString("  AZ"));
}

void Test_Helpers::encodeTitle_spaceAtEnd() {
    auto actualFile = Helpers::getFileNameFromTitle("AZ ");
    QCOMPARE(actualFile, QString("AZ "));

    auto actualFolder = Helpers::getFolderNameFromTitle("AZ ");
    QCOMPARE(actualFolder, QString("AZ~20~"));
}

void Test_Helpers::decodeTitle_spaceAtEnd() {
    auto actualFile = Helpers::getFileTitleFromName("AZ ");
    QCOMPARE(actualFile, QString("AZ "));

    auto actualFolder = Helpers::getFolderTitleFromName("AZ~20~");
    QCOMPARE(actualFolder, QString("AZ "));
}

void Test_Helpers::decodeTitle_accidentalSpaceAtEnd() {
    auto actualFile = Helpers::getFileTitleFromName("AZ~20~");
    QCOMPARE(actualFile, QString("AZ "));

    auto actualFolder = Helpers::getFolderTitleFromName("AZ ");
    QCOMPARE(actualFolder, QString("AZ "));
}

void Test_Helpers::encodeTitle_doubleSpaceAtEnd() {
    auto actualFile = Helpers::getFileNameFromTitle("AZ  ");
    QCOMPARE(actualFile, QString("AZ  "));

    auto actualFolder = Helpers::getFolderNameFromTitle("AZ  ");
    QCOMPARE(actualFolder, QString("AZ ~20~"));
}

void Test_Helpers::decodeTitle_doubleSpaceAtEnd() {
    auto actualFile = Helpers::getFileTitleFromName("AZ  ");
    QCOMPARE(actualFile, QString("AZ  "));

    auto actualFolder = Helpers::getFolderTitleFromName("AZ ~20~");
    QCOMPARE(actualFolder, QString("AZ  "));
}

void Test_Helpers::decodeTitle_accidentalDoubleSpaceAtEnd() {
    auto actualFile = Helpers::getFileTitleFromName("AZ ~20~");
    QCOMPARE(actualFile, QString("AZ  "));

    auto actualFolder = Helpers::getFolderTitleFromName("AZ  ");
    QCOMPARE(actualFolder, QString("AZ  "));
}


void Test_Helpers::decodeTitle_accidental1() {
    auto actualFile = Helpers::getFileTitleFromName("~");
    QCOMPARE(actualFile, QString("~"));

    auto actualFolder = Helpers::getFileTitleFromName("~");
    QCOMPARE(actualFolder, QString("~"));
}

void Test_Helpers::decodeTitle_accidental2() {
    auto actualFile = Helpers::getFileTitleFromName("~1");
    QCOMPARE(actualFile, QString("~1"));

    auto actualFolder = Helpers::getFileTitleFromName("~1");
    QCOMPARE(actualFolder, QString("~1"));
}

void Test_Helpers::decodeTitle_accidental3() {
    auto actualFile = Helpers::getFileTitleFromName("~1~7c~~");
    QCOMPARE(actualFile, QString("~1|~"));

    auto actualFolder = Helpers::getFileTitleFromName("~1~7c~~");
    QCOMPARE(actualFolder, QString("~1|~"));
}

void Test_Helpers::decodeTitle_accidental4() {
    auto actualFile = Helpers::getFileTitleFromName("A~7c1~~");
    QCOMPARE(actualFile, QString("A~7c1~~"));

    auto actualFolder = Helpers::getFileTitleFromName("A~7c1~~");
    QCOMPARE(actualFolder, QString("A~7c1~~"));
}

void Test_Helpers::decodeTitle_accidental5() {
    auto actualFile = Helpers::getFileTitleFromName("~77~");
    QCOMPARE(actualFile, QString("~77~"));

    auto actualFolder = Helpers::getFileTitleFromName("~77~");
    QCOMPARE(actualFolder, QString("~77~"));
}
