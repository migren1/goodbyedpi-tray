# GoodbyeDPI Tray

![License](https://img.shields.io/badge/license-MIT-green)
![Platform](https://img.shields.io/badge/platform-Windows%2010%20%2F%2011-0078D6)
![Release](https://img.shields.io/github/v/release/migren1/goodbyedpi-tray)
![Downloads](https://img.shields.io/github/downloads/migren1/goodbyedpi-tray/total)

GoodbyeDPI'yi **görev çubuğundan** (sistem tepsisi) tek tıkla aç/kapa yapan Windows uygulaması. Arka planda terminal/komut penceresi açılmaz; goodbyedpi gizli çalışır.

## ⬇️ İndir
**[➡️ GoodbyeDPI-Setup-1.0.0.exe indir](https://github.com/migren1/goodbyedpi-tray/releases/download/v1.0.0/GoodbyeDPI-Setup-1.0.0.exe)** &nbsp;·&nbsp; [tüm sürümler](https://github.com/migren1/goodbyedpi-tray/releases/latest)

İndir → çalıştır → kur. Tek dosya, tam offline (.NET + goodbyedpi gömülü). İlk açılışta bir kez UAC onayı ister (WinDivert sürücüsü için).

> İmzasız olduğu için Windows SmartScreen uyarısı çıkabilir: **Daha fazla bilgi → Yine de çalıştır**.

## Özellikler
- Win11 tarzı kalkan tray ikonu: dolu = açık, ince çizgi = kapalı
- Sol tık → ikon hizasında açılan akrilik HUD + büyük aç/kapa anahtarı; sağ tık → menü (Aç/Kapat, Panel, Çıkış)
- Koyu/açık tema (sistemi takip eder), açılışta goodbyedpi kapalı başlar
- "Programlar ekle/kaldır"da ikonuyla görünür, düzgün kaldırılır
- Preset: `-5 --dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253`

## Derleme
```
cd app
dotnet publish GoodbyeDPI.csproj -c Release -r win-x64 --self-contained true -p:SatelliteResourceLanguages=en -p:InvariantGlobalization=true
cd ../installer
ISCC.exe installer.iss
```

## Lisans
Tray/sarmalayıcı uygulama: **MIT** (bkz. [LICENSE](LICENSE)).
Pakete gömülü **GoodbyeDPI** (ValdikSS): **GPLv3** — lisans `app/payload/licenses/` içinde.
