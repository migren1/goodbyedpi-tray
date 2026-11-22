# GoodbyeDPI - Gorev cubugu ac/kapa (tray)
# Kendini admin'e yukseltir, goodbyedpi'yi GIZLI (pencere yok) baslatir/durdurur.

# --- Admin'e yukselt ---
$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Start-Process -FilePath 'powershell.exe' -Verb RunAs -ArgumentList @(
        '-NoProfile','-ExecutionPolicy','Bypass','-WindowStyle','Hidden','-File',"`"$PSCommandPath`""
    )
    exit
}

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

# --- goodbyedpi yolu ve argumanlari (2_any_country_dnsredir preset) ---
$exe = 'C:\Users\user\Documents\goodbyedpi-0.2.3rc3-2\goodbyedpi-0.2.3rc3-2\x86_64\goodbyedpi.exe'
if (-not (Test-Path $exe)) {
    $exe = Join-Path (Split-Path -Parent $PSCommandPath) 'x86_64\goodbyedpi.exe'
}
$dpiArgs = '-5 --dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253'

$script:proc = $null

function Test-Running {
    if ($script:proc -and -not $script:proc.HasExited) { return $true }
    $p = Get-Process -Name goodbyedpi -ErrorAction SilentlyContinue
    if ($p) { $script:proc = $p[0]; return $true }
    return $false
}

function New-DpiIcon([bool]$on) {
    $bmp = New-Object System.Drawing.Bitmap 32,32
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    if ($on) { $col = [System.Drawing.Color]::FromArgb(40,200,120) }
    else     { $col = [System.Drawing.Color]::FromArgb(130,130,130) }
    $brush = New-Object System.Drawing.SolidBrush $col
    $pts = @(
        (New-Object System.Drawing.Point 16,2),
        (New-Object System.Drawing.Point 29,7),
        (New-Object System.Drawing.Point 29,16),
        (New-Object System.Drawing.Point 16,30),
        (New-Object System.Drawing.Point 3,16),
        (New-Object System.Drawing.Point 3,7)
    )
    $g.FillPolygon($brush,$pts)
    $pen = New-Object System.Drawing.Pen ([System.Drawing.Color]::White), 3
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap   = [System.Drawing.Drawing2D.LineCap]::Round
    if ($on) {
        $g.DrawLines($pen, @(
            (New-Object System.Drawing.Point 10,16),
            (New-Object System.Drawing.Point 14,21),
            (New-Object System.Drawing.Point 23,10)
        ))
    } else {
        $g.DrawLine($pen, 10,16,22,16)
    }
    $pen.Dispose(); $brush.Dispose(); $g.Dispose()
    $hicon = $bmp.GetHicon()
    $bmp.Dispose()
    return [System.Drawing.Icon]::FromHandle($hicon)
}

$notify = New-Object System.Windows.Forms.NotifyIcon
$menu   = New-Object System.Windows.Forms.ContextMenuStrip
$miToggle = $menu.Items.Add('Ac')
$miExit   = $menu.Items.Add('Cikis')
$notify.ContextMenuStrip = $menu
$notify.Visible = $true

function Update-Ui {
    $on = Test-Running
    $notify.Icon = New-DpiIcon $on
    if ($on) { $notify.Text = 'GoodbyeDPI: Acik'; $miToggle.Text = 'Kapat' }
    else     { $notify.Text = 'GoodbyeDPI: Kapali'; $miToggle.Text = 'Ac' }
}

function Start-Dpi {
    if (Test-Running) { return }
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = $exe
    $psi.Arguments = $dpiArgs
    $psi.WorkingDirectory = Split-Path -Parent $exe
    $psi.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Hidden
    $psi.CreateNoWindow = $true
    $psi.UseShellExecute = $false
    $script:proc = [System.Diagnostics.Process]::Start($psi)
    Start-Sleep -Milliseconds 400
    Update-Ui
    $notify.ShowBalloonTip(1200,'GoodbyeDPI','Acildi',[System.Windows.Forms.ToolTipIcon]::Info)
}

function Stop-Dpi {
    Get-Process -Name goodbyedpi -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    $script:proc = $null
    Update-Ui
    $notify.ShowBalloonTip(1200,'GoodbyeDPI','Kapatildi',[System.Windows.Forms.ToolTipIcon]::Info)
}

function Toggle-Dpi { if (Test-Running) { Stop-Dpi } else { Start-Dpi } }

$miToggle.add_Click({ Toggle-Dpi })
$notify.add_MouseClick({ param($s,$e)
    if ($e.Button -eq [System.Windows.Forms.MouseButtons]::Left) { Toggle-Dpi }
})
$miExit.add_Click({
    Stop-Dpi
    $notify.Visible = $false
    $notify.Dispose()
    [System.Windows.Forms.Application]::Exit()
})

Update-Ui
[System.Windows.Forms.Application]::Run()
