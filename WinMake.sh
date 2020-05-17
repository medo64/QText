#!/bin/bash

if [ -t 1 ]; then
    ESCAPE_RESET="\E[0m"
    ESCAPE_WARNING="\E[33;1m"
    ESCAPE_ERROR="\E[31;1m"
    ESCAPE_RESULT="\E[32;1m"
fi

if [[ "$2" != "" ]]; then
    echo "Invalid argument count!" >&2
    exit 1
fi

case $1 in
    '')        BUILD='release' ; DO_PACKAGE=0 ;;
    'debug')   BUILD='debug'   ; DO_PACKAGE=0 ;;
    'release') BUILD='release' ; DO_PACKAGE=0 ;;
    'test')    BUILD='test'    ; DO_PACKAGE=0 ;;
    'package') BUILD='release' ; DO_PACKAGE=1 ;;

    *)
        echo "Invalid operation!" >&2
        exit 1
        ;;
esac


DIST_NAME='qtext'
DIST_VERSION=`grep VERSION src/QText.pro | head -1 | cut -d'=' -f2 | awk '{print $$1}' | tr -d '"'`

QT_PATH='/c/Qt'
QMAKE_PATH=`ls $QT_PATH/**/**/bin/qmake.exe | sort | tail -1`
MAKE_PATH=`ls $QT_PATH/Tools/**/bin/mingw32-make.exe | sort | tail -1`

if [[ ! -f "$QMAKE_PATH" ]]; then
    echo -e "${ESCAPE_ERROR}Cannot find qmake!${ESCAPE_RESET}" >&2
    exit 1
fi

if [[ ! -f "$MAKE_PATH" ]]; then
    echo -e "${ESCAPE_ERROR}Cannot find make!${ESCAPE_RESET}" >&2
    exit 1
fi

HAS_UNCOMMITTED_RESULT=`git diff --quiet ; echo $?`


rm -R build/ 2> /dev/null
mkdir -p build
cd build

PATH=$PATH:`dirname $MAKE_PATH`

$QMAKE_PATH -spec win32-g++ CONFIG+=$BUILD ../src/QText.pro
if [[ $? -ne 0 ]]; then
    echo -e "${ESCAPE_ERROR}QMake failed!${ESCAPE_RESET}" >&2
    exit 1
fi

$MAKE_PATH -f Makefile
if [[ $? -ne 0 ]]; then
    echo -e "${ESCAPE_ERROR}Make failed!${ESCAPE_RESET}" >&2
    exit 1
fi

case $BUILD in
    'release')
        rm ../bin/* 2> /dev/null
        mkdir -p ../bin

        cp release/qtext.exe ../bin/QText.exe
        cp `dirname $QMAKE_PATH`/libgcc_s_seh-1.dll ../bin/
        cp `dirname $QMAKE_PATH`/libstdc++-6.dll ../bin/
        cp `dirname $QMAKE_PATH`/libwinpthread-1.dll ../bin/
        cp `dirname $QMAKE_PATH`/Qt5Core.dll ../bin/
        cp `dirname $QMAKE_PATH`/Qt5Gui.dll ../bin/
        cp `dirname $QMAKE_PATH`/Qt5Network.dll ../bin/
        cp `dirname $QMAKE_PATH`/Qt5PrintSupport.dll ../bin/
        cp `dirname $QMAKE_PATH`/Qt5Widgets.dll ../bin/

        if [[ $HAS_UNCOMMITTED_RESULT -ne 0 ]] && [[ "$BUILD" == 'release' ]]; then
            echo -e "${ESCAPE_WARNING}Uncommitted changes present.${ESCAPE_RESET}" >&2
        fi

        echo -e "${ESCAPE_RESULT}Release build completed.${ESCAPE_RESET}" >&2

        if [[ $DO_PACKAGE -ne 0 ]]; then
            echo

            INNOSETUP_PATH='/c/Program Files (x86)/Inno Setup 6/ISCC.exe'

            if [[ ! -f "$INNOSETUP_PATH" ]]; then
                echo -e "${ESCAPE_ERROR}Cannot find InnoSetup 6!${ESCAPE_RESET}" >&2
                exit 1
            fi

            cd ..
            "$INNOSETUP_PATH" package/win/QText.iss
            if [[ $? -eq 0 ]]; then
                echo -e "${ESCAPE_RESULT}Package created.${ESCAPE_RESET}" >&2
            else
                echo -e "${ESCAPE_ERROR}Packaging failed!${ESCAPE_RESET}" >&2
                exit 1
            fi
        fi
        ;;

    'debug')
        rm ../bin/* 2> /dev/null
        mkdir -p ../bin

        cp debug/qtext.exe ../bin/QText.exe
        cp `dirname $QMAKE_PATH`/libgcc_s_seh-1.dll ../bin/
        cp `dirname $QMAKE_PATH`/libstdc++-6.dll ../bin/
        cp `dirname $QMAKE_PATH`/libwinpthread-1.dll ../bin/
        cp `dirname $QMAKE_PATH`/Qt5Cored.dll ../bin/
        cp `dirname $QMAKE_PATH`/Qt5Guid.dll ../bin/
        cp `dirname $QMAKE_PATH`/Qt5Networkd.dll ../bin/
        cp `dirname $QMAKE_PATH`/Qt5PrintSupportd.dll ../bin/
        cp `dirname $QMAKE_PATH`/Qt5Widgetsd.dll ../bin/

        echo -e "${ESCAPE_RESULT}Debug build completed.${ESCAPE_RESET}" >&2
        ;;

    'test')
        cp `dirname $QMAKE_PATH`/libgcc_s_seh-1.dll release/
        cp `dirname $QMAKE_PATH`/libstdc++-6.dll release/
        cp `dirname $QMAKE_PATH`/libwinpthread-1.dll release/
        cp `dirname $QMAKE_PATH`/Qt5Core.dll release/
        cp `dirname $QMAKE_PATH`/Qt5Gui.dll release/
        cp `dirname $QMAKE_PATH`/Qt5Network.dll release/
        cp `dirname $QMAKE_PATH`/Qt5PrintSupport.dll release/
        cp `dirname $QMAKE_PATH`/Qt5Test.dll release/
        cp `dirname $QMAKE_PATH`/Qt5Widgets.dll release/

        echo
        cd release
        ./qtexttests.exe \
            | GREP_COLOR='01;31' grep --color=always -e '^FAIL! ' -e '^' \
            | GREP_COLOR='01;32' grep --color=always -e '^PASS ' -e '^'
        if [[ $? -ne 0 ]]; then
            echo -e "${ESCAPE_ERROR}Testing failed!${ESCAPE_RESET}" >&2
            exit 1
        fi
        ;;
esac
