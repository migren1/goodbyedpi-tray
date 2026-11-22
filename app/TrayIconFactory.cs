using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace GoodbyeDpiTray;

/// <summary>
/// Gorev cubugu (tray) icin monokrom kalkan ikonu uretir.
/// Acik = dolu beyaz kalkan, Kapali = ince cizgi (soluk) kalkan.
/// </summary>
public static class TrayIconFactory
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr handle);

    public static Icon Create(bool on)
    {
        const int S = 32;
        using var bmp = new Bitmap(S, S);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = ShieldPath(S);

            if (on)
            {
                using var fill = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
                g.FillPath(fill, path);
            }
            else
            {
                using var pen = new Pen(Color.FromArgb(150, 255, 255, 255), 2.4f)
                {
                    LineJoin = LineJoin.Round
                };
                g.DrawPath(pen, path);
            }
        }

        IntPtr h = bmp.GetHicon();
        try { return (Icon)Icon.FromHandle(h).Clone(); }
        finally { DestroyIcon(h); }
    }

    private static GraphicsPath ShieldPath(int s)
    {
        // 24'luk tasarim orani s boyutuna olceklenir
        float f = s / 24f;
        PointF P(float x, float y) => new(x * f, y * f);

        var p = new GraphicsPath();
        // Ust kenar + yan + alt sivri uc (kalkan)
        PointF[] pts =
        {
            P(12, 2.4f),
            P(19.6f, 5.6f),
            P(19.6f, 11.6f),
            P(12, 21.6f),
            P(4.4f, 11.6f),
            P(4.4f, 5.6f),
        };
        p.AddPolygon(pts);
        p.CloseFigure();
        return p;
    }
}
