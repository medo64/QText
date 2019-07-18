QT       += core gui widgets
QT       += network  # QLocalServer/QLocalSocket

unix:QT  += x11extras
unix:LIBS += -lX11 -lxcb


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
