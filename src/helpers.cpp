#include "helpers.h"

bool Helpers::isValidTitleChar(QChar ch) {
    if (ch < QChar(' ')) { return false; }
    if (ch == QChar('"')) { return false; }
    if (ch == QChar('*')) { return false; }
    if (ch == QChar('/')) { return false; }
    if (ch == QChar(':')) { return false; }
    if (ch == QChar('<')) { return false; }
    if (ch == QChar('>')) { return false; }
    if (ch == QChar('?')) { return false; }
    if (ch == QChar('\\')) { return false; }
    if (ch == QChar('|')) { return false; }
    return true;
}

QString Helpers::getFSNameFromTitle(QString title) {
    QString name = "";
    for (int i = 0; i < title.size(); ++i) {
        QChar ch = title.at(i);
        if (!isValidTitleChar(ch)) {
            name.append("~");
            name.append(QString("%1").arg(ch.unicode(), 2, 16, QChar('0')));
            name.append("~");
        } else {
            name.append(ch);
        }
    }
    return name;
}

QString Helpers::getTitleFromFSName(QString name) {
    QString title = "";
    QString sbDecode = "";
    bool inEncoded = false;
    for (int i = 0; i < name.size(); ++i) {
        QChar ch = name.at(i);

        if (inEncoded) {
            if (ch == '~') { //end decode
                if (sbDecode.length() == 2) { //could be
                    bool ok;
                    int value = sbDecode.toInt(&ok, 16);
                    if (ok) {
                        auto charValue = QChar(value);
                        if (!isValidTitleChar(charValue)) { //decoded character was among invalid characters
                            title.append(charValue);
                            inEncoded = false;
                        } else { //not a char to be decoded
                            title.append(QChar('~'));
                            title.append(sbDecode);
                            sbDecode.clear();
                        }
                    } else { //cannot decode, go with plain
                        title.append(QChar('~'));
                        title.append(sbDecode);
                        sbDecode.clear();
                    }
                } else { //too short to be encoded, go with plain
                    title.append(QChar('~'));
                    title.append(sbDecode);
                    sbDecode.clear();
                }
            } else { //just another encoded character
                sbDecode.append(ch);
                if (sbDecode.length() > 2) { //too long to be encoded, go with plain
                    title.append(QChar('~'));
                    title.append(sbDecode);
                    inEncoded = false;
                }
            }
        } else {
            if (ch == QChar('~')) { //start decode
                sbDecode.clear();
                inEncoded = true;
            } else {
                title += ch;
            }
        }
    }

    if (inEncoded) { //any stragglers
        title.append("~");
        title.append(sbDecode);
    }

    return title;
}
