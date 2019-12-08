#!/bin/bash

if [ -t 1 ]; then
    ESCAPE_RESET="\E[0m"
    ESCAPE_WARNING="\E[33;1m"
    ESCAPE_ERROR="\E[31;1m"
fi


if [[ ! -f "./WinBuild.sh" ]]; then
    echo -e "${ESCAPE_ERROR}Cannot find build script!${ESCAPE_RESET}" >&2
    exit 1
fi


INNOSETUP_PATH="/c/Program Files (x86)/Inno Setup 6/ISCC.exe"

if [[ ! -f "$INNOSETUP_PATH" ]]; then
    echo -e "${ESCAPE_ERROR}Cannot find InnoSetup 6!${ESCAPE_RESET}" >&2
    exit 1
fi


./WinBuild.sh
"$INNOSETUP_PATH" package/win/QText.iss
