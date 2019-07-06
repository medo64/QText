QT       += core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets


DEFINES += QT_DEPRECATED_WARNINGS

CONFIG += c++11

SOURCES += \
        config.cpp \
        fileitem.cpp \
        filenamedialog.cpp \
        folderitem.cpp \
        helpers.cpp \
        hotkey.cpp \
        main.cpp \
        mainwindow.cpp \
        qtabbarex.cpp \
        qtabwidgetex.cpp \
        storage.cpp

HEADERS += \
        config.h \
        fileitem.h \
        filenamedialog.h \
        folderitem.h \
        helpers.h \
        hotkey.h \
        mainwindow.h \
        qtabbarex.h \
        qtabwidgetex.h \
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

} else {
    TEMPLATE = app
    TARGET = qtext
}