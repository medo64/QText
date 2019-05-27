QT       += core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = qtext
TEMPLATE = app

DEFINES += QT_DEPRECATED_WARNINGS

CONFIG += c++11

SOURCES += \
        fileitem.cpp \
        filenamedialog.cpp \
        folderitem.cpp \
        helpers.cpp \
        main.cpp \
        mainwindow.cpp \
        storage.cpp

HEADERS += \
        fileitem.h \
        filenamedialog.h \
        folderitem.h \
        helpers.h \
        mainwindow.h \
        storage.h

FORMS += \
        filenamedialog.ui \
        mainwindow.ui

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target

RESOURCES += \
    qtext.qrc

DISTFILES +=

RC_ICONS = icons/qtext.ico
