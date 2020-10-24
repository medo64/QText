#include <QImage>
#include "icons.h"

bool Icons::_darkMode = false;

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
    icon.addPixmap(getPixmap("new", 16));
    icon.addPixmap(getPixmap("new", 24));
    icon.addPixmap(getPixmap("new", 32));
    icon.addPixmap(getPixmap("new", 48));
    icon.addPixmap(getPixmap("new", 64));
    return icon;
}

QIcon Icons::saveFile() {
    QIcon icon;
    icon.addPixmap(getPixmap("save", 16));
    icon.addPixmap(getPixmap("save", 24));
    icon.addPixmap(getPixmap("save", 32));
    icon.addPixmap(getPixmap("save", 48));
    icon.addPixmap(getPixmap("save", 64));
    return icon;
}

QIcon Icons::renameFile() {
    QIcon icon;
    icon.addPixmap(getPixmap("rename", 16));
    icon.addPixmap(getPixmap("rename", 24));
    icon.addPixmap(getPixmap("rename", 32));
    icon.addPixmap(getPixmap("rename", 48));
    icon.addPixmap(getPixmap("rename", 64));
    return icon;
}

QIcon Icons::deleteFile() {
    QIcon icon;
    icon.addPixmap(getPixmap("delete", 16));
    icon.addPixmap(getPixmap("delete", 24));
    icon.addPixmap(getPixmap("delete", 32));
    icon.addPixmap(getPixmap("delete", 48));
    icon.addPixmap(getPixmap("delete", 64));
    return icon;
}

QIcon Icons::printFile() {
    QIcon icon;
    icon.addPixmap(getPixmap("print", 16));
    icon.addPixmap(getPixmap("print", 24));
    icon.addPixmap(getPixmap("print", 32));
    icon.addPixmap(getPixmap("print", 48));
    icon.addPixmap(getPixmap("print", 64));
    return icon;
}

QIcon Icons::printPreviewFile() {
    QIcon icon;
    icon.addPixmap(getPixmap("printPreview", 16));
    icon.addPixmap(getPixmap("printPreview", 24));
    icon.addPixmap(getPixmap("printPreview", 32));
    icon.addPixmap(getPixmap("printPreview", 48));
    icon.addPixmap(getPixmap("printPreview", 64));
    return icon;
}

QIcon Icons::printToPdfFile() {
    QIcon icon;
    icon.addPixmap(getPixmap("printPdf", 16));
    icon.addPixmap(getPixmap("printPdf", 24));
    icon.addPixmap(getPixmap("printPdf", 32));
    icon.addPixmap(getPixmap("printPdf", 48));
    icon.addPixmap(getPixmap("printPdf", 64));
    return icon;
}


QIcon Icons::cut() {
    QIcon icon;
    icon.addPixmap(getPixmap("cut", 16));
    icon.addPixmap(getPixmap("cut", 24));
    icon.addPixmap(getPixmap("cut", 32));
    icon.addPixmap(getPixmap("cut", 48));
    icon.addPixmap(getPixmap("cut", 64));
    return icon;
}

QIcon Icons::copy() {
    QIcon icon;
    icon.addPixmap(getPixmap("copy", 16));
    icon.addPixmap(getPixmap("copy", 24));
    icon.addPixmap(getPixmap("copy", 32));
    icon.addPixmap(getPixmap("copy", 48));
    icon.addPixmap(getPixmap("copy", 64));
    return icon;
}

QIcon Icons::paste() {
    QIcon icon;
    icon.addPixmap(getPixmap("paste", 16));
    icon.addPixmap(getPixmap("paste", 24));
    icon.addPixmap(getPixmap("paste", 32));
    icon.addPixmap(getPixmap("paste", 48));
    icon.addPixmap(getPixmap("paste", 64));
    return icon;
}


QIcon Icons::undo() {
    QIcon icon;
    icon.addPixmap(getPixmap("undo", 16));
    icon.addPixmap(getPixmap("undo", 24));
    icon.addPixmap(getPixmap("undo", 32));
    icon.addPixmap(getPixmap("undo", 48));
    icon.addPixmap(getPixmap("undo", 64));
    return icon;
}

QIcon Icons::redo() {
    QIcon icon;
    icon.addPixmap(getPixmap("redo", 16));
    icon.addPixmap(getPixmap("redo", 24));
    icon.addPixmap(getPixmap("redo", 32));
    icon.addPixmap(getPixmap("redo", 48));
    icon.addPixmap(getPixmap("redo", 64));
    return icon;
}


QIcon Icons::fontBold() {
    QIcon icon;
    icon.addPixmap(getPixmap("bold", 16));
    icon.addPixmap(getPixmap("bold", 24));
    icon.addPixmap(getPixmap("bold", 32));
    icon.addPixmap(getPixmap("bold", 48));
    icon.addPixmap(getPixmap("bold", 64));
    return icon;
}

QIcon Icons::fontItalic() {
    QIcon icon;
    icon.addPixmap(getPixmap("italic", 16));
    icon.addPixmap(getPixmap("italic", 24));
    icon.addPixmap(getPixmap("italic", 32));
    icon.addPixmap(getPixmap("italic", 48));
    icon.addPixmap(getPixmap("italic", 64));
    return icon;
}

QIcon Icons::fontUnderline() {
    QIcon icon;
    icon.addPixmap(getPixmap("underline", 16));
    icon.addPixmap(getPixmap("underline", 24));
    icon.addPixmap(getPixmap("underline", 32));
    icon.addPixmap(getPixmap("underline", 48));
    icon.addPixmap(getPixmap("underline", 64));
    return icon;
}

QIcon Icons::fontStrikethrough() {
    QIcon icon;
    icon.addPixmap(getPixmap("strikethrough", 16));
    icon.addPixmap(getPixmap("strikethrough", 24));
    icon.addPixmap(getPixmap("strikethrough", 32));
    icon.addPixmap(getPixmap("strikethrough", 48));
    icon.addPixmap(getPixmap("strikethrough", 64));
    return icon;
}


QIcon Icons::find() {
    QIcon icon;
    icon.addPixmap(getPixmap("find", 16));
    icon.addPixmap(getPixmap("find", 24));
    icon.addPixmap(getPixmap("find", 32));
    icon.addPixmap(getPixmap("find", 48));
    icon.addPixmap(getPixmap("find", 64));
    return icon;
}

QIcon Icons::findNext() {
    QIcon icon;
    icon.addPixmap(getPixmap("findNext", 16));
    icon.addPixmap(getPixmap("findNext", 24));
    icon.addPixmap(getPixmap("findNext", 32));
    icon.addPixmap(getPixmap("findNext", 48));
    icon.addPixmap(getPixmap("findNext", 64));
    return icon;
}


QIcon Icons::gotoIcon() {
    QIcon icon;
    icon.addPixmap(getPixmap("goto", 16));
    icon.addPixmap(getPixmap("goto", 24));
    icon.addPixmap(getPixmap("goto", 32));
    icon.addPixmap(getPixmap("goto", 48));
    icon.addPixmap(getPixmap("goto", 64));
    return icon;
}


QIcon Icons::settings() {
    QIcon icon;
    icon.addPixmap(getPixmap("settings", 16));
    icon.addPixmap(getPixmap("settings", 24));
    icon.addPixmap(getPixmap("settings", 32));
    icon.addPixmap(getPixmap("settings", 48));
    icon.addPixmap(getPixmap("settings", 64));
    return icon;
}

QIcon Icons::settingsWithUpgrade() {
    QIcon icon;
    icon.addPixmap(getPixmap("settingsWithUpgrade", 16));
    icon.addPixmap(getPixmap("settingsWithUpgrade", 24));
    icon.addPixmap(getPixmap("settingsWithUpgrade", 32));
    icon.addPixmap(getPixmap("settingsWithUpgrade", 48));
    icon.addPixmap(getPixmap("settingsWithUpgrade", 64));
    return icon;
}


QIcon Icons::gotoFile() {
    QIcon icon;
    icon.addPixmap(getPixmap("file", 16));
    icon.addPixmap(getPixmap("file", 24));
    icon.addPixmap(getPixmap("file", 32));
    icon.addPixmap(getPixmap("file", 48));
    icon.addPixmap(getPixmap("file", 64));
    return icon;
}

QIcon Icons::gotoFolder() {
    QIcon icon;
    icon.addPixmap(getPixmap("folder", 16));
    icon.addPixmap(getPixmap("folder", 24));
    icon.addPixmap(getPixmap("folder", 32));
    icon.addPixmap(getPixmap("folder", 48));
    icon.addPixmap(getPixmap("folder", 64));
    return icon;
}


void Icons::setDarkMode(bool darkMode) {
    _darkMode = darkMode;
}


QPixmap Icons::getPixmap(QString baseName, int size) {
    QString fileName = QString(":icons/%1x%1/%2.png").arg(QString::number(size), baseName);
    QImage image(fileName);
    if (_darkMode) {
        image.invertPixels();
        //remove yellow tint, naive way
        for (int x = 0; x < image.width(); x++) {
            for (int y = 0; y < image.height(); y++) {
                QColor color = image.pixelColor(x, y);
                QColor newColor = QColor::fromHsv(color.hue() + 165,
                                                  color.saturation(),
                                                  color.value(),
                                                  color.alpha()).lighter(125);
                image.setPixelColor(x, y, newColor);
            }
        }
    }
    return QPixmap::fromImage(image);
}
