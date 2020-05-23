#!/bin/bash

QT_PATH='/c/Qt'
CERTIFICATE_THUMBPRINT="026184de8dbf52fdcbae75fd6b1a7d9ce4310e5d"
TIMESTAMP_URL="http://timestamp.comodoca.com/rfc3161"


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
    'clean')   BUILD=''        ; DO_PACKAGE=0 ;;
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


CMD_QMAKE=`ls $QT_PATH/**/**/bin/qmake.exe | sort | tail -1`
CMD_MAKE=`ls $QT_PATH/Tools/**/bin/mingw32-make.exe | sort | tail -1`

if [[ ! -f "$CMD_QMAKE" ]]; then
    echo -e "${ESCAPE_ERROR}Cannot find qmake!${ESCAPE_RESET}" >&2
    exit 1
fi
QMAKE_DIR=`dirname $CMD_QMAKE`

if [[ ! -f "$CMD_MAKE" ]]; then
    echo -e "${ESCAPE_ERROR}Cannot find make!${ESCAPE_RESET}" >&2
    exit 1
fi


CMD_CERTUTIL=`command -v certutil`
if [[ ! -f "$CMD_CERTUTIL" ]]; then
    echo -e "${ESCAPE_WARNING}Cannot find certutil!${ESCAPE_RESET}" >&2
    CERTIFICATE_THUMBPRINT=""
elif [[ "$CERTIFICATE_THUMBPRINT" == "" ]]; then
    echo -e "${ESCAPE_WARNING}No signing certificate thumbprint!${ESCAPE_RESET}" >&2
else
    $CMD_CERTUTIL -silent -verifystore -user My $CERTIFICATE_THUMBPRINT > /dev/null
    if [[ $? -ne 0 ]]; then
        echo -e "${ESCAPE_WARNING}Cannot validate certificate thumbprint!${ESCAPE_RESET}" >&2
        CERTIFICATE_THUMBPRINT=""
    fi
fi

CMD_SIGNTOOL=""
for SIGNTOOL_PATH in "/c/Program Files (x86)/Microsoft SDKs/ClickOnce/SignTool/signtool.exe" \
            "/c/Program Files (x86)/Windows Kits/10/App Certification Kit/signtool.exe" \
            "/c/Program Files (x86)/Windows Kits/10/bin/x86/signtool.exe"; do
    if [[ -f "$SIGNTOOL_PATH" ]]; then
        CMD_SIGNTOOL="$SIGNTOOL_PATH"
        break
    fi
done

if [[ ! -f "$CMD_SIGNTOOL" ]]; then
    echo -e "${ESCAPE_WARNING}Cannot find signtool!${ESCAPE_RESET}" >&2
fi


HAS_UNCOMMITTED_RESULT=`git diff --quiet ; echo $?`


rm bin/* 2> /dev/null
mkdir -p bin
rm -R build/ 2> /dev/null
mkdir -p build
cd build


if [[ "$BUILD" != "" ]]; then
    PATH=$PATH:`dirname $CMD_MAKE`

    $CMD_QMAKE -spec win32-g++ CONFIG+=$BUILD ../src/QText.pro
    if [[ $? -ne 0 ]]; then
        echo -e "${ESCAPE_ERROR}QMake failed!${ESCAPE_RESET}" >&2
        exit 1
    fi

    $CMD_MAKE -f Makefile
    if [[ $? -ne 0 ]]; then
        echo -e "${ESCAPE_ERROR}Make failed!${ESCAPE_RESET}" >&2
        exit 1
    fi


    case $BUILD in
        'release')
            cp release/qtext.exe              ../bin/QText.exe
            cp $QMAKE_DIR/libgcc_s_seh-1.dll  ../bin/
            cp $QMAKE_DIR/libstdc++-6.dll     ../bin/
            cp $QMAKE_DIR/libwinpthread-1.dll ../bin/
            cp $QMAKE_DIR/Qt5Core.dll         ../bin/
            cp $QMAKE_DIR/Qt5Gui.dll          ../bin/
            cp $QMAKE_DIR/Qt5Network.dll      ../bin/
            cp $QMAKE_DIR/Qt5PrintSupport.dll ../bin/
            cp $QMAKE_DIR/Qt5Widgets.dll      ../bin/

            if [[ "$CERTIFICATE_THUMBPRINT" != "" ]]; then
                echo
                if [[ "$TIMESTAMP_URL" != "" ]]; then
                    "$CMD_SIGNTOOL" sign -s "My" -sha1 $CERTIFICATE_THUMBPRINT -tr $TIMESTAMP_URL -v ../bin/QText.exe
                else
                    "$CMD_SIGNTOOL" sign -s "My" -sha1 $CERTIFICATE_THUMBPRINT -v ../bin/QText.exe
                fi
            fi

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
                    LAST_PACKAGE=`ls -t dist/*.exe | head -1`

                    if [[ "$CERTIFICATE_THUMBPRINT" != "" ]]; then
                        echo
                        if [[ "$TIMESTAMP_URL" != "" ]]; then
                            "$CMD_SIGNTOOL" sign -s "My" -sha1 $CERTIFICATE_THUMBPRINT -tr $TIMESTAMP_URL -v $LAST_PACKAGE
                        else
                            "$CMD_SIGNTOOL" sign -s "My" -sha1 $CERTIFICATE_THUMBPRINT -v $LAST_PACKAGE
                        fi
                    fi

                    echo
                    echo -e "${ESCAPE_RESULT}Package created ($LAST_PACKAGE).${ESCAPE_RESET}" >&2
                else
                    echo -e "${ESCAPE_ERROR}Packaging failed!${ESCAPE_RESET}" >&2
                    exit 1
                fi
            fi
            ;;

        'debug')
            cp debug/qtext.exe                 ../bin/QText.exe
            cp $QMAKE_DIR/libgcc_s_seh-1.dll   ../bin/
            cp $QMAKE_DIR/libstdc++-6.dll      ../bin/
            cp $QMAKE_DIR/libwinpthread-1.dll  ../bin/
            cp $QMAKE_DIR/Qt5Cored.dll         ../bin/
            cp $QMAKE_DIR/Qt5Guid.dll          ../bin/
            cp $QMAKE_DIR/Qt5Networkd.dll      ../bin/
            cp $QMAKE_DIR/Qt5PrintSupportd.dll ../bin/
            cp $QMAKE_DIR/Qt5Widgetsd.dll      ../bin/

            echo -e "${ESCAPE_RESULT}Debug build completed.${ESCAPE_RESET}" >&2
            ;;

        'test')
            cp $QMAKE_DIR/libgcc_s_seh-1.dll  release/
            cp $QMAKE_DIR/libstdc++-6.dll     release/
            cp $QMAKE_DIR/libwinpthread-1.dll release/
            cp $QMAKE_DIR/Qt5Core.dll         release/
            cp $QMAKE_DIR/Qt5Gui.dll          release/
            cp $QMAKE_DIR/Qt5Network.dll      release/
            cp $QMAKE_DIR/Qt5PrintSupport.dll release/
            cp $QMAKE_DIR/Qt5Test.dll         release/
            cp $QMAKE_DIR/Qt5Widgets.dll      release/

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
fi