# Global Intelligence Monitor - Android OSINT & Real-Time Intelligence Dashboard

**Professional Intelligence Application** - Real-time global monitoring of earthquakes, cyber threats, nuclear facilities, conflicts, and news with 3D globe visualization.

## Özellikler

### 🌍 3D Dünya Küresi
- **Ultra HD Grafikleri**: SkiaSharp ile yapılmış yüksek kaliteli rendering
- **Etkileşimli Kontroller**: Kaydırma, yakınlaştırma, dönüş
- **Canlı Veri Katmanları**:
  - Depremler (USGS, Kandilli, EMSC)
  - Uçak Takibi (ADS-B)
  - Gemi Takibi (AIS)
  - Siber Tehditleri
  - Nükleer Tehditler
  - Çatışma Bölgeleri

### 📰 Çok Kaynaktan Haber Agregatörü
- **Otomatik Kaynak Taraması**: NewsAPI, Guardian, BBC, Reuters, yerel kaynaklar
- **Ülkeye Göre Haberler**: Kullanıcının konumuna göre yerel haberler
- **Kategorilere Göre Filtreleme**: Deprem, Çatışma, Siber, Nükleer, Hava Durumu
- **Canlı Breaking News**: Anlık güncellemeler
- **Kaynak Doğrulaması**: Güvenilir kaynaklar işaretleme

### 🚨 Tehdit İzleme Sistemleri

#### Depremler
- **Çoklu Kaynaklar**: USGS, Kandilli Rasathanesi, EMSC
- **Tsunami Uyarıları**: Otomatik tsunami tehdit tespiti
- **Bölgesel Analiz**: Etkilenen şehirler ve nüfus
- **Ardışık Sarsıntılar**: Öncü ve ardı sarsıntılar

#### Siber Tehditleri
- **CVE Takibi**: NIST NVD'den otomatik güncellemeler
- **DDoS Tespiti**: Aktif saldırı izleme
- **Malware Database**: Kötü amaçlı yazılım takibi
- **Zero-Day Vulnerabilities**: 0-day açıkların izlenmesi
- **Sektör Analizi**: Finansal, Sağlık, Enerjı vb. sektörlere göre

#### Nükleer Tehditleri
- **Tesis Haritası**: Tüm nükleer tesisler
- **Test Sitesi Takibi**: Nükleer test siteleri
- **Acil Durum Uyarıları**: Anlık tehdit bildirimleri

#### Çatışma Bölgeleri
- **Intensity Mapping**: Çatışma yoğunluğu haritası
- **İdari Bilgiler**: Kayıplar, yerinden edilenler, mülteciler
- **Tarih Analizi**: Çatışmanın gelişimi

### 📡 Gerçek Zamanlı İzleme
- **Canlı Uçak Akışı**: ADS-B verisi (FlightRadar24 vs.)
- **Gemi Konumlandırması**: AIS verisi (MarineTraffic vs.)
- **WebSocket Bağlantıları**: Anlık veri akışı
- **Bildirim Sistemi**: Push, Ses, Titreşim, SMS

### 🔍 OSINT Araçları
- **Sosyal Medya Taraması**: Twitter, Telegram, Reddit
- **Google Araması Entegrasyonu**: Google Custom Search
- **İP & Domain Araması**: GeoIP, WHOIS, ASN
- **Görüntü Ters Araması**: Google Reverse Image Search
- **Kaynak Bağlantısı**: Tüm haberler kaynak linklerine sahip

### ⚙️ Özelleştirme
- **Bildirim Tercihler**: Her tehdit tipi için ayrı ayarlar
- **Haber Filtreleri**: Kategori, Ülke, Dil seçimi
- **Arayüz Teması**: Koyu/Açık tema
- **Performans Ayarları**: Grafik kalitesi, güncelleme sıklığı
- **Veri Saklama**: Kaçak yedekleme, sürüm kontrol

## Mimari

### Teknoloji Yığını

```
┌─────────────────────────────────────┐
│   Presentation Layer (MAUI)         │
│  - GlobeView, NewsView, AlertsView  │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│   ViewModel Layer (MVVM)             │
│  - GlobeViewModel, NewsViewModel     │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│   Service Layer                      │
│  - EarthquakeService                │
│  - NewsAggregatorService            │
│  - CyberThreatsService              │
│  - ... (20+ Services)               │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│   Data Layer                         │
│  - Realm Database                   │
│  - Cache Service                    │
│  - REST API Clients                 │
└─────────────────────────────────────┘
                 │
┌────────────────▼────────────────────┐
│   External APIs                      │
│  - USGS, Kandilli, EMSC             │
│  - NewsAPI, Guardian, BBC           │
│  - NVD, Shodan, WHOIS               │
│  - FlightRadar, MarineTraffic       │
└─────────────────────────────────────┘
```

### Temel Sınıflar

```
Models/
├── EarthquakeData
├── AircraftData
├── ShipData
├── NewsItem
├── CyberThreat
├── NuclearThreat
├── ConflictZone
├── PowerOutage
├── AlertNotification
└── ... (30+ model)

Services/
├── IEarthquakeService
├── INewsAggregatorService
├── ICyberThreatsService
├── INuclearThreatsService
├── IConflictZonesService
├── IFlightTrackingService
├── IShipTrackingService
├── IGlobeRenderer
├── IDatabaseService
├── INotificationService
└── ... (20+ service interface)

ViewModels/
├── DashboardViewModel
├── GlobeViewModel
├── NewsViewModel
├── AlertsViewModel
├── SettingsViewModel
└── OsintToolViewModel

Views/
├── DashboardView
├── GlobeView
├── NewsView
├── AlertsView
├── SettingsView
└── OsintToolView
```

## Kurulum ve Derleme

### Gereksinimler

```
- .NET 8.0 SDK
- Android SDK (API 21+)
- Visual Studio 2022 veya VS Code
- Git
```

### 1. Proje Klonlama

```bash
git clone https://github.com/yourusername/GlobalIntelligenceMonitor.git
cd GlobalIntelligenceMonitor
```

### 2. API Anahtarlarını Ayarlama

`.env` dosyası oluşturun:

```env
# News APIs
NEWS_API_KEY=your_newsapi_key
GUARDIAN_API_KEY=your_guardian_key

# Cyber Security
NVD_API_KEY=your_nvd_key
SHODAN_API_KEY=your_shodan_key
ABUSEIPDB_API_KEY=your_abuseipdb_key

# Geolocation
GOOGLE_MAPS_API_KEY=your_google_maps_key

# Flight & Maritime
FLIGHTRADAR_API_KEY=your_flightradar_key
MARINETRAFFIC_API_KEY=your_marinetraffic_key

# Firebase (Push Notifications)
FIREBASE_PROJECT_ID=your_firebase_project
FIREBASE_API_KEY=your_firebase_key
```

### 3. Paketleri Yükleme

```bash
dotnet restore
```

### 4. Android Emülatör Hazırlama

```bash
# Emülatör listesini göster
emulator -list-avds

# Emülatörü başlat
emulator -avd Pixel_6_Pro
```

### 5. Uygulamayı Derleme ve Çalıştırma

#### Debug Modu
```bash
# VS Code ile
dotnet maui run -f net8.0-android

# Visual Studio ile
# Proje aç → Sağ tıkla → Debug → Android
```

#### Release APK Oluşturma
```bash
dotnet publish -f net8.0-android -c Release

# APK dosyası:
# bin/Release/net8.0-android/com.globalintelligence.monitor-Signed.apk
```

### 6. Android Cihaza Yükleme

```bash
# Cihazı bağlı tutarak
adb install -r bin/Release/net8.0-android/com.globalintelligence.monitor.apk
```

## GitHub Actions CI/CD Kurulumu

### `.github/workflows/android-build.yml` Oluşturun

```yaml
name: Build Android APK

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build APK
        run: |
          dotnet publish -f net8.0-android -c Release
          mv bin/Release/net8.0-android/com.globalintelligence.monitor-Signed.apk GlobalIntelligenceMonitor.apk

      - name: Upload APK Artifact
        uses: actions/upload-artifact@v3
        with:
          name: GlobalIntelligenceMonitor.apk
          path: GlobalIntelligenceMonitor.apk

      - name: Create Release
        uses: actions/create-release@v1
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        with:
          tag_name: v${{ github.run_number }}
          release_name: Build ${{ github.run_number }}
          draft: false
          prerelease: false
```

## API Entegrasyonları

### Deprem Kaynakları
- **USGS**: Dünya depremleri
- **Kandilli Rasathanesi**: Türkiye depremleri
- **EMSC**: Avrupa depremleri

### Haber Kaynakları
- **NewsAPI.org**: Dünya haberleri
- **The Guardian**: Gazeteci araştırması
- **BBC World**: İngiltçe haberler
- **Reuters**: Ajanstürk haberleri
- **Yerel Kaynaklar**: Her ülkenin kendi kaynakları

### Siber İstihbarat
- **NIST NVD**: CVE veritabanı
- **Shodan.io**: İnternet taraması
- **AbuseIPDB**: Kötü niyetli IP'ler
- **Exploit DB**: Exploit'ler

### Uçak & Gemi Takibi
- **FlightRadar24 API**: Uçak konumlandırması
- **MarineTraffic API**: Gemi konumlandırması

## Performans Optimizasyonları

### Bellek Kullanımı
- Realm ORM ile veritabanı bağlantısı havuzu
- LRU Cache implementasyonu
- Tuple Pooling (MAUI özelliği)

### Ağ Optimizasyonları
- HTTP/2 ve gzip sıkıştırması
- Request batching
- WebSocket canlı akışları
- CDN entegrasyonu

### UI Performansı
- SkiaSharp çoklu thread rendering
- Lazy loading katmanlar
- Virtual scrolling haberler
- Tile cache harita verisi

## Proje Yapısı

```
GlobalIntelligenceMonitor/
├── GlobalIntelligenceMonitor.csproj
├── MauiProgram.cs
├── App.xaml
├── Models/
│   ├── DataModels.cs
│   └── ViewModels.cs
├── Services/
│   ├── IEarthquakeService.cs
│   ├── EarthquakeService.cs
│   ├── NewsAndOsintServices.cs
│   ├── ThreatServices.cs
│   ├── GlobeRenderer.cs
│   ├── CoreServices.cs
│   └── ApiClients/
├── Views/
│   ├── DashboardView.xaml
│   ├── GlobeView.xaml
│   ├── NewsView.xaml
│   ├── AlertsView.xaml
│   └── SettingsView.xaml
├── Resources/
│   ├── Fonts/
│   ├── Icons/
│   └── Styles/
├── Platforms/
│   └── Android/
│       ├── AndroidManifest.xml
│       └── Resources/
└── .github/
    └── workflows/
        └── android-build.yml
```

## Android İzinleri

`AndroidManifest.xml` dosyasında şu izinler tanımlanmalıdır:

```xml
<!-- Network -->
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

<!-- Location -->
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />

<!-- Notifications -->
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />

<!-- Storage -->
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

<!-- Camera (OSINT araçları için)
<uses-permission android:name="android.permission.CAMERA" />

<!-- NFC (gelecek sürüm) -->
<uses-permission android:name="android.permission.NFC" />
```

## Geliştirme Yol Haritası

### v1.0 (Mevcut)
- ✅ 3D Dünya Küresi
- ✅ Deprem İzleme
- ✅ Haber Agregation
- ✅ Siber Tehdit İzleme
- ✅ Nükleer Tesisi Haritası

### v1.1 (Planlı)
- 📋 Uçak Takibi (ADS-B)
- 📋 Gemi Takibi (AIS)
- 📋 Sosyal Medya Taraması
- 📋 Öğrenme Geçmişi

### v1.2 (Planlı)
- 📋 Çatışma Analizi
- 📋 Ekonomik Veri
- 📋 Nüfus Çıkarımı
- 📋 Makine Öğrenmesi Tahminleri

### v2.0 (Uzun vadeli)
- 📋 AI Destekli Analiz
- 📋 Blockchain Doğrulama
- 📋 Gerçek Zamanlı Tercüme
- 📋 Sesli Komutlar

## Katkı Yönergeleri

1. Bu repoyu fork edin
2. Branch oluşturun (`git checkout -b feature/YeniÖzellik`)
3. Değişiklikleri commit edin (`git commit -m 'Yeni özellik ekle'`)
4. Branch'i push edin (`git push origin feature/YeniÖzellik`)
5. Pull Request açın

## Lisans

Bu proje MIT Lisansı altında lisanslanmıştır.

## İletişim & Destek

- **Issues**: GitHub Issues'i kullanın
- **Email**: support@globalintelligence.monitor
- **Twitter**: @GlobalIntelMon

## Güvenlik Uyarısı

Bu uygulama **DEMO** amaçlıdır. Gerçek istihbarat uygulamalarında şifreleme, VPN ve güvenlik denetimi eklenmesi gereklidir.

## Versiyon

**v0.1.0** - Alpha Sürümü
Yapım Tarihi: 2024
