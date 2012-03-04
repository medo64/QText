@ECHO OFF


ECHO.
ECHO --- Sign executables


ECHO.
ECHO --- Setup

RMDIR /Q /S ".\Temp" 2> NUL
CALL "%PROGRAMFILES(x86)%\Inno Setup 5\iscc.exe" /O".\Temp" ".\QText.iss"
IF ERRORLEVEL 1 EXIT /B %ERRORLEVEL%

FOR /F %%I IN ('DIR ".\Temp\*.exe" /B') DO SET SETUPEXE=%%I
ECHO Setup is in file %SETUPEXE%


ECHO.
ECHO --- Sign setup


ECHO.
ECHO --- Zip

SET SETUPZIP=%SETUPEXE:.exe=.zip%
ECHO Zipping into %SETUPZIP%
"%PROGRAMFILES%\WinRAR\WinRAR.exe" a -afzip -ep -m5 ".\Temp\%SETUPZIP%" "..\Binaries\QText.exe" "..\Binaries\Readme.txt"
IF ERRORLEVEL 1 EXIT /B %ERRORLEVEL%


ECHO.
ECHO --- Move to releases

MOVE ".\Temp\*.*" "..\Releases\."
IF ERRORLEVEL 1 EXIT /B %ERRORLEVEL%
RMDIR /Q /S ".\Temp"


ECHO.
ECHO Done.
