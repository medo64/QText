#define AppName        GetStringFileInfo('..\Binaries\QText.exe', 'ProductName')
#define AppVersion     GetStringFileInfo('..\Binaries\QText.exe', 'ProductVersion')
#define AppFileVersion GetStringFileInfo('..\Binaries\QText.exe', 'FileVersion')
#define AppCompany     GetStringFileInfo('..\Binaries\QText.exe', 'CompanyName')
#define AppCopyright   GetStringFileInfo('..\Binaries\QText.exe', 'LegalCopyright')
#define AppBase        LowerCase(StringChange(AppName, ' ', ''))
#define AppSetupFile   AppBase + StringChange(AppVersion, '.', '')

#define AppVersionEx   StringChange(AppVersion, '0.00', '')
#ifdef VersionHash
#  if "" != VersionHash
#    define AppVersionEx AppVersionEx + " (" + VersionHash + ")"
#  endif
#endif


[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppCompany}
AppPublisherURL=https://www.medo64.com/{#AppBase}/
AppCopyright={#AppCopyright}
VersionInfoProductVersion={#AppVersion}
VersionInfoProductTextVersion={#AppVersionEx}
VersionInfoVersion={#AppFileVersion}
DefaultDirName={pf}\{#AppCompany}\{#AppName}
OutputBaseFilename={#AppSetupFile}
OutputDir=..\Releases
SourceDir=..\Binaries
AppId=JosipMedved_QText
CloseApplications="yes"
RestartApplications="no"
AppMutex=Global\JosipMedved_QText
UninstallDisplayIcon={app}\QText.exe
AlwaysShowComponentsList=no
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=yes
MergeDuplicateFiles=yes
MinVersion=0,5.1
PrivilegesRequired=admin
ShowLanguageDialog=no
SolidCompression=yes
ChangesAssociations=no
DisableWelcomePage=yes
LicenseFile=..\Setup\License.rtf


[Messages]
SetupAppTitle=Setup {#AppName} {#AppVersionEx}
SetupWindowTitle=Setup {#AppName} {#AppVersionEx}
BeveledLabel=medo64.com

[Dirs]
Name: "{userappdata}\Josip Medved\QText";  Flags: uninsalwaysuninstall

[Files]
Source: "QText.exe";           DestDir: "{app}";                            Flags: ignoreversion;
Source: "QText.pdb";           DestDir: "{app}";                            Flags: ignoreversion;
Source: "QText.Document.dll";  DestDir: "{app}";                            Flags: ignoreversion;
Source: "QText.Document.pdb";  DestDir: "{app}";                            Flags: ignoreversion;
Source: "..\README.md";        DestDir: "{app}";  DestName: "ReadMe.txt";   Flags: overwritereadonly uninsremovereadonly;  AfterInstall: AdjustTextFile;
Source: "..\LICENSE.md";       DestDir: "{app}";  DestName: "License.txt";  Flags: overwritereadonly uninsremovereadonly;  AfterInstall: AdjustTextFile;

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


function InitializeSetup(): Boolean;
var
  productKeys       :TArrayOfString;
  i                 :Integer;
  iProductKey       :String;
  iProductName      :String;
  iProductIcon      :String;
  iProductCodeLeft  :Integer;
  iProductCodeRight :Integer;
  iProductCode      :String;
  iResultCode       :Integer;
  isUninstalled     :Boolean;
begin
  Result := True;
  if RegGetSubkeyNames(HKEY_LOCAL_MACHINE, 'SOFTWARE\Classes\Installer\Products\', productKeys) then begin
    for i := 0 to GetArrayLength(productKeys) - 1 do begin
      iProductKey := 'SOFTWARE\Classes\Installer\Products\' + productKeys[i];
      if RegQueryStringValue(HKEY_LOCAL_MACHINE, iProductKey, 'ProductName', iProductName) then begin
        if (iProductName = 'QText') then begin
          if RegQueryStringValue(HKEY_LOCAL_MACHINE, iProductKey, 'ProductIcon', iProductIcon) then begin
            iProductCodeLeft := Pos('{', iProductIcon);
            iProductCodeRight := Pos('}', iProductIcon);
            if ((iProductCodeLeft > 0) and (iProductCodeRight > 0) and (iProductCodeRight > iProductCodeLeft)) then begin
              iProductCode := Copy(iProductIcon, iProductCodeLeft, iProductCodeRight - iProductCodeLeft + 1);
              if MsgBox('Setup has detected old version installed. In order to continue, old version will need to be uninstalled.' #13#10#13#10 'Uninstall old version?', mbConfirmation, MB_YESNO) = IDYES then begin
                isUninstalled := False;
                if Exec('msiexec.exe', '/uninstall ' + iProductCode + ' /passive', '', SW_SHOWNORMAL, ewWaitUntilTerminated, iResultCode) then begin
                  if not RegKeyExists(HKEY_LOCAL_MACHINE, iProductKey) then begin
                    isUninstalled := True;
                  end;
                end;
                if (isUninstalled) then begin
                  Result := True;
                end else begin
                  Result := MsgBox('Setup did not remove old version.' #13#10#13#10 'Do you wish to continue with installation without removing previous version?', mbError, MB_YESNO) = IDYES;
                end;
              end;
            end;
          end;
        end;
      end;
    end;
  end;
end;


function SetFileAttributes(lpFileName: string; dwFileAttributes: LongInt): Boolean;
  external 'SetFileAttributesA@kernel32.dll stdcall';


procedure AdjustTextFile();
var
  path : String;
  data : String;
begin
  path := ExpandConstant(CurrentFileName)
  LoadStringFromFile(path, data);
  StringChangeEx(data, #10, #13#10, True);
  SaveStringToFile(path, data, False);
  SetFileAttributes(path, 1);
end;
