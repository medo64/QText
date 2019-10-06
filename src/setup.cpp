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
    QDir autostartDirectory(QDir::cleanPath(QDir::homePath() +"/.config/autostart"));
    if (!autostartDirectory.exists()) { autostartDirectory.mkpath("."); }

    QFile file(QDir::cleanPath(autostartDirectory.path() + "/qtext.desktop"));
    if (file.open(QIODevice::WriteOnly)) {
        QTextStream stream(&file);
        stream << "[Desktop Entry]" << endl;
        stream << "Name=QText" << endl;
        stream << "Exec=" << QCoreApplication::applicationFilePath() << endl;
        stream << "Terminal=false" << endl;
        stream << "Type=Application" << endl;
        stream << "StartupNotify=false" << endl;
        stream << "X-GNOME-Autostart-enabled=true" << endl;
    }
}

void Setup::nativeAutostartRemove() {

}

bool Setup::nativeAutostartCheck() {
    return false;
}

#endif
