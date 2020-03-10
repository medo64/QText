APP_PRODUCT = "QText"
APP_COMPANY = "Josip Medved"
APP_VERSION = "0.1.4"
APP_COPYRIGHT = "Copyright 2004 Josip Medved <jmedved@jmedved.com>"
APP_DESCRIPTION = "Note taking utility with auto-save."

DEFINES += "APP_PRODUCT=\"\\\"$$APP_PRODUCT\\\"\""
DEFINES += "APP_COMPANY=\"\\\"$$APP_COMPANY\\\"\""
DEFINES += "APP_VERSION=\\\"$$APP_VERSION\\\""
DEFINES += "APP_COPYRIGHT=\"\\\"$$APP_COPYRIGHT\\\"\""
DEFINES += "APP_DESCRIPTION=\"\\\"$$APP_DESCRIPTION\\\"\""

APP_COMMIT = $$system(git -C \"$$_PRO_FILE_PWD_\" log -n 1 --format=%h)
APP_COMMIT_DIRTY = $$system(git -C \"$$_PRO_FILE_PWD_\" diff --quiet ; echo $?)
!equals(APP_COMMIT_DIRTY, 0) {
    APP_COMMIT = $$upper($$APP_COMMIT) #upper-case if working directory is dirty
}
DEFINES += "APP_COMMIT=\"\\\"$$APP_COMMIT\\\"\""

DEFINES += "APP_QT_VERSION=\\\"$$QT_VERSION\\\""


QT       += core gui widgets
QT       += network  # QLocalServer/QLocalSocket
QT       += printsupport

unix {
    QT  += x11extras
    LIBS += -lX11 -lxcb
}

win32 {
    QMAKE_TARGET_PRODUCT = $$APP_PRODUCT
    QMAKE_TARGET_COMPANY = $$APP_COMPANY
    QMAKE_TARGET_COPYRIGHT = $$APP_COPYRIGHT
    QMAKE_TARGET_DESCRIPTION = $$APP_DESCRIPTION
    RC_ICONS = icons/qtext.ico
    VERSION = $$APP_VERSION + ".0"
}

DEFINES += QT_DEPRECATED_WARNINGS
CONFIG(release, debug|release):DEFINES += QT_NO_DEBUG_OUTPUT

CONFIG += c++11

SOURCES += \
        ui/filenamedialog.cpp \
        ui/foldersdialog.cpp \
        ui/gotodialog.cpp \
        ui/inserttimedialog.cpp \
        ui/settingsdialog.cpp \
        ui/mainwindow.cpp \
        ui/qtabbarex.cpp \
        ui/qtabwidgetex.cpp \
        medo/config.cpp \
        medo/hotkey.cpp \
        medo/singleinstance.cpp \
        medo/state.cpp \
        clipboard.cpp \
        fileitem.cpp \
        folderitem.cpp \
        helpers.cpp \
        icons.cpp \
        main.cpp \
        settings.cpp \
        setup.cpp \
        storage.cpp

HEADERS += \
        ui/filenamedialog.h \
        ui/foldersdialog.h \
        ui/gotodialog.h \
        ui/inserttimedialog.h \
        ui/settingsdialog.h \
        ui/mainwindow.h \
        ui/qtabbarex.h \
        ui/qtabwidgetex.h \
        medo/config.h \
        medo/hotkey.h \
        medo/singleinstance.h \
        medo/state.h \
        clipboard.h \
        fileitem.h \
        folderitem.h \
        helpers.h \
        icons.h \
        settings.h \
        setup.h \
        storage.h

FORMS += \
        ui/filenamedialog.ui \
        ui/foldersdialog.ui \
        ui/gotodialog.ui \
        ui/inserttimedialog.ui \
        ui/settingsdialog.ui \
        ui/mainwindow.ui

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target

RESOURCES += \
    qtext.qrc

DISTFILES +=


Test {
    QT += testlib
    QT -= gui

    CONFIG += qst console warn_on depend_includepath testcase
    CONFIG -= app_bundle

    TEMPLATE = app
    TARGET = qtexttests

    SOURCES -= main.cpp
    SOURCES += \
            test/test_config.cpp \
            test/test_helpers.cpp \
            test/test_main.cpp

    HEADERS += \
            test/test_config.h \
            test/test_helpers.h

    RESOURCES += \
        test.qrc

} else {
    TEMPLATE = app
    TARGET = qtext
}
