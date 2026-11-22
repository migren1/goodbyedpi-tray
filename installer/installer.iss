; GoodbyeDPI Tray - Inno Setup kurulum script'i
; Derleme: ISCC.exe installer.iss  (once app Release derlenmis olmali)

#define AppName "GoodbyeDPI"
#define AppVersion "1.0.0"
#define AppPublisher "GoodbyeDPI Tray"
#define AppExe "GoodbyeDPI.exe"
; app self-contained publish ciktisi (.NET runtime GOMULU, offline)
#define BuildDir "..\app\bin\Release\net8.0-windows\win-x64\publish"

[Setup]
AppId={{B3F2B8E1-7C4A-4E2A-9A1D-5E2A7C4B9F01}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#AppExe}
OutputDir=.\out
OutputBaseFilename=GoodbyeDPI-Setup-{#AppVersion}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
; Program Files'a yazmak icin admin gerekir
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "tr"; MessagesFile: "compiler:Languages\Turkish.isl"

[Tasks]
Name: "desktopicon"; Description: "Masaüstüne kısayol"; GroupDescription: "Ek simgeler:"
Name: "startup"; Description: "Windows açılışında başlat"; GroupDescription: "Seçenekler:"; Flags: unchecked

[Files]
; Tum self-contained app ciktisini (exe + gomulu .NET runtime + goodbyedpi payload) kur
; Tam offline: hicbir indirme/baglanti yok.
Source: "{#BuildDir}\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"; Tasks: desktopicon
Name: "{userstartup}\{#AppName}"; Filename: "{app}\{#AppExe}"; Tasks: startup

[Run]
; Kurulum sonu: uygulamayi baslat
Filename: "{app}\{#AppExe}"; Description: "GoodbyeDPI'yi şimdi başlat"; Flags: nowait postinstall skipifsilent shellexec

[UninstallRun]
; Kaldirmadan once goodbyedpi'yi durdur
Filename: "taskkill.exe"; Parameters: "/f /im goodbyedpi.exe"; Flags: runhidden; RunOnceId: "StopGoodbyeDpi"
Filename: "taskkill.exe"; Parameters: "/f /im {#AppExe}"; Flags: runhidden; RunOnceId: "StopTray"
