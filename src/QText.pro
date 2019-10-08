APP_PRODUCT = "QText"
APP_COMPANY = "Josip Medved"
APP_VERSION = "0.0.1"
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
    VERSION = $$APP_VERSION.0
}

DEFINES += QT_DEPRECATED_WARNINGS
CONFIG(release, debug|release):DEFINES += QT_NO_DEBUG_OUTPUT

CONFIG += c++11

SOURCES += \
        medo/config.cpp \
        medo/hotkey.cpp \
        medo/singleinstance.cpp \
        medo/state.cpp \
        fileitem.cpp \
        filenamedialog.cpp \
        folderitem.cpp \
        helpers.cpp \
        main.cpp \
        mainwindow.cpp \
        qtabbarex.cpp \
        qtabwidgetex.cpp \
        settings.cpp \
        settingsdialog.cpp \
        setup.cpp \
        storage.cpp

HEADERS += \
        medo/config.h \
        medo/hotkey.h \
        medo/singleinstance.h \
        medo/state.h \
        fileitem.h \
        filenamedialog.h \
        folderitem.h \
        helpers.h \
        mainwindow.h \
        qtabbarex.h \
        qtabwidgetex.h \
        settings.h \
        settingsdialog.h \
        setup.h \
        storage.h

FORMS += \
        filenamedialog.ui \
        settingsdialog.ui \
        mainwindow.ui

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
            test_config.cpp \
            test_helpers.cpp \
            test_main.cpp

    HEADERS += \
            test_config.h \
            test_helpers.h

    RESOURCES += \
        test.qrc

} else {
    TEMPLATE = app
    TARGET = qtext
}
