using System.Diagnostics;
using System.IO;

namespace GoodbyeDpiTray;

/// <summary>
/// goodbyedpi.exe'yi gizli (pencere yok) baslatir/durdurur.
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
        // Kurulumda: <app>\goodbyedpi\x86_64\goodbyedpi.exe
        string[] candidates =
        {
            Path.Combine(baseDir, "goodbyedpi", "x86_64", "goodbyedpi.exe"),
            Path.Combine(baseDir, "goodbyedpi", "x86", "goodbyedpi.exe"),
            Path.Combine(baseDir, "x86_64", "goodbyedpi.exe"),
        };
        foreach (var c in candidates)
            if (File.Exists(c)) return c;
        return candidates[0]; // bulunamazsa ilk aday (hata mesaji icin)
    }

    public bool IsRunning()
    {
        if (_proc is { HasExited: false }) return true;
        var found = Process.GetProcessesByName("goodbyedpi");
        if (found.Length > 0)
        {
            _proc = found[0];
            return true;
        }
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
        foreach (var p in Process.GetProcessesByName("goodbyedpi"))
        {
            try { p.Kill(true); p.WaitForExit(2000); } catch { /* yok say */ }
        }
        _proc = null;
    }

    public bool Toggle()
    {
        if (IsRunning()) { Stop(); return false; }
        Start(); return true;
    }
}
