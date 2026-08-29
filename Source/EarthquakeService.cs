using GlobalIntelligenceMonitor.Models;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Reactive.Subjects;
using Serilog;

namespace GlobalIntelligenceMonitor.Services;

/// <summary>
/// Deprem Servisi - Çoklu kaynaktan (USGS, Kandilli, EMSC) deprem verisi toplar
/// </summary>
public class EarthquakeService : IEarthquakeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDatabaseService _databaseService;
    private readonly INotificationService _notificationService;
    private readonly Subject<EarthquakeData> _earthquakeSubject = new();
    private CancellationTokenSource? _cancellationTokenSource;

    // API Endpoints
    private const string USGS_API = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary";
    private const string KANDILLI_API = "http://www.koeri.boun.edu.tr/scripts/lst8.asp";
    private const string EMSC_API = "https://www.seismicportal.eu/api/fdsn/event/query";

    private ILogger _logger => Log.ForContext<EarthquakeService>();

    public EarthquakeService(
        IHttpClientFactory httpClientFactory,
        IDatabaseService databaseService,
        INotificationService notificationService)
    {
        _httpClientFactory = httpClientFactory;
        _databaseService = databaseService;
        _notificationService = notificationService;

        // Canlı deprem akışını başlat
        _ = StartLiveEarthquakeStreamAsync();
    }

    public async Task<List<EarthquakeData>> GetRecentEarthquakesAsync(int hours = 24)
    {
        try
        {
            var earthquakes = new List<EarthquakeData>();

            // USGS'den verileri al
            var usgsEarthquakes = await GetUSGSEarthquakesAsync(hours);
            earthquakes.AddRange(usgsEarthquakes);

            // Kandilli Rasathanesi'nden Türkiye depremleri al
            var kandilliEarthquakes = await GetKandilliEarthquakesAsync(hours);
            earthquakes.AddRange(kandilliEarthquakes);

            // EMSC'den Avrupa depremleri al
            var emscEarthquakes = await GetEMSCEarthquakesAsync(hours);
            earthquakes.AddRange(emscEarthquakes);

            // Duplikaları temizle
            earthquakes = earthquakes
                .GroupBy(e => new { e.Latitude, e.Longitude, e.Magnitude })
                .Select(g => g.First())
                .OrderByDescending(e => e.Time)
                .ToList();

            // Veritabanına kaydet
            foreach (var eq in earthquakes)
            {
                await _databaseService.SaveEarthquakeAsync(eq);
            }

            return earthquakes;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching recent earthquakes");
            return new List<EarthquakeData>();
        }
    }

    public async Task<List<EarthquakeData>> GetEarthquakesByRegionAsync(double latitude, double longitude, double radiusKm)
    {
        try
        {
            var earthquakes = await GetRecentEarthquakesAsync();

            // Yarıçapa göre filtrele
            return earthquakes
                .Where(e => CalculateDistance(latitude, longitude, e.Latitude, e.Longitude) <= radiusKm)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching earthquakes by region");
            return new List<EarthquakeData>();
        }
    }

    public async Task<List<EarthquakeData>> GetEarthquakesByCountryAsync(string countryCode)
    {
        try
        {
            var earthquakes = await GetRecentEarthquakesAsync();

            return earthquakes
                .Where(e => e.Country.Equals(countryCode, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching earthquakes by country");
            return new List<EarthquakeData>();
        }
    }

    public async Task<List<EarthquakeData>> GetEarthquakesByMagnitudeAsync(double minimumMagnitude)
    {
        try
        {
            var earthquakes = await GetRecentEarthquakesAsync();

            return earthquakes
                .Where(e => e.Magnitude >= minimumMagnitude)
                .OrderByDescending(e => e.Magnitude)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching earthquakes by magnitude");
            return new List<EarthquakeData>();
        }
    }

    public async Task<EarthquakeData?> GetEarthquakeDetailsAsync(string earthquakeId)
    {
        try
        {
            return await _databaseService.GetEarthquakeByIdAsync(earthquakeId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching earthquake details");
            return null;
        }
    }

    public IObservable<EarthquakeData> SubscribeToLiveEarthquakes()
    {
        return _earthquakeSubject.AsObservable();
    }

    public async Task<List<EarthquakeData>> GetTsunamiWarningsAsync(string countryCode)
    {
        try
        {
            var earthquakes = await GetEarthquakesByCountryAsync(countryCode);

            return earthquakes
                .Where(e => e.TsunamiWarning)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching tsunami warnings");
            return new List<EarthquakeData>();
        }
    }

    public async Task<EarthquakeStatistics> GetEarthquakeStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var earthquakes = await _databaseService.GetEarthquakesByDateRangeAsync(startDate, endDate);

            var majorEarthquakes = earthquakes.Where(e => e.Magnitude >= 6.0).ToList();

            return new EarthquakeStatistics
            {
                TotalEarthquakes = earthquakes.Count,
                AverageMagnitude = earthquakes.Count > 0 ? earthquakes.Average(e => e.Magnitude) : 0,
                MajorEarthquakes = majorEarthquakes.Count,
                TsunamiWarnings = earthquakes.Count(e => e.TsunamiWarning),
                MostAffectedCountries = earthquakes
                    .GroupBy(e => e.Country)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => g.Key)
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error calculating earthquake statistics");
            return new EarthquakeStatistics();
        }
    }

    public async Task<List<EarthquakeData>> GetAftershecksAsync(string mainShockId)
    {
        try
        {
            var mainShock = await GetEarthquakeDetailsAsync(mainShockId);
            if (mainShock == null)
                return new List<EarthquakeData>();

            var aftershocks = await GetEarthquakesByRegionAsync(
                mainShock.Latitude,
                mainShock.Longitude,
                50); // 50 km yarıçap

            return aftershocks
                .Where(e => e.Time > mainShock.Time && e.Magnitude < mainShock.Magnitude)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching aftershocks");
            return new List<EarthquakeData>();
        }
    }

    // ==================== Özel USGS Metodu ====================

    private async Task<List<EarthquakeData>> GetUSGSEarthquakesAsync(int hours)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var endpoint = $"{USGS_API}/all_{hours}hour.geojson";

            var response = await client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
                return new List<EarthquakeData>();

            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);

            var earthquakes = new List<EarthquakeData>();
            var features = jObject["features"] as JArray;

            if (features != null)
            {
                foreach (var feature in features)
                {
                    var props = feature["properties"];
                    var coords = feature["geometry"]["coordinates"];

                    var earthquake = new EarthquakeData
                    {
                        Id = props?["ids"]?.ToString() ?? Guid.NewGuid().ToString(),
                        Time = UnixTimeStampToDateTime((long?)props?["time"] ?? 0),
                        Latitude = (double?)coords?[1] ?? 0,
                        Longitude = (double?)coords?[0] ?? 0,
                        Depth = (double?)coords?[2] ?? 0,
                        Magnitude = (double?)props?["mag"] ?? 0,
                        Place = props?["place"]?.ToString() ?? "",
                        Source = "USGS",
                        FeltReports = (int?)props?["felt"] ?? 0,
                        TsunamiWarning = props?["tsunami"]?.ToString() == "1"
                    };

                    earthquake.SeverityLevel = DetermineSeverity(earthquake.Magnitude);
                    earthquake.Country = ExtractCountryFromPlace(earthquake.Place);

                    earthquakes.Add(earthquake);
                }
            }

            return earthquakes;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching USGS earthquakes");
            return new List<EarthquakeData>();
        }
    }

    // ==================== Kandilli Rasathanesi Metodu ====================

    private async Task<List<EarthquakeData>> GetKandilliEarthquakesAsync(int hours)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(KANDILLI_API);

            if (!response.IsSuccessStatusCode)
                return new List<EarthquakeData>();

            var content = await response.Content.ReadAsStringAsync();
            var earthquakes = new List<EarthquakeData>();

            // Kandilli HTML'den verileri parse et
            var lines = content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains(","))
                    continue;

                try
                {
                    var parts = line.Split(',');
                    if (parts.Length < 7)
                        continue;

                    var earthquake = new EarthquakeData
                    {
                        Time = DateTime.Parse($"{parts[0]} {parts[1]}"),
                        Latitude = double.Parse(parts[2].Trim()),
                        Longitude = double.Parse(parts[3].Trim()),
                        Depth = double.Parse(parts[4].Trim()),
                        Magnitude = double.Parse(parts[5].Trim()),
                        Place = parts.Length > 7 ? parts[7] : "Turkey",
                        Country = "TR",
                        Source = "Kandilli"
                    };

                    earthquake.SeverityLevel = DetermineSeverity(earthquake.Magnitude);

                    if ((DateTime.UtcNow - earthquake.Time).TotalHours <= hours)
                    {
                        earthquakes.Add(earthquake);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex, "Error parsing Kandilli earthquake line");
                }
            }

            return earthquakes;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching Kandilli earthquakes");
            return new List<EarthquakeData>();
        }
    }

    // ==================== EMSC Metodu ====================

    private async Task<List<EarthquakeData>> GetEMSCEarthquakesAsync(int hours)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var startTime = DateTime.UtcNow.AddHours(-hours).ToString("yyyy-MM-ddTHH:mm:ss");
            var endpoint = $"{EMSC_API}?starttime={startTime}&minmagnitude=3.0&format=json";

            var response = await client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
                return new List<EarthquakeData>();

            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);

            var earthquakes = new List<EarthquakeData>();
            var features = jObject["features"] as JArray;

            if (features != null)
            {
                foreach (var feature in features)
                {
                    var props = feature["properties"];
                    var coords = feature["geometry"]["coordinates"];

                    var earthquake = new EarthquakeData
                    {
                        Time = DateTime.Parse(props?["origin_time"]?.ToString() ?? DateTime.UtcNow.ToString()),
                        Latitude = (double?)coords?[1] ?? 0,
                        Longitude = (double?)coords?[0] ?? 0,
                        Depth = (double?)coords?[2] ?? 0,
                        Magnitude = (double?)props?["magnitude"]?.ToString() ?? 0,
                        Place = props?["place_name"]?.ToString() ?? "",
                        Source = "EMSC"
                    };

                    earthquake.SeverityLevel = DetermineSeverity(earthquake.Magnitude);
                    earthquake.Country = ExtractCountryFromPlace(earthquake.Place);

                    earthquakes.Add(earthquake);
                }
            }

            return earthquakes;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching EMSC earthquakes");
            return new List<EarthquakeData>();
        }
    }

    // ==================== Canlı Akış ====================

    private async Task StartLiveEarthquakeStreamAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                var lastCheck = DateTime.UtcNow.AddMinutes(-1);
                var recentEarthquakes = await GetRecentEarthquakesAsync(1);

                foreach (var earthquake in recentEarthquakes)
                {
                    if (earthquake.Time > lastCheck)
                    {
                        _earthquakeSubject.OnNext(earthquake);

                        // Ciddi depremlere bildirim gönder
                        if (earthquake.Magnitude >= 5.0)
                        {
                            await _notificationService.SendAlertAsync(new AlertNotification
                            {
                                Type = AlertType.Earthquake,
                                Severity = earthquake.SeverityLevel,
                                Title = $"Earthquake Alert: {earthquake.Magnitude} M",
                                Message = $"{earthquake.Place} - Depth: {earthquake.Depth}km",
                                Location = new GeoPoint { Latitude = earthquake.Latitude, Longitude = earthquake.Longitude },
                                NotificationChannels = new List<NotificationChannel>
                                {
                                    NotificationChannel.Push,
                                    NotificationChannel.Sound,
                                    NotificationChannel.Vibration
                                }
                            });
                        }
                    }
                }

                // Her 30 saniyede bir kontrol et
                await Task.Delay(30000, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in live earthquake stream");
                await Task.Delay(5000);
            }
        }
    }

    // ==================== Yardımcı Metotlar ====================

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
            .AddMilliseconds(unixTimeStamp)
            .ToLocalTime();
    }

    private static SeverityLevel DetermineSeverity(double magnitude)
    {
        return magnitude switch
        {
            >= 8.0 => SeverityLevel.Extreme,
            >= 7.0 => SeverityLevel.Critical,
            >= 6.0 => SeverityLevel.High,
            >= 5.0 => SeverityLevel.Moderate,
            _ => SeverityLevel.Low
        };
    }

    private static string ExtractCountryFromPlace(string place)
    {
        // Basit ülke kodu çıkarma
        return place switch
        {
            { } p when p.Contains("Turkey") => "TR",
            { } p when p.Contains("Japan") => "JP",
            { } p when p.Contains("Chile") => "CL",
            { } p when p.Contains("Indonesia") => "ID",
            { } p when p.Contains("Mexico") => "MX",
            _ => "UN" // Unknown
        };
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Dünya yarıçapı (km)

        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }
}
