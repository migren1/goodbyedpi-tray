using System.Diagnostics;
using System.IO;

namespace GoodbyeDpiTray;

/// <summary>
/// goodbyedpi.exe'yi gizli (pencere yok) baslatir/durdurur.
/// Tespit ISIMLE DEGIL TAM YOLLA yapilir: tray exe'si de "GoodbyeDPI" adli
/// oldugundan isim-bazli arama (buyuk/kucuk harf duyarsiz) trayi backend saniyordu.
/// </summary>
public sealed class DpiController
{
    // 2_any_country_dnsredir preset'i
    private const string DpiArgs =
        "-5 --dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253";

    private Process? _proc;

    public string ExePath { get; }

    public DpiController()
    {
        ExePath = ResolveExe();
    }

    private static string ResolveExe()
    {
        string baseDir = AppContext.BaseDirectory;
        string[] candidates =
        {
            Path.Combine(baseDir, "goodbyedpi", "x86_64", "goodbyedpi.exe"),
            Path.Combine(baseDir, "goodbyedpi", "x86", "goodbyedpi.exe"),
            Path.Combine(baseDir, "x86_64", "goodbyedpi.exe"),
        };
        foreach (var c in candidates)
            if (File.Exists(c)) return c;
        return candidates[0];
    }

    /// <summary>Yalnizca GERCEK goodbyedpi backend'ini bulur (tam yol esitligi, kendini haric).</summary>
    private Process? FindBackend()
    {
        int self = Environment.ProcessId;
        foreach (var p in Process.GetProcessesByName("goodbyedpi"))
        {
            if (p.Id == self) continue;
            try
            {
                if (string.Equals(p.MainModule?.FileName, ExePath, StringComparison.OrdinalIgnoreCase))
                    return p;
            }
            catch
            {
                // MainModule okunamadi: adi "goodbyedpi" (tam kucuk) ise backend'dir;
                // tray exe'si "GoodbyeDPI" adli oldugundan bu ayirt eder.
                if (p.ProcessName == "goodbyedpi") return p;
            }
        }
        return null;
    }

    public bool IsRunning()
    {
        if (_proc is { HasExited: false }) return true;
        var p = FindBackend();
        if (p != null) { _proc = p; return true; }
        _proc = null;
        return false;
    }

    public void Start()
    {
        if (IsRunning()) return;
        if (!File.Exists(ExePath))
            throw new FileNotFoundException("goodbyedpi.exe bulunamadi", ExePath);

        var psi = new ProcessStartInfo
        {
            FileName = ExePath,
            Arguments = DpiArgs,
            WorkingDirectory = Path.GetDirectoryName(ExePath)!,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
        };
        _proc = Process.Start(psi);
    }

    public void Stop()
    {
        // Sadece gercek backend'i oldur (tray'e dokunma).
        Process? p;
        while ((p = FindBackend()) != null)
        {
            try { p.Kill(true); p.WaitForExit(2000); } catch { break; }
        }
        _proc = null;
    }

    public bool Toggle()
    {
        if (IsRunning()) { Stop(); return false; }
        Start(); return true;
    }
}
