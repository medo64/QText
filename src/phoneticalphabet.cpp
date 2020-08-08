#include "phoneticalphabet.h"

QString PhoneticAlphabet::getNatoText(QString character) {
    QString chLatin = character.normalized(QString::NormalizationForm_KD).toUpper().toLatin1(); //yes, relies on undefined conversion to remove accents
    QChar ch = chLatin[0];

    QString out;
    if (ch == 'A') {
        out = "Alfa";
    } else if (ch == 'B') {
        out = "Bravo";
    } else if (ch == 'C') {
        out = "Charlie";
    } else if (ch == 'D') {
        out = "Delta";
    } else if (ch == 'E') {
        out = "Echo";
    } else if (ch == 'F') {
        out = "Foxtrot";
    } else if (ch == 'G') {
        out = "Golf";
    } else if (ch == 'H') {
        out = "Hotel";
    } else if (ch == 'I') {
        out = "India";
    } else if (ch == 'J') {
        out = "Juliett";
    } else if (ch == 'K') {
        out = "Kilo";
    } else if (ch == 'L') {
        out = "Lima";
    } else if (ch == 'M') {
        out = "Mike";
    } else if (ch == 'N') {
        out = "November";
    } else if (ch == 'O') {
        out = "Oscar";
    } else if (ch == 'P') {
        out = "Papa";
    } else if (ch == 'Q') {
        out = "Quebec";
    } else if (ch == 'R') {
        out = "Romeo";
    } else if (ch == 'S') {
        out = "Sierra";
    } else if (ch == 'T') {
        out = "Tango";
    } else if (ch == 'U') {
        out = "Uniform";
    } else if (ch == 'V') {
        out = "Victor";
    } else if (ch == 'W') {
        out = "Whiskey";
    } else if (ch == 'X') {
        out = "X-ray";
    } else if (ch == 'Y') {
        out = "Yankee";
    } else if (ch == 'Z') {
        out = "Zulu";
    } else if (ch == '0') {
        out = "Zero";
    } else if (ch == '1') {
        out = "One";
    } else if (ch == '2') {
        out = "Two";
    } else if (ch == '3') {
        out = "Three";
    } else if (ch == '4') {
        out = "Four";
    } else if (ch == '5') {
        out = "Five";
    } else if (ch == '6') {
        out = "Six";
    } else if (ch == '7') {
        out = "Seven";
    } else if (ch == '8') {
        out = "Eight";
    } else if (ch == '9') {
        out = "Nine";
    } else {
        return "[" + ch + "]"; //just return the darn character back
    }

    if (chLatin.length() > 1) { out += "'"; } //add prime if character had accents

    return out;
}
