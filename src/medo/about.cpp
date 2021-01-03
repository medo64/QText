#include <QCoreApplication>
#include <QMessageBox>
#include "about.h"

void About::showDialog(QWidget* parent) {
    QString description = QCoreApplication::applicationName();
    description.append(" ");
#ifdef APP_VERSION
    description.append(APP_VERSION);
#else
    description.append(QCoreApplication::applicationVersion());
#endif

#ifdef APP_COMMIT
    QString commit = APP_COMMIT;
    if (commit.length() > 0) {
        description.append("+");
        description.append(APP_COMMIT);
    }
#endif

#ifdef QT_DEBUG
    description.append("\nDEBUG");
#endif

    description.append("\n\nQt ");

    QString runtimeVersion = qVersion();
    QString compileVersion = APP_QT_VERSION;
    description.append(runtimeVersion);
    if (runtimeVersion != compileVersion) {
        description.append(" / ");
        description.append(compileVersion);
    }
    description.append("\n" + QSysInfo::prettyProductName());
    description.append("\n" + QSysInfo::kernelType() + " " + QSysInfo::kernelVersion());

#ifdef APP_COPYRIGHT
    description.append("\n\n");
    description.append(APP_COPYRIGHT);
#endif

    QMessageBox::about(parent, "About",  description);
}
