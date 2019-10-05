#include "setup.h"
#include <QCoreApplication>
#include <QDir>
#include <QFile>
#include <QTextStream>

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
}

void Setup::nativeAutostartRemove() {
}

bool Setup::nativeAutostartCheck() {
    return false;
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
