#include "helpers.h"

/*!
 * \brief Returns escaped file name from title
 * \param fileTitle Title to convert.
 */
QString Helpers::getFileNameFromTitle(QString fileTitle) {
    return getFSNameFromTitle(fileTitle, false);
}

/*!
 * \brief Returns title based on escaped file name.
 * \param folderName Name of file with escapes.
 */
QString Helpers::getFileTitleFromName(QString fileName) {
    return getFSTitleFromName(fileName);
}

/*!
 * \brief Returns escaped folder name from title
 * \param folderTitle Title to convert.
 */
QString Helpers::getFolderNameFromTitle(QString folderTitle) {
    return getFSNameFromTitle(folderTitle, true);
}

/*!
 * \brief Returns title based on escaped folder name.
 * \param folderName Name of folder with escapes.
 */
QString Helpers::getFolderTitleFromName(QString folderName) {
    return getFSTitleFromName(folderName);
}


/*!
 * \brief Returns if character is considered a valid file system name character.
 * \param ch Character to check.
 */
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

/*!
 * \brief Returns escaped file system name from title
 * \param fsTitle Title to convert.
 * \param isFolder If true, folder rules are to be used (last dot is encoded)
 */
QString Helpers::getFSNameFromTitle(QString fsTitle, bool isFolder) {
    QString name = "";
    for (int i = 0; i < fsTitle.size(); ++i) {
        QChar ch = fsTitle.at(i);
        if (!isValidTitleChar(ch)) {
            name.append("~");
            name.append(QString("%1").arg(ch.unicode(), 2, 16, QChar('0')));
            name.append("~");
        } else {
            name.append(ch);
        }
    }
    if (isFolder && ((name.length() > 0) && name.endsWith(".", Qt::CaseSensitive))) {
        name.remove(name.length() - 1, 1);
        name.append("~");
        name.append(QString("%1").arg(QChar('.').unicode(), 2, 16, QChar('0')));
        name.append("~");
        }
    return name;
}

/*!
 * \brief Returns title based on escaped file system name.
 * \param fsName Name with escapes.
 * \param isFolder If true, folder rules are to be used (last dot is encoded)
 */
QString Helpers::getFSTitleFromName(QString fsName) {
    QString title = "";
    QString sbDecode = "";
    bool inEncoded = false;
    for (int i = 0; i < fsName.size(); ++i) {
        QChar ch = fsName.at(i);

        if (inEncoded) {
            if (ch == '~') { //end decode
                if (sbDecode.length() == 2) { //could be
                    bool ok;
                    int value = sbDecode.toInt(&ok, 16);
                    if (ok) {
                        auto charValue = QChar(value);
                        if (!isValidTitleChar(charValue) || (charValue == '.')) { //decoded character was among invalid characters
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
