#include "icons.h"

QIcon Icons::application() {
    QIcon icon;
    icon.addFile(":icons/16x16/qtext.png", QSize(16, 16));
    icon.addFile(":icons/32x32/qtext.png", QSize(32, 32));
    icon.addFile(":icons/48x48/qtext.png", QSize(48, 48));
    icon.addFile(":icons/64x64/qtext.png", QSize(64, 64));
    return icon;
}

QIcon Icons::tray() {
    QIcon icon;
    if ((QSysInfo::kernelType() == "winnt") && (QSysInfo::productVersion() == "10")) {
        icon.addFile(":icons/16x16/tray-white.png", QSize(16, 16));
        icon.addFile(":icons/32x32/tray-white.png", QSize(32, 32));
        icon.addFile(":icons/48x48/tray-white.png", QSize(48, 48));
        icon.addFile(":icons/64x64/tray-white.png", QSize(64, 64));
    } else {
        icon.addFile(":icons/16x16/tray-color.png", QSize(16, 16));
        icon.addFile(":icons/32x32/tray-color.png", QSize(32, 32));
        icon.addFile(":icons/48x48/tray-color.png", QSize(48, 48));
        icon.addFile(":icons/64x64/tray-color.png", QSize(64, 64));
    }
    return icon;
}


QIcon Icons::newFile() {
    QIcon icon;
    icon.addFile(":icons/16x16/new.png", QSize(16, 16));
    icon.addFile(":icons/24x24/new.png", QSize(24, 24));
    icon.addFile(":icons/32x32/new.png", QSize(32, 32));
    icon.addFile(":icons/48x48/new.png", QSize(48, 48));
    icon.addFile(":icons/64x64/new.png", QSize(64, 64));
    return icon;
}

QIcon Icons::saveFile() {
    QIcon icon;
    icon.addFile(":icons/16x16/save.png", QSize(16, 16));
    icon.addFile(":icons/24x24/save.png", QSize(24, 24));
    icon.addFile(":icons/32x32/save.png", QSize(32, 32));
    icon.addFile(":icons/48x48/save.png", QSize(48, 48));
    icon.addFile(":icons/64x64/save.png", QSize(64, 64));
    return icon;
}

QIcon Icons::renameFile() {
    QIcon icon;
    icon.addFile(":icons/16x16/rename.png", QSize(16, 16));
    icon.addFile(":icons/24x24/rename.png", QSize(24, 24));
    icon.addFile(":icons/32x32/rename.png", QSize(32, 32));
    icon.addFile(":icons/48x48/rename.png", QSize(48, 48));
    icon.addFile(":icons/64x64/rename.png", QSize(64, 64));
    return icon;
}

QIcon Icons::deleteFile() {
    QIcon icon;
    icon.addFile(":icons/16x16/delete.png", QSize(16, 16));
    icon.addFile(":icons/24x24/delete.png", QSize(24, 24));
    icon.addFile(":icons/32x32/delete.png", QSize(32, 32));
    icon.addFile(":icons/48x48/delete.png", QSize(48, 48));
    icon.addFile(":icons/64x64/delete.png", QSize(64, 64));
    return icon;
}

QIcon Icons::printFile() {
    QIcon icon;
    icon.addFile(":icons/16x16/print.png", QSize(16, 16));
    icon.addFile(":icons/24x24/print.png", QSize(24, 24));
    icon.addFile(":icons/32x32/print.png", QSize(32, 32));
    icon.addFile(":icons/48x48/print.png", QSize(48, 48));
    icon.addFile(":icons/64x64/print.png", QSize(64, 64));
    return icon;
}


QIcon Icons::cut() {
    QIcon icon;
    icon.addFile(":icons/16x16/cut.png", QSize(16, 16));
    icon.addFile(":icons/24x24/cut.png", QSize(24, 24));
    icon.addFile(":icons/32x32/cut.png", QSize(32, 32));
    icon.addFile(":icons/48x48/cut.png", QSize(48, 48));
    icon.addFile(":icons/64x64/cut.png", QSize(64, 64));
    return icon;
}

QIcon Icons::copy() {
    QIcon icon;
    icon.addFile(":icons/16x16/copy.png", QSize(16, 16));
    icon.addFile(":icons/24x24/copy.png", QSize(24, 24));
    icon.addFile(":icons/32x32/copy.png", QSize(32, 32));
    icon.addFile(":icons/48x48/copy.png", QSize(48, 48));
    icon.addFile(":icons/64x64/copy.png", QSize(64, 64));
    return icon;
}

QIcon Icons::paste() {
    QIcon icon;
    icon.addFile(":icons/16x16/paste.png", QSize(16, 16));
    icon.addFile(":icons/24x24/paste.png", QSize(24, 24));
    icon.addFile(":icons/32x32/paste.png", QSize(32, 32));
    icon.addFile(":icons/48x48/paste.png", QSize(48, 48));
    icon.addFile(":icons/64x64/paste.png", QSize(64, 64));
    return icon;
}


QIcon Icons::undo() {
    QIcon icon;
    icon.addFile(":icons/16x16/undo.png", QSize(16, 16));
    icon.addFile(":icons/24x24/undo.png", QSize(24, 24));
    icon.addFile(":icons/32x32/undo.png", QSize(32, 32));
    icon.addFile(":icons/48x48/undo.png", QSize(48, 48));
    icon.addFile(":icons/64x64/undo.png", QSize(64, 64));
    return icon;
}

QIcon Icons::redo() {
    QIcon icon;
    icon.addFile(":icons/16x16/redo.png", QSize(16, 16));
    icon.addFile(":icons/24x24/redo.png", QSize(24, 24));
    icon.addFile(":icons/32x32/redo.png", QSize(32, 32));
    icon.addFile(":icons/48x48/redo.png", QSize(48, 48));
    icon.addFile(":icons/64x64/redo.png", QSize(64, 64));
    return icon;
}


QIcon Icons::gotoDialog() {
    QIcon icon;
    icon.addFile(":icons/16x16/goto.png", QSize(16, 16));
    icon.addFile(":icons/24x24/goto.png", QSize(24, 24));
    icon.addFile(":icons/32x32/goto.png", QSize(32, 32));
    icon.addFile(":icons/48x48/goto.png", QSize(48, 48));
    icon.addFile(":icons/64x64/goto.png", QSize(64, 64));
    return icon;
}


QIcon Icons::appMenu() {
    QIcon icon;
    icon.addFile(":icons/16x16/app.png", QSize(16, 16));
    icon.addFile(":icons/24x24/app.png", QSize(24, 24));
    icon.addFile(":icons/32x32/app.png", QSize(32, 32));
    icon.addFile(":icons/48x48/app.png", QSize(48, 48));
    icon.addFile(":icons/64x64/app.png", QSize(64, 64));
    return icon;
}
