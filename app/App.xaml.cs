using System.IO;
using System.Windows;
using WF = System.Windows.Forms;
using Drawing = System.Drawing;

namespace GoodbyeDpiTray;

public partial class App : System.Windows.Application
{
    private WF.NotifyIcon _tray = null!;
    private Drawing.Icon? _icon;
    private readonly DpiController _dpi = new();
    private FlyoutWindow? _flyout;

    private static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GoodbyeDPI", "gbdpi.log");

    internal static void Log(string s)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
            File.AppendAllText(LogPath, DateTime.Now.ToString("HH:mm:ss ") + s + "\r\n");
        }
        catch { /* yok say */ }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AppDomain.CurrentDomain.UnhandledException += (_, ev) => Log("UNHANDLED: " + ev.ExceptionObject);
        DispatcherUnhandledException += (_, ev) => { Log("DISPATCHER: " + ev.Exception); ev.Handled = true; };

        Log("=== baslangic === exe=" + _dpi.ExePath + " var=" + File.Exists(_dpi.ExePath));

        _tray = new WF.NotifyIcon { Visible = true, Text = "GoodbyeDPI" };

        var menu = new WF.ContextMenuStrip();
        var miToggle = new WF.ToolStripMenuItem("Aç / Kapat", null, (_, _) => Toggle());
        var miPanel = new WF.ToolStripMenuItem("Panel", null, (_, _) => ShowFlyout());
        var miExit = new WF.ToolStripMenuItem("Çıkış", null, (_, _) => ExitApp());
        menu.Items.Add(miToggle);
        menu.Items.Add(miPanel);
        menu.Items.Add(new WF.ToolStripSeparator());
        menu.Items.Add(miExit);
        menu.Opening += (_, _) => miToggle.Text = _dpi.IsRunning() ? "Kapat" : "Aç";
        _tray.ContextMenuStrip = menu;

        // Sol tik = PANEL (HUD) ac/kapa. Ac/kapa icin sag tik menu ya da paneldeki anahtar.
        _tray.MouseClick += (_, ev) =>
        {
            Log("MouseClick " + ev.Button);
            if (ev.Button == WF.MouseButtons.Left) ShowFlyout();
        };

        RefreshIcon();
        Log("hazir (tray gorunur)");
    }

    private void RefreshIcon()
    {
        bool on = _dpi.IsRunning();
        var newIcon = TrayIconFactory.Create(on);
        _tray.Icon = newIcon;
        _tray.Text = on ? "GoodbyeDPI: Açık" : "GoodbyeDPI: Kapalı";
        _icon?.Dispose();
        _icon = newIcon;
    }

    private void Toggle()
    {
        try
        {
            Log("Toggle basladi, suan calisiyor=" + _dpi.IsRunning());
            bool nowOn = _dpi.Toggle();
            RefreshIcon();
            _flyout?.Render(nowOn);
            Log("Toggle bitti, yeni durum=" + nowOn);
            _tray.ShowBalloonTip(1000, "GoodbyeDPI",
                nowOn ? "Açıldı" : "Kapatıldı", WF.ToolTipIcon.Info);
        }
        catch (Exception ex)
        {
            Log("Toggle HATA: " + ex);
            _tray.ShowBalloonTip(4000, "GoodbyeDPI", "Hata: " + ex.Message, WF.ToolTipIcon.Error);
        }
    }

    private bool Dark => Dwm.IsSystemDark();

    private void ShowFlyout()
    {
        try
        {
            int anchorX = WF.Cursor.Position.X; // tiklanan/imlec konumu (ikon hizasi)
            _flyout ??= CreateFlyout();
            if (_flyout.IsVisible) _flyout.Hide();
            else _flyout.ShowAt(_dpi.IsRunning(), Dark, anchorX);
        }
        catch (Exception ex) { Log("ShowFlyout HATA: " + ex); }
    }

    private FlyoutWindow CreateFlyout()
    {
        var f = new FlyoutWindow();
        f.ToggleRequested += Toggle;
        return f;
    }

    private void ExitApp()
    {
        try { _dpi.Stop(); } catch { /* yok say */ }
        _tray.Visible = false;
        _tray.Dispose();
        _icon?.Dispose();
        Shutdown();
    }
}
