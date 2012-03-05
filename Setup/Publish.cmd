@ECHO OFF

SET            FILE_SETUP=".\QText.iss"
SET         FILE_SOLUTION="..\Source\QText.sln"
SET      FILES_EXECUTABLE="..\Binaries\QText.exe"
SET           FILES_OTHER="..\Binaries\ReadMe.txt"
SET   CERTIFICATE_SUBJECT="Josip Medved"
SET CERTIFICATE_TIMESTAMP=


ECHO --- BUILD SOLUTION
ECHO.

RMDIR /Q /S "..\Binaries" 2> NUL
"%PROGRAMFILES(X86)%\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe" /Build "Release" %FILE_SOLUTION%
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

ECHO.


ECHO --- SIGN SOLUTION
ECHO.

IF [%CERTIFICATE_TIMESTAMP%]==[] (
    "\Tools\SignTool\signtool.exe" sign /s "My" /n %CERTIFICATE_SUBJECT% /v %FILES_EXECUTABLE%
) ELSE (
    "\Tools\SignTool\signtool.exe" sign /s "My" /n %CERTIFICATE_SUBJECT% /tr %CERTIFICATE_TIMESTAMP% /v %FILES_EXECUTABLE%
)
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

ECHO.


ECHO --- BUILD SETUP
ECHO.

RMDIR /Q /S ".\Temp" 2> NUL
CALL "%PROGRAMFILES(x86)%\Inno Setup 5\iscc.exe" /O".\Temp" %FILE_SETUP%
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

FOR /F %%I IN ('DIR ".\Temp\*.exe" /B') DO SET _SETUPEXE=%%I
ECHO Setup is in file %_SETUPEXE%

ECHO.


ECHO --- SIGN SETUP
ECHO.

IF [%CERTIFICATE_TIMESTAMP%]==[] (
    "\Tools\SignTool\signtool.exe" sign /s "My" /n %CERTIFICATE_SUBJECT% /v ".\Temp\%_SETUPEXE%"
) ELSE (
    "\Tools\SignTool\signtool.exe" sign /s "My" /n %CERTIFICATE_SUBJECT% /tr %CERTIFICATE_TIMESTAMP% /v ".\Temp\%_SETUPEXE%"
)
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

ECHO.


ECHO --- BUILD ZIP
ECHO.

SET _SETUPZIP=%_SETUPEXE:.exe=.zip%
ECHO Zipping into %_SETUPZIP%
"%PROGRAMFILES%\WinRAR\WinRAR.exe" a -afzip -ep -m5 ".\Temp\%_SETUPZIP%" %FILES_EXECUTABLE% %FILES_OTHER%
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

ECHO.


ECHO --- RELEASE
ECHO.

MOVE ".\Temp\*.*" "..\Releases\."
IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%
RMDIR /Q /S ".\Temp"

ECHO.


ECHO.
ECHO Done.
ECHO.

PAUSE
