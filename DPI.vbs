' GoodbyeDPI tray baslatici - hicbir pencere/terminal gostermeden calistirir (0 = gizli)
Dim sh, here
Set sh = CreateObject("WScript.Shell")
here = Left(WScript.ScriptFullName, InStrRev(WScript.ScriptFullName, "\"))
sh.Run "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File """ & here & "dpi-tray.ps1""", 0, False
