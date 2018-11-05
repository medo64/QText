@ECHO OFF
SETLOCAL EnableDelayedExpansion

SET        SOURCE_SOLUTION="..\Source\QText.sln"
SET       SOURCE_INNOSETUP=".\QText.iss"
SET       FILES_EXECUTABLE="..\Binaries\QText.exe" "..\Binaries\QText.Document.dll"
SET            FILES_OTHER="..\README.md" "..\LICENSE.md"

SET              TOOLS_GIT="%PROGRAMFILES(X86)%\Git\mingw64\bin\git.exe" "%PROGRAMFILES%\Git\mingw64\bin\git.exe" "C:\Program Files\Git\mingw64\bin\git.exe"
SET     TOOLS_VISUALSTUDIO="%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe"
SET         TOOLS_SIGNTOOL="%PROGRAMFILES(X86)%\Microsoft SDKs\ClickOnce\SignTool\signtool.exe" "%PROGRAMFILES(X86)%\Windows Kits\10\App Certification Kit\signtool.exe" "%PROGRAMFILES(X86)%\Windows Kits\10\bin\x86\signtool.exe"
SET        TOOLS_INNOSETUP="%PROGRAMFILES(X86)%\Inno Setup 5\iscc.exe"
SET           TOOLS_WINRAR="%PROGRAMFILES(X86)%\WinRAR\WinRAR.exe" "%PROGRAMFILES%\WinRAR\WinRAR.exe" "C:\Program Files\WinRAR\WinRAR.exe"
SET          TOOLS_ILMERGE="%PROGRAMFILES(x86)%\Microsoft\ILMerge\ILMerge.exe"

SET CERTIFICATE_THUMBPRINT="026184de8dbf52fdcbae75fd6b1a7d9ce4310e5d"
SET      SIGN_TIMESTAMPURL="http://timestamp.comodoca.com/rfc3161"


ECHO --- DISCOVER TOOLS
ECHO:

WHERE /Q git
IF ERRORLEVEL 1 (
    FOR %%I IN (%TOOLS_GIT%) DO (
        IF EXIST %%I IF NOT DEFINED TOOL_GIT SET TOOL_GIT=%%I
    )
) ELSE (
    SET TOOL_GIT="git"
)
IF [%TOOL_GIT%]==[] SET WARNING=1
ECHO Git .................: %TOOL_GIT%

FOR %%I IN (%TOOLS_VISUALSTUDIO%) DO (
    IF EXIST %%I IF NOT DEFINED TOOL_VISUALSTUDIO SET TOOL_VISUALSTUDIO=%%I
)
IF [%TOOL_VISUALSTUDIO%]==[] ECHO Visual Studio not found^^! & GOTO Error
ECHO Visual Studio .......: %TOOL_VISUALSTUDIO%

FOR %%I IN (%TOOLS_ILMERGE%) DO (
    IF EXIST %%I IF NOT DEFINED TOOL_ILMERGE SET TOOL_ILMERGE=%%I
)
IF [%TOOL_ILMERGE%]==[] SET WARNING=1
ECHO ILMerge .............: %TOOL_ILMERGE%

FOR %%I IN (%TOOLS_SIGNTOOL%) DO (
    IF EXIST %%I IF NOT DEFINED TOOL_SIGNTOOL SET TOOL_SIGNTOOL=%%I
)
IF [%TOOL_SIGNTOOL%]==[] SET WARNING=1
ECHO SignTool ............: %TOOL_SIGNTOOL%

FOR %%I IN (%TOOLS_INNOSETUP%) DO (
    IF EXIST %%I IF NOT DEFINED TOOL_INNOSETUP SET TOOL_INNOSETUP=%%I
)
IF [%TOOL_INNOSETUP%]==[] ECHO InnoSetup not found^^! & GOTO Error
ECHO InnoSetup ...........: %TOOL_INNOSETUP%

FOR %%I IN (%TOOLS_WINRAR%) DO (
    IF EXIST %%I IF NOT DEFINED TOOL_WINRAR SET TOOL_WINRAR=%%I
)
IF [%TOOL_WINRAR%]==[] SET WARNING=1
ECHO WinRAR ..............: %TOOL_WINRAR%

IF NOT [%CERTIFICATE_THUMBPRINT%]==[] (
    CERTUTIL -silent -verifystore -user My %CERTIFICATE_THUMBPRINT% > NUL
    IF NOT ERRORLEVEL 0 (
        SET CERTIFICATE_THUMBPRINT=
        SET WARNING=1
    )
)

ECHO:
ECHO:


IF NOT [%TOOL_GIT%]==[] (
    ECHO --- DISCOVER VERSION
    ECHO:

    FOR /F "delims=" %%I IN ('%TOOL_GIT% log -n 1 --format^=%%h') DO @SET VERSION_HASH=%%I%

    IF NOT [!VERSION_HASH!]==[] (
        FOR /F "delims=" %%I IN ('%TOOL_GIT% rev-list --count HEAD') DO @SET VERSION_NUMBER=%%I%
        %TOOL_GIT% diff --exit-code --quiet
        IF NOT ERRORLEVEL 0 SET VERSION_HASH=%VERSION_HASH%+
    )
    ECHO Hash ...: !VERSION_HASH!
    ECHO Revision: !VERSION_NUMBER!

    ECHO:
    ECHO:
)


ECHO --- BUILD SOLUTION
ECHO:

RMDIR /Q /S "..\Binaries" 2> NUL
%TOOL_VISUALSTUDIO% /Build "Release" %SOURCE_SOLUTION%
IF NOT ERRORLEVEL 0 ECHO Build failed^^! & GOTO Error

ECHO Build successful.

ECHO:
ECHO:


IF NOT [%TOOL_SIGNTOOL%]==[] IF NOT [%CERTIFICATE_THUMBPRINT%]==[] (
    ECHO --- SIGN EXECUTABLES
    ECHO:

    FOR %%I IN (%FILES_EXECUTABLE%) DO (
        SET SHOULD_SIGN=FALSE
        IF [%%~xI]==[.exe] SET SHOULD_SIGN=TRUE
        IF [%%~xI]==[.dll] SET SHOULD_SIGN=TRUE
        IF [!SHOULD_SIGN!]==[TRUE] (
            IF [%SIGN_TIMESTAMPURL%]==[] (
                %TOOL_SIGNTOOL% sign /s "My" /sha1 %CERTIFICATE_THUMBPRINT% /v %%I
            ) ELSE (
                %TOOL_SIGNTOOL% sign /s "My" /sha1 %CERTIFICATE_THUMBPRINT% /tr %SIGN_TIMESTAMPURL% /v %%I
            )
            IF NOT ERRORLEVEL 0 GOTO Error
        )
    )

    ECHO:
    ECHO:
)


RMDIR /Q /S ".\Temp" 2> NUL
MKDIR ".\Temp"


IF NOT [%TOOL_INNOSETUP%]==[] (
    ECHO --- BUILD SETUP
    ECHO:

    SET TOOL_INNOSETUP_ISPP=%TOOL_INNOSETUP:iscc.exe=ispp.dll%
    IF NOT EXIST !!TOOL_INNOSETUP_ISPP!! ECHO InnoSetup pre-processor not installed^^! & GOTO Error

    CALL %TOOL_INNOSETUP% /DVersionHash=%VERSION_HASH% /O".\Temp" %SOURCE_INNOSETUP%
    IF NOT ERRORLEVEL 0 GOTO Error

    FOR /F %%I IN ('DIR ".\Temp\*.exe" /B') DO SET SETUPEXE=%%I

    ECHO:
    ECHO:


    IF NOT [%TOOL_SIGNTOOL%]==[] IF NOT [%CERTIFICATE_THUMBPRINT%]==[] (
        ECHO --- SIGN SETUP
        ECHO:
        
        IF [%SIGN_TIMESTAMPURL%]==[] (
            %TOOL_SIGNTOOL% sign /s "My" /sha1 %CERTIFICATE_THUMBPRINT% /v ".\Temp\!SETUPEXE!"
        ) ELSE (
            %TOOL_SIGNTOOL% sign /s "My" /sha1 %CERTIFICATE_THUMBPRINT% /tr %SIGN_TIMESTAMPURL% /v ".\Temp\!SETUPEXE!"
        )
        IF NOT ERRORLEVEL 0 GOTO Error

        ECHO:
        ECHO:
    )
) ELSE (
    FOR %%I IN (%FILES%) DO (
        IF [%%~xI]==[.exe] (
            IF NOT DEFINED SETUPEXE SET SETUPEXE=%%~nI000%%~xI
        )
    )
)


IF NOT [%TOOL_ILMERGE%]==[] (
    ECHO --- MERGE ASSEMBLIES
    ECHO:
    
    %TOOL_ILMERGE% /targetplatform:v4 /keyfile:..\Source\QText\Properties\App.snk /out:..\Binaries\QTextPortable.exe %FILES_EXECUTABLE%
    IF ERRORLEVEL 1 PAUSE && EXIT /B %ERRORLEVEL%

    ECHO Completed.

    ECHO:
    ECHO:

    
    CERTUTIL -silent -verifystore -user My %CERTIFICATE_THUMBPRINT% > NUL
    IF %ERRORLEVEL%==0 (
        ECHO --- RESIGN SOLUTION
        ECHO:

        IF [%SIGN_TIMESTAMPURL%]==[] (
            %TOOL_SIGNTOOL% sign /s "My" /sha1 %CERTIFICATE_THUMBPRINT% /v ..\Binaries\QTextPortable.exe
        ) ELSE (
            %TOOL_SIGNTOOL% sign /s "My" /sha1 %CERTIFICATE_THUMBPRINT% /tr %SIGN_TIMESTAMPURL% /v ..\Binaries\QTextPortable.exe
        )
        IF NOT ERRORLEVEL 0 GOTO Error

        ECHO:
        ECHO:
    )


    IF NOT [%TOOL_WINRAR%]==[] (
        ECHO --- BUILD ZIP
        ECHO:

        ECHO Zipping into !SETUPEXE:.exe=.zip!
        %TOOL_WINRAR% a -afzip -ep -m5 ".\Temp\!SETUPEXE:.exe=.zip!" ..\Binaries\QTextPortable.exe %FILES_OTHER%
        %TOOL_WINRAR% rn ".\Temp\!SETUPEXE:.exe=.zip!" QTextPortable.exe QText.exe
        %TOOL_WINRAR% rn ".\Temp\!SETUPEXE:.exe=.zip!" *.md *.txt
        IF NOT ERRORLEVEL 0 GOTO Error

        ECHO:
        ECHO:
    )
)


ECHO --- RELEASE
ECHO:

MKDIR "..\Releases" 2> NUL
FOR %%I IN (".\Temp\*.*") DO (
    SET FILE_FROM=%%~nI%%~xI
    IF NOT [%VERSION_HASH%]==[] (
        SET FILE_TO=!FILE_FROM:000=-rev%VERSION_NUMBER%-%VERSION_HASH%!
    ) ELSE (
        SET FILE_TO=!FILE_FROM!
    )
    MOVE ".\Temp\!FILE_FROM!" "..\Releases\!FILE_TO!" > NUL
    IF NOT ERRORLEVEL 0 GOTO Error
    ECHO !FILE_TO!
    IF NOT DEFINED FILE_RELEASED SET FILE_RELEASED=!FILE_TO!
)

IF [%FILE_RELEASED%]==[] ECHO No files.

ECHO:
ECHO:


ECHO --- DONE

IF NOT [%WARNING%]==[] (    
    ECHO:
    IF [%TOOL_GIT%]==[] ECHO Git executable not found.
    IF [%TOOL_SIGNTOOL%]==[] ECHO SignTool executable not found.
    IF [%TOOL_INNOSETUP%]==[] ECHO InnoSetup executable not found.
    IF [%TOOL_WINRAR%]==[] ECHO WinRAR executable not found.
    IF [%TOOL_ILMERGE%]==[] ECHO ILMerge executable not found.

    IF [%CERTIFICATE_THUMBPRINT%]==[] ECHO Executables not signed.
    PAUSE
)

IF NOT [%FILE_RELEASED%]==[] explorer /select,"..\Releases\!FILE_RELEASED!"

ENDLOCAL
RMDIR /Q /S ".\Temp" 2> NUL
EXIT /B 0


:Error
ENDLOCAL
RMDIR /Q /S ".\Temp" 2> NUL
PAUSE
EXIT /B 1
