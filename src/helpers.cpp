#include <QDesktopServices>
#include <QDir>
#include <QProcess>
#include <QUrl>
#include "helpers.h"
#include "icons.h"

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

    QChar lastChar = QChar( (name.length() > 0) ? name.at(name.length() - 1) : '\0' );
    if (isFolder && ((lastChar == '.') || (lastChar == ' '))) {
        name.remove(name.length() - 1, 1);
        name.append("~");
        name.append(QString("%1").arg(lastChar.unicode(), 2, 16, QChar('0')));
        name.append("~");
    }

    QChar firstChar = (name.length() > 0) ? name.at(0) : '\0';
    if (firstChar == ' ') {
        name.remove(0, 1);
        name.insert(0, "~" + QString("%1").arg(firstChar.unicode(), 2, 16, QChar('0')) + "~");
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
                        if (!isValidTitleChar(charValue) || (charValue == '.') || (charValue == ' ')) { //decoded character was among invalid characters
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


/*!
 * \brief Shows directory or file in file manager. Returns true if successful.
 * \param directoryPath Directory path.
 * \param filePath File path. Can be nullptr.
 */
bool Helpers::showInFileManager(QString directoryPath, QString filePath) {
#if defined(Q_OS_WIN)
    const QString explorerExe = "explorer.exe";
    QStringList params;
    params += "/select,";
    params += QDir::toNativeSeparators((filePath != nullptr) ? filePath : directoryPath);
    if (QProcess::startDetached(explorerExe, params)) { return true; }
#elif defined(Q_OS_LINUX)
    QString nautilusPath = "/usr/bin/nautilus";
    QFile nautilus(nautilusPath);
    if (nautilus.exists()) {
        QStringList params;
        if (filePath != nullptr) {
            params += "-s";
            params += filePath;
        } else {
            params += directoryPath;
        }
        if (QProcess::startDetached(nautilusPath, params)) { return true; }
    }
#endif

    return QDesktopServices::openUrl(QUrl::fromLocalFile(directoryPath)); //fall-back to showing just directory
}


/*!
 * \brief Opens file with default application
 * \param filePath File path.
 */
bool Helpers::openWithDefaultApplication(QString filePath) {
    return QDesktopServices::openUrl(QUrl::fromLocalFile(filePath));
}

/*!
 * \brief Opens file with vscode
 * \param filePath File path.
 */
bool Helpers::openWithVSCode(QString filePath) {
#if defined(Q_OS_WIN)
    QStringList homePaths = QStandardPaths::standardLocations(QStandardPaths::HomeLocation);
    const QString executablePath = QDir::cleanPath(homePaths[0] + "/AppData/Local/Programs/Microsoft VS Code/Code.exe");
#elif defined(Q_OS_LINUX)
    QString executablePath = "/usr/bin/code";
#endif

    QFile executableFile(executablePath);
    if (executableFile.exists()) {
        QStringList params;
        params += QDir::toNativeSeparators(filePath);
        if (QProcess::startDetached(executablePath, params)) { return true; }
    }

    return false;
}

/*!
 * \brief Returns true if VS Code is found
 */
bool Helpers::openWithVSCodeAvailable() {
#if defined(Q_OS_WIN)
    QStringList homePaths = QStandardPaths::standardLocations(QStandardPaths::HomeLocation);
    const QString executablePath = QDir::cleanPath(homePaths[0] + "/AppData/Local/Programs/Microsoft VS Code/Code.exe");
#elif defined(Q_OS_LINUX)
    QString executablePath = "/usr/bin/code";
#endif

    QFile executableFile(executablePath);
    return executableFile.exists();
}


/*!
 * \brief Sets palette for read-only control
 */
void Helpers::setReadonlyPalette(QWidget* widget) {
    QPalette readOnlyPalette = widget->palette();
    readOnlyPalette.setColor(QPalette::Base, widget->palette().color(QPalette::Window));
    widget->setPalette(readOnlyPalette);
}


/*!
 * \brief Replaces dialog icon
 */
void Helpers::replaceDialogIcon(QWidget* dialog) {
    dialog->setWindowIcon(Icons::appMono());
}
