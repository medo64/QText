#define AppName        GetStringFileInfo('..\..\bin\QText.exe', 'ProductName')
#define AppVersion     GetStringFileInfo('..\..\bin\QText.exe', 'ProductVersion')
#define AppFileVersion GetStringFileInfo('..\..\bin\QText.exe', 'FileVersion')
#define AppCompany     GetStringFileInfo('..\..\bin\QText.exe', 'CompanyName')
#define AppCopyright   GetStringFileInfo('..\..\bin\QText.exe', 'LegalCopyright')
#define AppBase        LowerCase(StringChange(AppName, ' ', ''))
#define AppVersion3    Copy(AppVersion, 1, RPos('.', AppVersion) - 1)
#define AppSetupFile   AppBase + '-' + AppVersion3


[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppCompany}
AppPublisherURL=https://www.medo64.com/{#AppBase}/
AppCopyright={#AppCopyright}
VersionInfoProductVersion={#AppVersion}
VersionInfoProductTextVersion={#AppVersion3}
VersionInfoVersion={#AppFileVersion}
DefaultDirName={pf}\{#AppCompany}\{#AppName}
OutputBaseFilename={#AppSetupFile}
SourceDir=..\..\bin
OutputDir=..\dist
AppId=JosipMedved_QText
CloseApplications="yes"
RestartApplications="no"
AppMutex=Global\JosipMedved_QText
UninstallDisplayIcon={app}\QText.exe
AlwaysShowComponentsList=no
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=yes
MergeDuplicateFiles=yes
MinVersion=0,6.0
PrivilegesRequired=admin
ShowLanguageDialog=no
SolidCompression=yes
ChangesAssociations=no
DisableWelcomePage=yes
LicenseFile=../package/win/License.rtf


[Messages]
SetupAppTitle=Setup {#AppName} {#AppVersion3}
SetupWindowTitle=Setup {#AppName} {#AppVersion3}
BeveledLabel=medo64.com

[Dirs]
Name: "{userappdata}\Josip Medved\QText";  Flags: uninsalwaysuninstall

[Files]
Source: "QText.exe";                                DestDir: "{app}";  Flags: ignoreversion;
Source: "libgcc_s_seh-1.dll";                       DestDir: "{app}";  Flags: ignoreversion;
Source: "libstdc++-6.dll";                          DestDir: "{app}";  Flags: ignoreversion;
Source: "libstdc++-6.dll";                          DestDir: "{app}";  Flags: ignoreversion;
Source: "libwinpthread-1.dll";                      DestDir: "{app}";  Flags: ignoreversion;
Source: "Qt5Core.dll";                              DestDir: "{app}";  Flags: ignoreversion;
Source: "Qt5Gui.dll";                               DestDir: "{app}";  Flags: ignoreversion;
Source: "Qt5Network.dll";                           DestDir: "{app}";  Flags: ignoreversion;
Source: "Qt5Widgets.dll";                           DestDir: "{app}";  Flags: ignoreversion;
Source: "..\README.md";   DestName: "README.txt";   DestDir: "{app}";  Flags: overwritereadonly uninsremovereadonly;  Attribs: readonly;
Source: "..\LICENSE.md";  DestName: "LICENSE.txt";  DestDir: "{app}";  Flags: overwritereadonly uninsremovereadonly;  Attribs: readonly;

[Icons]
Name: "{userstartmenu}\QText";  Filename: "{app}\QText.exe"

[Registry]
Root: HKLM;  Subkey: "Software\Josip Medved\QText";                    ValueType: none;    ValueName: "Installed";                                           Flags: deletevalue uninsdeletevalue
Root: HKCU;  Subkey: "Software\Josip Medved\QText";                    ValueType: none;    ValueName: "Installed";                                           Flags: deletevalue uninsdeletekey
Root: HKCU;  Subkey: "Software\Josip Medved";                                                                                                                Flags: uninsdeletekeyifempty
Root: HKCU;  Subkey: "Software\Josip Medved\QText";                                                                                                          Flags: uninsdeletekey
Root: HKCU;  Subkey: "Software\Microsoft\Windows\CurrentVersion\Run";  ValueType: string;  ValueName: "QText";      ValueData: """{app}\QText.exe"" /hide";  Flags: uninsdeletevalue

[Run]
Description: "Launch application now";  Filename: "{app}\QText.exe";   Parameters: "/setup";  Flags: postinstall nowait skipifsilent runasoriginaluser shellexec
Description: "View ReadMe.txt";         Filename: "{app}\ReadMe.txt";                         Flags: postinstall nowait skipifsilent runasoriginaluser shellexec unchecked


[Code]

procedure InitializeWizard;
begin
  WizardForm.LicenseAcceptedRadio.Checked := True;
end;


function PrepareToInstall(var NeedsRestart: Boolean): String;
var
    ResultCode: Integer;
begin
    Exec(ExpandConstant('{sys}\taskkill.exe'), '/f /im qtext.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Result := Result;
end;
