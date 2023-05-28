#include <QAbstractButton>
#include <QApplication>
#include <QDesktopServices>
#include <QDialogButtonBox>
#include <QDir>
#include <QProcess>
#include <QStyle>
#include <QStyleFactory>
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
    for (QChar ch : fsTitle) {
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
    for (QChar ch : fsName) {
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
 * \brief Returns text with accents removed.
 * \param text Text.
 */
QString Helpers::getTextWithoutAccents(QString text) {
    QString normalizedText = text.normalized(QString::NormalizationForm_D);

    QString filtered;
    for (int i = 0; i < normalizedText.length(); i++) {
        if (normalizedText.at(i).category() != QChar::Mark_NonSpacing) {
            filtered.append(normalizedText.at(i));
        }
    }

    return filtered.normalized(QString::NormalizationForm_C);
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
 * \brief Opens file with VSCode
 * \param file File.
 */
bool Helpers::openFileWithVSCode(FileItem* file) {
    return openFileWithVSCode(file->path());
}

/*!
 * \brief Opens file with VSCode
 * \param filePath File path.
 */
bool Helpers::openFileWithVSCode(QString filePath) {
#if defined(Q_OS_WIN)
    QStringList homePaths = QStandardPaths::standardLocations(QStandardPaths::HomeLocation);
    const QString executablePath = QDir::cleanPath(homePaths[0] + "/AppData/Local/Programs/Microsoft VS Code/Code.exe");
#elif defined(Q_OS_LINUX)
    QString executablePath = "/usr/bin/code";
#endif

    QFile executableFile(executablePath);
    if (!executableFile.exists()) { return false; }

    QStringList params;
    params += QDir::toNativeSeparators(filePath);
    return QProcess::startDetached(executablePath, params);
}

/*!
 * \brief Opens directory with VSCode
 * \param file File item.
 */
bool Helpers::openDirectoryWithVSCode(FileItem* file) {
#if defined(Q_OS_WIN)
    QStringList homePaths = QStandardPaths::standardLocations(QStandardPaths::HomeLocation);
    const QString executablePath = QDir::cleanPath(homePaths[0] + "/AppData/Local/Programs/Microsoft VS Code/Code.exe");
#elif defined(Q_OS_LINUX)
    QString executablePath = "/usr/bin/code";
#endif

    QFile executableFile(executablePath);
    if (!executableFile.exists()) { return false; }

    auto folder = file->folder();
    auto rootFolder = folder->isRoot() ? folder : folder->rootFolder();

    QStringList params;
    params += QDir::toNativeSeparators(rootFolder->path());
    params += QDir::toNativeSeparators(file->path());
    return  QProcess::startDetached(executablePath, params);
}

/*!
 * \brief Opens directory with VSCode
 * \param folder Folder item.
 */
bool Helpers::openDirectoryWithVSCode(FolderItem* folder) {
#if defined(Q_OS_WIN)
    QStringList homePaths = QStandardPaths::standardLocations(QStandardPaths::HomeLocation);
    const QString executablePath = QDir::cleanPath(homePaths[0] + "/AppData/Local/Programs/Microsoft VS Code/Code.exe");
#elif defined(Q_OS_LINUX)
    QString executablePath = "/usr/bin/code";
#endif

    QFile executableFile(executablePath);
    if (!executableFile.exists()) { return false; }

    auto rootFolder = folder->isRoot() ? folder : folder->rootFolder();

    QStringList params;
    params += QDir::toNativeSeparators(rootFolder->path());
    return  QProcess::startDetached(executablePath, params);
}

/*!
 * \brief Opens file with VSCode
 * \param path Directory paths.
 */
bool Helpers::openDirectoriesWithVSCode(QStringList directoryPaths) {
#if defined(Q_OS_WIN)
    QStringList homePaths = QStandardPaths::standardLocations(QStandardPaths::HomeLocation);
    const QString executablePath = QDir::cleanPath(homePaths[0] + "/AppData/Local/Programs/Microsoft VS Code/Code.exe");
#elif defined(Q_OS_LINUX)
    QString executablePath = "/usr/bin/code";
#endif

    QFile executableFile(executablePath);
    if (!executableFile.exists()) { return false; }

    QStringList params;
    for (QString directoryPath : directoryPaths) {
        params += QDir::toNativeSeparators(directoryPath);
    }
    return  QProcess::startDetached(executablePath, params);
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
void Helpers::setupResizableDialog(QWidget* dialog) {
    dialog->setWindowIcon(Icons::appMono());

    for (QAbstractButton* button : dialog->findChild<QDialogButtonBox*>()->buttons()) {
        button->setIcon(QIcon());
    }
}

/*!
 * \brief Replaces dialog icon and makes it fixed size
 */
void Helpers::setupFixedSizeDialog(QWidget* dialog) {
    dialog->setWindowIcon(Icons::appMono());
    dialog->setFixedSize(dialog->geometry().width(), dialog->sizeHint().height());

    for (QAbstractButton* button : dialog->findChild<QDialogButtonBox*>()->buttons()) {
        button->setIcon(QIcon());
    }
}

/*!
 * \brief Sets application palette to dark
 */
void Helpers::setupTheme(bool darkMode) {
    if (darkMode) {
        qApp->setStyle(QStyleFactory::create("Fusion"));

        QPalette newPalette;
        newPalette.setColor(QPalette::Window,          QColor( 37,  37,  37));
        newPalette.setColor(QPalette::WindowText,      QColor(212, 212, 212));
        newPalette.setColor(QPalette::Base,            QColor( 60,  60,  60));
        newPalette.setColor(QPalette::AlternateBase,   QColor( 45,  45,  45));
        newPalette.setColor(QPalette::PlaceholderText, QColor(127, 127, 127));
        newPalette.setColor(QPalette::Text,            QColor(212, 212, 212));
        newPalette.setColor(QPalette::Button,          QColor( 45,  45,  45));
        newPalette.setColor(QPalette::ButtonText,      QColor(212, 212, 212));
        newPalette.setColor(QPalette::BrightText,      QColor(240, 240, 240));
        newPalette.setColor(QPalette::Highlight,       QColor( 38,  79, 120));
        newPalette.setColor(QPalette::HighlightedText, QColor(240, 240, 240));

        newPalette.setColor(QPalette::Link,            QColor(182, 208, 250));
        newPalette.setColor(QPalette::LinkVisited,     QColor(222, 208, 250));

        newPalette.setColor(QPalette::Light,           QColor( 60,  60,  60));
        newPalette.setColor(QPalette::Midlight,        QColor( 52,  52,  52));
        newPalette.setColor(QPalette::Dark,            QColor( 30,  30,  30) );
        newPalette.setColor(QPalette::Mid,             QColor( 37,  37,  37));
        newPalette.setColor(QPalette::Shadow,          QColor( 0,    0,   0));

        newPalette.setColor(QPalette::Disabled, QPalette::Text, QColor(127, 127, 127));

        qApp->setPalette(newPalette);
    } else {
        qApp->setPalette(qApp->style()->standardPalette());
        qApp->setStyle(QStyleFactory::create("WindowsVista"));
    }
}

/*!
 * \brief Checks if OS is in dark mode.
 * On Linux returns false always.
 */
bool Helpers::isOSInDarkMode() {
#if defined(Q_OS_WIN)
    return false; //TODO
#else
    return false;
#endif
}

/*!
 * \brief Opens URL.
 */
void Helpers::openUrl(QString url) {
    QDesktopServices::openUrl(QUrl(url, QUrl::TolerantMode));
}

bool Helpers::isWayland() {
    QString platformName = QGuiApplication::platformName();
    QString sessionType = getenv("XDG_SESSION_TYPE");
    return (platformName.compare("wayland") == 0) || (sessionType.compare("wayland") == 0);
}
