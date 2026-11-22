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
    private WF.Timer? _clickTimer;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _tray = new WF.NotifyIcon
        {
            Visible = true,
            Text = "GoodbyeDPI"
        };

        var menu = new WF.ContextMenuStrip();
        var miToggle = new WF.ToolStripMenuItem("Aç / Kapat", null, (_, _) => Toggle());
        var miExit = new WF.ToolStripMenuItem("Çıkış", null, (_, _) => ExitApp());
        menu.Items.Add(miToggle);
        menu.Items.Add(new WF.ToolStripSeparator());
        menu.Items.Add(miExit);
        menu.Opening += (_, _) =>
            miToggle.Text = _dpi.IsRunning() ? "Kapat" : "Aç";
        _tray.ContextMenuStrip = menu;

        // Tek tik = flyout, cift tik = hizli ac/kapa
        _clickTimer = new WF.Timer { Interval = 250 };
        _clickTimer.Tick += (_, _) => { _clickTimer!.Stop(); ToggleFlyout(); };

        _tray.MouseClick += (_, ev) =>
        {
            if (ev.Button == WF.MouseButtons.Left)
            {
                if (_clickTimer!.Enabled) return; // cift tik bekliyor
                _clickTimer.Start();
            }
        };
        _tray.MouseDoubleClick += (_, ev) =>
        {
            if (ev.Button == WF.MouseButtons.Left)
            {
                _clickTimer!.Stop();
                Toggle();
            }
        };

        RefreshIcon();
    }

    private bool Dark => Dwm.IsSystemDark();

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
            bool nowOn = _dpi.Toggle();
            RefreshIcon();
            _flyout?.Render(nowOn);
            _tray.ShowBalloonTip(1200, "GoodbyeDPI",
                nowOn ? "Açıldı" : "Kapatıldı", WF.ToolTipIcon.Info);
        }
        catch (Exception ex)
        {
            _tray.ShowBalloonTip(3000, "GoodbyeDPI", "Hata: " + ex.Message, WF.ToolTipIcon.Error);
        }
    }

    private void ToggleFlyout()
    {
        _flyout ??= CreateFlyout();
        if (_flyout.IsVisible)
        {
            _flyout.Hide();
        }
        else
        {
            _flyout.ShowAt(_dpi.IsRunning(), Dark);
        }
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
