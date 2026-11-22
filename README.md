# GoodbyeDPI Tray

GoodbyeDPI'yi **görev çubuğundan** (sistem tepsisi) tek tıkla aç/kapa yapan Windows uygulaması. Arka planda terminal/komut penceresi açılmaz; goodbyedpi gizli çalışır.

## Özellikler
- Win11 tarzı kalkan tray ikonu: dolu = açık, ince çizgi = kapalı
- Tıklayınca akrilik flyout + büyük aç/kapa anahtarı; çift tık = hızlı aç/kapa; sağ tık = menü
- Koyu/açık tema (sistemi takip eder)
- goodbyedpi + .NET runtime installer'a gömülü — **tam offline, tek dosya**
- Preset: `-5 --dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253`

## Kurulum
[Releases](../../releases) → `GoodbyeDPI-Setup-1.0.0.exe` indir ve çalıştır. İlk açılışta bir kez UAC onayı ister (goodbyedpi'nin WinDivert sürücüsü için).

> İmzasız olduğu için Windows SmartScreen uyarısı çıkabilir: "Daha fazla bilgi → Yine de çalıştır".

## Derleme
```
cd app
dotnet publish GoodbyeDPI.csproj -c Release -r win-x64 --self-contained true -p:SatelliteResourceLanguages=en -p:InvariantGlobalization=true
cd ../installer
ISCC.exe installer.iss
```

## Lisans
GoodbyeDPI (ValdikSS) GPLv3 — bkz. `app/payload/licenses/`. Bu tray sarmalayıcı aynı lisans altındadır.
