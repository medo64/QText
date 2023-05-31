#include "setup.h"
#include <QCoreApplication>
#include <QDir>
#include <QFile>
#include <QTextStream>

#if defined(Q_OS_WIN)
    #include <windows.h>
#endif

bool Setup::autostart() {
    return nativeAutostartCheck();
}

void Setup::setAutostart(bool newAutostart) {
    if (newAutostart) {
        nativeAutostartAdd();
    } else {
        nativeAutostartRemove();
    }
}


#if defined(Q_OS_WIN)

void Setup::nativeAutostartAdd() {
    QString valueName = QCoreApplication::applicationName();
    QString value = QString("\"%1\" --hide").arg(QDir::toNativeSeparators(QCoreApplication::applicationFilePath()));
    HKEY runKey = nullptr;
    if (RegOpenKeyEx(HKEY_CURRENT_USER, TEXT("Software\\Microsoft\\Windows\\CurrentVersion\\Run"), 0, KEY_WRITE, &runKey) == ERROR_SUCCESS) {
        QByteArray buffer = QByteArray(reinterpret_cast<const char*>(value.utf16()), (value.length() + 1) * 2);
        RegSetValueEx(runKey, reinterpret_cast<LPCWSTR>(valueName.utf16()), 0, REG_SZ,
                      reinterpret_cast<const unsigned char*>(buffer.constData()), static_cast<DWORD>(buffer.size()));
    }
    RegCloseKey(runKey);
}

void Setup::nativeAutostartRemove() {
    QString valueName = QCoreApplication::applicationName();
    HKEY runKey = nullptr;
    if (RegOpenKeyEx(HKEY_CURRENT_USER, TEXT("Software\\Microsoft\\Windows\\CurrentVersion\\Run"), 0, KEY_WRITE, &runKey) == ERROR_SUCCESS) {
        RegDeleteValue(runKey, reinterpret_cast<LPCWSTR>(valueName.utf16()));
    }
    RegCloseKey(runKey);
}

bool Setup::nativeAutostartCheck() {
    QString valueName = QCoreApplication::applicationName();
    QString expectedValue = QString("\"%1\" --hide").arg(QDir::toNativeSeparators(QCoreApplication::applicationFilePath()));
    bool isFound = false;
    HKEY runKey = nullptr;
    if (RegOpenKeyEx(HKEY_CURRENT_USER, TEXT("Software\\Microsoft\\Windows\\CurrentVersion\\Run"), 0, KEY_READ, &runKey) == ERROR_SUCCESS) {
        ushort valueBuffer[4096];
        DWORD valueBufferSize = sizeof(valueBuffer);
        if (RegGetValue(runKey, nullptr, reinterpret_cast<LPCWSTR>(valueName.utf16()), RRF_RT_REG_SZ, nullptr, valueBuffer, &valueBufferSize) ==  ERROR_SUCCESS) {
            auto value = QString::fromUtf16(valueBuffer, static_cast<int>(valueBufferSize) / 2 - 1);
            isFound = (expectedValue.compare(value) == 0);
        }
    }
    RegCloseKey(runKey);
    return isFound;
}

#elif defined(Q_OS_LINUX)

void Setup::nativeAutostartAdd() {
    QDir autostartDirectory(QDir::cleanPath(QDir::homePath() + "/.config/autostart"));
    if (!autostartDirectory.exists()) { autostartDirectory.mkpath("."); }
    QString autostartFile = QDir::cleanPath(autostartDirectory.path() + "/qtext.desktop");
    QString execLine = QCoreApplication::applicationFilePath() + " --hide";

    QFile file(autostartFile);
    if (file.open(QIODevice::WriteOnly)) {
        QTextStream stream(&file);
        stream << "[Desktop Entry]" << Qt::endl;
        stream << "Name=QText" << Qt::endl;
        stream << "Exec=" << execLine << Qt::endl;
        stream << "Terminal=false" << Qt::endl;
        stream << "Type=Application" << Qt::endl;
        stream << "StartupNotify=false" << Qt::endl;
        stream << "X-GNOME-Autostart-enabled=true" << Qt::endl;
    }
}

void Setup::nativeAutostartRemove() {
    QDir autostartDirectory(QDir::cleanPath(QDir::homePath() + "/.config/autostart"));
    if (!autostartDirectory.exists()) { autostartDirectory.mkpath("."); }
    QString autostartFile = QDir::cleanPath(autostartDirectory.path() + "/qtext.desktop");
    QFile::remove(autostartFile);
}

bool Setup::nativeAutostartCheck() {
    QDir autostartDirectory(QDir::cleanPath(QDir::homePath() + "/.config/autostart"));
    if (!autostartDirectory.exists()) { autostartDirectory.mkpath("."); }
    QString autostartFile = QDir::cleanPath(autostartDirectory.path() + "/qtext.desktop");
    QString execLine = QCoreApplication::applicationFilePath() + " --hide";

    QFile file(autostartFile);
    if (file.open(QIODevice::ReadOnly)) {
        QTextStream stream(&file);
        while (!stream.atEnd()) {
            QString line = stream.readLine();
            if (line.startsWith("Exec=")) {
                QStringList parts = line.split("=");
                QString execLineFound = parts[1].trimmed();
                if (execLine.compare(execLineFound, Qt::CaseSensitive) == 0) { return true; }
                break;
            }
        }
    }
    return false;
}

#endif
