#!/bin/bash

if [ -t 1 ]; then
    ESCAPE_RESET="\E[0m"
    ESCAPE_WARNING="\E[33;1m"
    ESCAPE_ERROR="\E[31;1m"
fi


DIST_NAME=qtext
DIST_VERSION=`grep VERSION src/QText.pro | head -1 | cut -d'=' -f2 | awk '{print $$1}' | tr -d '"'`

QT_PATH=/c/Qt
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


# BUILD Release

rm -R build/ 2> /dev/null
mkdir -p build
cd build
PATH=$PATH:`dirname $MAKE_PATH`
$QMAKE_PATH -spec win32-g++ CONFIG+=release ../src/QText.pro
$MAKE_PATH -f Makefile.Release
cd ..
mkdir -p bin/
cp build/release/qtext.exe bin/QText.exe
cp `dirname $QMAKE_PATH`/libgcc_s_seh-1.dll bin/
cp `dirname $QMAKE_PATH`/libstdc++-6.dll bin/
cp `dirname $QMAKE_PATH`/libwinpthread-1.dll bin/
cp `dirname $QMAKE_PATH`/Qt5Core.dll bin/
cp `dirname $QMAKE_PATH`/Qt5Gui.dll bin/
cp `dirname $QMAKE_PATH`/Qt5Network.dll bin/
cp `dirname $QMAKE_PATH`/Qt5Widgets.dll bin/

if [[ $HAS_UNCOMMITTED_RESULT -ne 0 ]]; then
	echo -e "${ESCAPE_WARNING}Uncommitted changes present.${ESCAPE_RESET}" >&2
fi
