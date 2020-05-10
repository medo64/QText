#!/bin/bash

SCRIPT_DIRECTORY=$(dirname $(realpath $0))

command -v astyle >/dev/null
if [[ $? -eq 0 ]]; then
    EXE="astyle"
elif [[ -f "/c/Qt/AStyle/bin/AStyle.exe" ]]; then
    EXE="/c/Qt/AStyle/bin/AStyle.exe"
else
    echo "AStyle not found!" >&2
    exit 1
fi

DIRECTORY=$(realpath "$SCRIPT_DIRECTORY/../src")
cd $DIRECTORY

$EXE --project=.astylerc --suffix=none --recursive --formatted --lineend=linux *.cpp,*.h | tail -n +4
exit $?
