#include "icons.h"

QIcon Icons::app() {
    QIcon icon;
#ifndef QT_DEBUG
    icon.addFile(":icons/16x16/app.png", QSize(16, 16));
    icon.addFile(":icons/24x24/app.png", QSize(24, 24));
    icon.addFile(":icons/32x32/app.png", QSize(32, 32));
    icon.addFile(":icons/48x48/app.png", QSize(48, 48));
    icon.addFile(":icons/64x64/app.png", QSize(64, 64));
    icon.addFile(":icons/96x96/app.png", QSize(96, 96));
    icon.addFile(":icons/128x128/app.png", QSize(128, 128));
#else
    icon.addFile(":icons/16x16/appDebug.png", QSize(16, 16));
    icon.addFile(":icons/24x24/appDebug.png", QSize(24, 24));
    icon.addFile(":icons/32x32/appDebug.png", QSize(32, 32));
    icon.addFile(":icons/48x48/appDebug.png", QSize(48, 48));
    icon.addFile(":icons/64x64/appDebug.png", QSize(64, 64));
    icon.addFile(":icons/96x96/appDebug.png", QSize(96, 96));
    icon.addFile(":icons/128x128/appDebug.png", QSize(128, 128));
#endif
    return icon;
}

QIcon Icons::appMono() {
    QIcon icon;
    icon.addFile(":icons/16x16/appMono.png", QSize(16, 16));
    icon.addFile(":icons/24x24/appMono.png", QSize(24, 24));
    icon.addFile(":icons/32x32/appMono.png", QSize(32, 32));
    icon.addFile(":icons/48x48/appMono.png", QSize(48, 48));
    icon.addFile(":icons/64x64/appMono.png", QSize(64, 64));
    icon.addFile(":icons/96x96/appMono.png", QSize(96, 96));
    icon.addFile(":icons/128x128/appMono.png", QSize(128, 128));
    return icon;
}

QIcon Icons::appWhite() {
    QIcon icon;
    icon.addFile(":icons/16x16/appWhite.png", QSize(16, 16));
    icon.addFile(":icons/24x24/appWhite.png", QSize(24, 24));
    icon.addFile(":icons/32x32/appWhite.png", QSize(32, 32));
    icon.addFile(":icons/48x48/appWhite.png", QSize(48, 48));
    icon.addFile(":icons/64x64/appWhite.png", QSize(64, 64));
    icon.addFile(":icons/96x96/appWhite.png", QSize(96, 96));
    icon.addFile(":icons/128x128/appWhite.png", QSize(128, 128));
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

QIcon Icons::printPreviewFile() {
    QIcon icon;
    icon.addFile(":icons/16x16/printPreview.png", QSize(16, 16));
    icon.addFile(":icons/24x24/printPreview.png", QSize(24, 24));
    icon.addFile(":icons/32x32/printPreview.png", QSize(32, 32));
    icon.addFile(":icons/48x48/printPreview.png", QSize(48, 48));
    icon.addFile(":icons/64x64/printPreview.png", QSize(64, 64));
    return icon;
}

QIcon Icons::printToPdfFile() {
    QIcon icon;
    icon.addFile(":icons/16x16/printPdf.png", QSize(16, 16));
    icon.addFile(":icons/24x24/printPdf.png", QSize(24, 24));
    icon.addFile(":icons/32x32/printPdf.png", QSize(32, 32));
    icon.addFile(":icons/48x48/printPdf.png", QSize(48, 48));
    icon.addFile(":icons/64x64/printPdf.png", QSize(64, 64));
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


QIcon Icons::find() {
    QIcon icon;
    icon.addFile(":icons/16x16/find.png", QSize(16, 16));
    icon.addFile(":icons/24x24/find.png", QSize(24, 24));
    icon.addFile(":icons/32x32/find.png", QSize(32, 32));
    icon.addFile(":icons/48x48/find.png", QSize(48, 48));
    icon.addFile(":icons/64x64/find.png", QSize(64, 64));
    return icon;
}

QIcon Icons::findNext() {
    QIcon icon;
    icon.addFile(":icons/16x16/findNext.png", QSize(16, 16));
    icon.addFile(":icons/24x24/findNext.png", QSize(24, 24));
    icon.addFile(":icons/32x32/findNext.png", QSize(32, 32));
    icon.addFile(":icons/48x48/findNext.png", QSize(48, 48));
    icon.addFile(":icons/64x64/findNext.png", QSize(64, 64));
    return icon;
}


QIcon Icons::gotoIcon() {
    QIcon icon;
    icon.addFile(":icons/16x16/goto.png", QSize(16, 16));
    icon.addFile(":icons/24x24/goto.png", QSize(24, 24));
    icon.addFile(":icons/32x32/goto.png", QSize(32, 32));
    icon.addFile(":icons/48x48/goto.png", QSize(48, 48));
    icon.addFile(":icons/64x64/goto.png", QSize(64, 64));
    return icon;
}


QIcon Icons::settings() {
    QIcon icon;
    icon.addFile(":icons/16x16/settings.png", QSize(16, 16));
    icon.addFile(":icons/24x24/settings.png", QSize(24, 24));
    icon.addFile(":icons/32x32/settings.png", QSize(32, 32));
    icon.addFile(":icons/48x48/settings.png", QSize(48, 48));
    icon.addFile(":icons/64x64/settings.png", QSize(64, 64));
    return icon;
}
