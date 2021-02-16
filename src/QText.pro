APP_PRODUCT = "QText"
APP_COMPANY = "Josip Medved"
APP_VERSION = "0.1.18"
APP_COPYRIGHT = "Copyright 2004 Josip Medved <jmedved@jmedved.com>"
APP_DESCRIPTION = "Note taking utility with auto-save."

DEFINES += "APP_PRODUCT=\"\\\"$$APP_PRODUCT\\\"\""
DEFINES += "APP_COMPANY=\"\\\"$$APP_COMPANY\\\"\""
DEFINES += "APP_VERSION=\\\"$$APP_VERSION\\\""
DEFINES += "APP_COPYRIGHT=\"\\\"$$APP_COPYRIGHT\\\"\""
DEFINES += "APP_DESCRIPTION=\"\\\"$$APP_DESCRIPTION\\\"\""
DEFINES += "APP_QT_VERSION=\\\"$$QT_VERSION\\\""

APP_COMMIT = $$system(git -C \"$$_PRO_FILE_PWD_\" log -n 1 --format=%h)
APP_COMMIT_DIFF = $$system(git -C \"$$_PRO_FILE_PWD_\" diff)
!isEmpty(APP_COMMIT_DIFF) {
    APP_COMMIT = $$upper($$APP_COMMIT) #upper-case if working directory is dirty
}
DEFINES += "APP_COMMIT=\"\\\"$$APP_COMMIT\\\"\""


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
    RC_ICONS = icons/app.ico
    VERSION = $$APP_VERSION + ".0"
}

DEFINES += QT_DEPRECATED_WARNINGS
CONFIG(release, debug|release):DEFINES += QT_NO_DEBUG_OUTPUT

CONFIG += c++11
QMAKE_CXXFLAGS_WARN_ON += -Wall
QMAKE_CXXFLAGS_WARN_ON += -Wextra
QMAKE_CXXFLAGS_WARN_ON += -Wshadow
QMAKE_CXXFLAGS_WARN_ON += -Wdouble-promotion


SOURCES += \
        medo/about.cpp \
        ui/finddialog.cpp \
        ui/foldersdialog.cpp \
        ui/gotodialog.cpp \
        ui/hotkeyedit.cpp \
        ui/inserttimedialog.cpp \
        ui/settingsdialog.cpp \
        ui/mainwindow.cpp \
        ui/newfiledialog.cpp \
        ui/qtabbarex.cpp \
        ui/qtabwidgetex.cpp \
        ui/phoneticdialog.cpp \
        ui/renamefiledialog.cpp \
        medo/appsetupmutex.cpp \
        medo/config.cpp \
        medo/feedback.cpp \
        medo/hotkey.cpp \
        medo/singleinstance.cpp \
        medo/state.cpp \
        medo/upgrade.cpp \
        storage/fileitem.cpp \
        storage/folderitem.cpp \
        storage/storage.cpp \
        storage/storagemonitorlocker.cpp \
        storage/storagemonitorthread.cpp \
        app.cpp \
        clipboard.cpp \
        deletion.cpp \
        find.cpp \
        helpers.cpp \
        icons.cpp \
        phoneticalphabet.cpp \
        rtfconverter.cpp \
        settings.cpp \
        setup.cpp

HEADERS += \
        medo/about.h \
        ui/finddialog.h \
        ui/foldersdialog.h \
        ui/gotodialog.h \
        ui/hotkeyedit.h \
        ui/inserttimedialog.h \
        ui/mainwindow.h \
        ui/newfiledialog.h \
        ui/qtabbarex.h \
        ui/qtabwidgetex.h \
        ui/phoneticdialog.h \
        ui/renamefiledialog.h \
        ui/settingsdialog.h \
        medo/appsetupmutex.h \
        medo/config.h \
        medo/feedback.h \
        medo/hotkey.h \
        medo/singleinstance.h \
        medo/state.h \
        medo/upgrade.h \
        storage/deletionstyle.h \
        storage/fileitem.h \
        storage/filetype.h \
        storage/folderitem.h \
        storage/storage.h \
        storage/storagemonitorlocker.h \
        storage/storagemonitorthread.h \
        clipboard.h \
        deletion.h \
        find.h \
        helpers.h \
        icons.h \
        phoneticalphabet.h \
        rtfconverter.h \
        settings.h \
        setup.h

FORMS += \
        ui/finddialog.ui \
        ui/foldersdialog.ui \
        ui/gotodialog.ui \
        ui/inserttimedialog.ui \
        ui/newfiledialog.ui \
        ui/phoneticdialog.ui \
        ui/renamefiledialog.ui \
        ui/settingsdialog.ui \
        ui/mainwindow.ui

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target

RESOURCES += \
    qtext.qrc

DISTFILES += \
    .astylerc


test {
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
