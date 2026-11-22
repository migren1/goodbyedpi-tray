using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;
using Size = System.Windows.Size;

namespace GoodbyeDpiTray;

public partial class FlyoutWindow : Window
{
    private bool _dark = true;
    private bool _on;

    /// <summary>Kullanici anahtara basti; App gercek ac/kapa yapar.</summary>
    public event Action? ToggleRequested;

    public FlyoutWindow()
    {
        InitializeComponent();
        TitleText.Text = "GoodbyeDPI";
        HintText.Text = "Simgeye çift tıkla hızlı aç/kapa";
        Deactivated += (_, _) => Hide();
        SourceInitialized += (_, _) => ApplyBackdrop();
    }

    private void ApplyBackdrop()
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        Dwm.Apply(hwnd, _dark);
    }

    public void ShowAt(bool on, bool dark)
    {
        _dark = dark;
        Render(on, animate: false);
        ApplyBackdrop();

        // Olcu hazir olsun
        Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var wa = SystemParameters.WorkArea;
        Left = wa.Right - Width - 12;
        Top = wa.Bottom - Height - 12;
        if (double.IsNaN(Width))
        {
            Left = wa.Right - 248;
            Top = wa.Bottom - 170;
        }

        Show();
        Activate();
        Topmost = true;
    }

    private void OnTrackClick(object sender, MouseButtonEventArgs e)
        => ToggleRequested?.Invoke();

    /// <summary>Durumu yansit (App ac/kapa yaptiktan sonra cagirir).</summary>
    public void Render(bool on, bool animate = true)
    {
        _on = on;

        // --- Renk paleti (monokrom, temaya gore) ---
        Color white = Color.FromRgb(0xFA, 0xFA, 0xFA);
        Color black = Color.FromRgb(0x09, 0x09, 0x0B);
        Color ink = _dark ? white : black;
        Color accentBg = _dark ? white : black;   // "acik" dolu yuzey
        Color accentFg = _dark ? black : white;    // dolu yuzey ustu yazi

        Panel.Background = new SolidColorBrush(
            _dark ? Color.FromArgb(0x1A, 0x00, 0x00, 0x00)
                  : Color.FromArgb(0x14, 0xFF, 0xFF, 0xFF));
        Panel.BorderThickness = new Thickness(1);
        Panel.BorderBrush = new SolidColorBrush(
            _dark ? Color.FromArgb(0x1F, 0xFF, 0xFF, 0xFF)
                  : Color.FromArgb(0x16, 0x00, 0x00, 0x00));

        TitleText.Foreground = new SolidColorBrush(ink);
        HintText.Foreground = new SolidColorBrush(Color.FromArgb(0x73, ink.R, ink.G, ink.B));

        // Rozet
        BadgeText.Text = on ? "Açık" : "Kapalı";
        if (on)
        {
            Badge.Background = new SolidColorBrush(accentBg);
            BadgeText.Foreground = new SolidColorBrush(accentFg);
        }
        else
        {
            Badge.Background = new SolidColorBrush(Color.FromArgb(0x1A, ink.R, ink.G, ink.B));
            BadgeText.Foreground = new SolidColorBrush(Color.FromArgb(0xB0, ink.R, ink.G, ink.B));
        }

        // Anahtar
        SwitchLabel.Text = on ? "Açık" : "Kapalı";
        if (on)
        {
            Track.Background = new SolidColorBrush(accentBg);
            Thumb.Background = new SolidColorBrush(accentFg);
            SwitchLabel.Foreground = new SolidColorBrush(accentFg);
        }
        else
        {
            Track.Background = new SolidColorBrush(Color.FromArgb(0x1F, ink.R, ink.G, ink.B));
            Thumb.Background = new SolidColorBrush(_dark ? Color.FromRgb(0xE4, 0xE4, 0xE7) : white);
            SwitchLabel.Foreground = new SolidColorBrush(Color.FromArgb(0xA6, ink.R, ink.G, ink.B));
        }

        // Thumb konumu (0 <-> 164)
        double target = on ? 164 : 0;
        if (animate)
        {
            var anim = new DoubleAnimation(target, TimeSpan.FromMilliseconds(220))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            ThumbShift.BeginAnimation(TranslateTransform.XProperty, anim);
        }
        else
        {
            ThumbShift.BeginAnimation(TranslateTransform.XProperty, null);
            ThumbShift.X = target;
        }
    }
}
