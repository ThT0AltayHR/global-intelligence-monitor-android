using GlobalIntelligenceMonitor.Models;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Reactive.Subjects;
using Serilog;

namespace GlobalIntelligenceMonitor.Services;

/// <summary>
/// Siber Tehdit Servisi - CVE, DDoS, Malware vb. tehditler
/// </summary>
public interface ICyberThreatsService
{
    Task<List<CyberThreat>> GetActiveCyberThreatsAsync();
    Task<List<CyberThreat>> GetCyberThreatsByCountryAsync(string countryCode);
    Task<List<CyberThreat>> GetCyberThreatsBySeverityAsync(SeverityLevel severity);
    Task<List<CyberThreat>> GetCyberThreatsBySectorAsync(string sector);
    Task<CyberThreat?> GetCyberThreatDetailsAsync(string threatId);
    Task<List<CyberThreat>> GetCVEThreatsByIdAsync(string cveId);
    IObservable<CyberThreat> SubscribeToCyberThreats();
    Task<CyberSecurityStats> GetCyberSecurityStatsAsync(DateTime startDate, DateTime endDate);
}

public class CyberThreatsService : ICyberThreatsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDatabaseService _databaseService;
    private readonly INotificationService _notificationService;
    private readonly Subject<CyberThreat> _cyberThreatSubject = new();
    private readonly ILogger _logger = Log.ForContext<CyberThreatsService>();

    // API Endpoints
    private const string NVD_API = "https://services.nvd.nist.gov/rest/json/cves/2.0";
    private const string SHODAN_API = "https://api.shodan.io";
    private const string EXPLOIT_DB_API = "https://www.exploit-db.com/api";
    private const string ABUSE_IPDB_API = "https://api.abuseipdb.com/api/v2";

    public CyberThreatsService(
        IHttpClientFactory httpClientFactory,
        IDatabaseService databaseService,
        INotificationService notificationService)
    {
        _httpClientFactory = httpClientFactory;
        _databaseService = databaseService;
        _notificationService = notificationService;

        _ = StartCyberThreatMonitoringAsync();
    }

    public async Task<List<CyberThreat>> GetActiveCyberThreatsAsync()
    {
        try
        {
            var threats = new List<CyberThreat>();

            // NIST NVD'den son CVE'leri al
            var nvdThreats = await GetNVDVulnerabilitiesAsync();
            threats.AddRange(nvdThreats);

            // Shodan'dan aktif tehditleri al
            var shodanThreats = await GetShodanThreatsAsync();
            threats.AddRange(shodanThreats);

            // Abuse IPDB'den kötü niyetli IP'leri al
            var abusedIPs = await GetAbuseIPDBThreatsAsync();
            threats.AddRange(abusedIPs);

            // Duplikaları temizle
            threats = threats
                .DistinctBy(t => t.Id)
                .OrderByDescending(t => t.Severity)
                .ToList();

            // Veritabanına kaydet
            foreach (var threat in threats)
            {
                await _databaseService.SaveCyberThreatAsync(threat);
            }

            return threats;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting active cyber threats");
            return new List<CyberThreat>();
        }
    }

    public async Task<List<CyberThreat>> GetCyberThreatsByCountryAsync(string countryCode)
    {
        try
        {
            var threats = await GetActiveCyberThreatsAsync();

            return threats
                .Where(t => t.AffectedCountries.Contains(countryCode))
                .OrderByDescending(t => t.Severity)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting cyber threats by country");
            return new List<CyberThreat>();
        }
    }

    public async Task<List<CyberThreat>> GetCyberThreatsBySeverityAsync(SeverityLevel severity)
    {
        try
        {
            var threats = await GetActiveCyberThreatsAsync();

            return threats
                .Where(t => t.Severity == severity)
                .OrderByDescending(t => t.DetectedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting cyber threats by severity");
            return new List<CyberThreat>();
        }
    }

    public async Task<List<CyberThreat>> GetCyberThreatsBySectorAsync(string sector)
    {
        try
        {
            var threats = await GetActiveCyberThreatsAsync();

            return threats
                .Where(t => t.AffectedSectors.Contains(sector))
                .OrderByDescending(t => t.Severity)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting cyber threats by sector");
            return new List<CyberThreat>();
        }
    }

    public async Task<CyberThreat?> GetCyberThreatDetailsAsync(string threatId)
    {
        try
        {
            return await _databaseService.GetCyberThreatByIdAsync(threatId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting cyber threat details");
            return null;
        }
    }

    public async Task<List<CyberThreat>> GetCVEThreatsByIdAsync(string cveId)
    {
        try
        {
            var threats = await GetActiveCyberThreatsAsync();

            return threats
                .Where(t => t.CVEIds.Contains(cveId))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting CVE threats");
            return new List<CyberThreat>();
        }
    }

    public IObservable<CyberThreat> SubscribeToCyberThreats()
    {
        return _cyberThreatSubject.AsObservable();
    }

    public async Task<CyberSecurityStats> GetCyberSecurityStatsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var threats = await _databaseService.GetCyberThreatsByDateRangeAsync(startDate, endDate);

            return new CyberSecurityStats
            {
                TotalThreats = threats.Count,
                NewThreats = threats.Count(t => t.DetectedAt > DateTime.UtcNow.AddHours(-24)),
                ZeroDayVulnerabilities = threats.Count(t => t.ThreatType == CyberThreatType.ZeroDay),
                MostAffectedCountries = threats
                    .SelectMany(t => t.AffectedCountries)
                    .GroupBy(c => c)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => g.Key)
                    .ToList(),
                MostAffectedSectors = threats
                    .SelectMany(t => t.AffectedSectors)
                    .GroupBy(s => s)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => g.Key)
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error calculating cyber security stats");
            return new CyberSecurityStats();
        }
    }

    // ==================== Özel Metotlar ====================

    private async Task<List<CyberThreat>> GetNVDVulnerabilitiesAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{NVD_API}?recentlyPublished&pageSize=100");

            if (!response.IsSuccessStatusCode)
                return new List<CyberThreat>();

            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);
            var vulnerabilities = jObject["vulnerabilities"] as JArray;

            var threats = new List<CyberThreat>();

            if (vulnerabilities != null)
            {
                foreach (var vuln in vulnerabilities)
                {
                    var cveId = vuln["cve"]?["id"]?.ToString() ?? "";
                    var cvssScore = (double?)vuln["cve"]?["metrics"]?["cvssV3_1"]?["cvssV3_1Data"]?["baseScore"] ?? 0;

                    var threat = new CyberThreat
                    {
                        Id = cveId,
                        ThreatType = CyberThreatType.Vulnerability,
                        Description = vuln["cve"]?["descriptions"]?[0]?["value"]?.ToString() ?? "",
                        Severity = DetermineCyberSeverity(cvssScore),
                        DetectedAt = DateTime.Parse(vuln["cve"]?["published"]?.ToString() ?? DateTime.UtcNow.ToString()),
                        CVEIds = new List<string> { cveId },
                        ReportedBy = "NIST NVD"
                    };

                    threats.Add(threat);
                }
            }

            return threats;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting NVD vulnerabilities");
            return new List<CyberThreat>();
        }
    }

    private async Task<List<CyberThreat>> GetShodanThreatsAsync()
    {
        try
        {
            // Shodan.io API kullanımı gerçek API anahtarı ile yapılmalı
            // Burada mock veri döneceğiz
            await Task.Delay(100);

            return new List<CyberThreat>
            {
                new CyberThreat
                {
                    ThreatType = CyberThreatType.Vulnerability,
                    Severity = SeverityLevel.High,
                    Description = "Exposed database servers detected",
                    AffectedCountries = new List<string> { "US", "DE", "FR" },
                    AffectedSectors = new List<string> { "Technology", "Finance" },
                    ReportedBy = "Shodan",
                    DetectedAt = DateTime.UtcNow
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting Shodan threats");
            return new List<CyberThreat>();
        }
    }

    private async Task<List<CyberThreat>> GetAbuseIPDBThreatsAsync()
    {
        try
        {
            var client = _httpenticationFactory.CreateClient();
            var response = await client.GetAsync($"{ABUSE_IPDB_API}/check?maxAgeInDays=90");

            if (!response.IsSuccessStatusCode)
                return new List<CyberThreat>();

            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);
            var abuseData = jObject["data"];

            var threat = new CyberThreat
            {
                ThreatType = CyberThreatType.DDoS,
                Severity = SeverityLevel.High,
                Description = "Malicious IP detected",
                SourceIp = abuseData?["ipAddress"]?.ToString(),
                SourceCountry = abuseData?["countryCode"]?.ToString(),
                ReportedBy = "AbuseIPDB",
                DetectedAt = DateTime.UtcNow
            };

            return new List<CyberThreat> { threat };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting AbuseIPDB threats");
            return new List<CyberThreat>();
        }
    }

    private async Task StartCyberThreatMonitoringAsync()
    {
        var cts = new CancellationTokenSource();

        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                var threats = await GetActiveCyberThreatsAsync();

                foreach (var threat in threats.Where(t => t.Severity >= SeverityLevel.High))
                {
                    _cyberThreatSubject.OnNext(threat);

                    // Ciddi tehditlere bildirim gönder
                    await _notificationService.SendAlertAsync(new AlertNotification
                    {
                        Type = AlertType.Cyber,
                        Severity = threat.Severity,
                        Title = $"Cyber Threat: {threat.ThreatType}",
                        Message = threat.Description,
                        NotificationChannels = new List<NotificationChannel>
                        {
                            NotificationChannel.Push,
                            NotificationChannel.Sound
                        }
                    });
                }

                await Task.Delay(300000, cts.Token); // Her 5 dakikada bir
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in cyber threat monitoring");
                await Task.Delay(5000);
            }
        }
    }

    private SeverityLevel DetermineCyberSeverity(double cvssScore)
    {
        return cvssScore switch
        {
            >= 9.0 => SeverityLevel.Critical,
            >= 7.0 => SeverityLevel.High,
            >= 5.0 => SeverityLevel.Moderate,
            _ => SeverityLevel.Low
        };
    }
}

/// <summary>
/// Nükleer Tehdit Servisi
/// </summary>
public interface INuclearThreatsService
{
    Task<List<NuclearThreat>> GetAllNuclearThreatsAsync();
    Task<List<NuclearThreat>> GetNuclearThreatsByCountryAsync(string countryCode);
    Task<List<NuclearThreat>> GetNuclearFacilitiesAsync();
    Task<List<NuclearThreat>> GetNuclearTestSitesAsync();
    Task<NuclearThreat?> GetNuclearThreatDetailsAsync(string threatId);
    IObservable<NuclearThreat> SubscribeToNuclearThreats();
    Task<List<NuclearThreat>> GetNuclearThreatsInRadiusAsync(double latitude, double longitude, double radiusKm);
}

public class NuclearThreatsService : INuclearThreatsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDatabaseService _databaseService;
    private readonly Subject<NuclearThreat> _nuclearThreatSubject = new();
    private readonly ILogger _logger = Log.ForContext<NuclearThreatsService>();

    private readonly List<NuclearThreat> _knownFacilities = new()
    {
        new NuclearThreat
        {
            Country = "TR",
            FacilityName = "Akkuyu Nuclear Power Plant",
            Latitude = 36.1667,
            Longitude = 33.9167,
            FacilityType = "Nuclear Power Plant",
            AlertRadiusKm = 20
        },
        new NuclearThreat
        {
            Country = "JP",
            FacilityName = "Fukushima Daiichi",
            Latitude = 37.4204,
            Longitude = 141.0331,
            FacilityType = "Nuclear Power Plant",
            AlertRadiusKm = 30
        }
        // Daha fazla tesisi ekle
    };

    public NuclearThreatsService(
        IHttpClientFactory httpClientFactory,
        IDatabaseService databaseService)
    {
        _httpClientFactory = httpClientFactory;
        _databaseService = databaseService;

        _ = StartNuclearThreatMonitoringAsync();
    }

    public async Task<List<NuclearThreat>> GetAllNuclearThreatsAsync()
    {
        try
        {
            var threats = await _databaseService.GetNuclearThreatsAsync();
            return threats.OrderByDescending(t => t.Severity).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting all nuclear threats");
            return _knownFacilities;
        }
    }

    public async Task<List<NuclearThreat>> GetNuclearThreatsByCountryAsync(string countryCode)
    {
        try
        {
            var threats = await GetAllNuclearThreatsAsync();
            return threats.Where(t => t.Country == countryCode).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting nuclear threats by country");
            return new List<NuclearThreat>();
        }
    }

    public async Task<List<NuclearThreat>> GetNuclearFacilitiesAsync()
    {
        try
        {
            var threats = await GetAllNuclearThreatsAsync();
            return threats
                .Where(t => t.Type == NuclearEventType.Accident || 
                           t.FacilityType.Contains("Power Plant"))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting nuclear facilities");
            return new List<NuclearThreat>();
        }
    }

    public async Task<List<NuclearThreat>> GetNuclearTestSitesAsync()
    {
        try
        {
            var threats = await GetAllNuclearThreatsAsync();
            return threats.Where(t => t.Type == NuclearEventType.Test).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting nuclear test sites");
            return new List<NuclearThreat>();
        }
    }

    public async Task<NuclearThreat?> GetNuclearThreatDetailsAsync(string threatId)
    {
        try
        {
            var threats = await GetAllNuclearThreatsAsync();
            return threats.FirstOrDefault(t => t.Id == threatId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting nuclear threat details");
            return null;
        }
    }

    public IObservable<NuclearThreat> SubscribeToNuclearThreats()
    {
        return _nuclearThreatSubject.AsObservable();
    }

    public async Task<List<NuclearThreat>> GetNuclearThreatsInRadiusAsync(double latitude, double longitude, double radiusKm)
    {
        try
        {
            var threats = await GetAllNuclearThreatsAsync();

            return threats
                .Where(t => CalculateDistance(latitude, longitude, t.Latitude, t.Longitude) <= radiusKm)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting nuclear threats in radius");
            return new List<NuclearThreat>();
        }
    }

    private async Task StartNuclearThreatMonitoringAsync()
    {
        var cts = new CancellationTokenSource();

        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                var threats = await GetAllNuclearThreatsAsync();

                foreach (var threat in threats.Where(t => t.Severity >= SeverityLevel.Critical))
                {
                    _nuclearThreatSubject.OnNext(threat);
                }

                await Task.Delay(600000, cts.Token); // Her 10 dakikada bir
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in nuclear threat monitoring");
                await Task.Delay(5000);
            }
        }
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}

/// <summary>
/// Çatışma Bölgesi Servisi
/// </summary>
public interface IConflictZonesService
{
    Task<List<ConflictZone>> GetAllConflictZonesAsync();
    Task<List<ConflictZone>> GetActiveConflictsAsync();
    Task<List<ConflictZone>> GetConflictsByCountryAsync(string countryCode);
    Task<List<ConflictZone>> GetConflictsByIntensityAsync(ConflictIntensity intensity);
    Task<ConflictZone?> GetConflictDetailsAsync(string conflictId);
    IObservable<ConflictZone> SubscribeToConflictUpdates();
    Task<ConflictStatistics> GetConflictStatisticsAsync(DateTime startDate, DateTime endDate);
}

public class ConflictZonesService : IConflictZonesService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDatabaseService _databaseService;
    private readonly Subject<ConflictZone> _conflictZoneSubject = new();
    private readonly ILogger _logger = Log.ForContext<ConflictZonesService>();

    private const string ACLED_API = "https://api.acleddata.com";

    public ConflictZonesService(
        IHttpClientFactory httpClientFactory,
        IDatabaseService databaseService)
    {
        _httpClientFactory = httpClientFactory;
        _databaseService = databaseService;

        _ = StartConflictZoneMonitoringAsync();
    }

    public async Task<List<ConflictZone>> GetAllConflictZonesAsync()
    {
        try
        {
            return await _databaseService.GetAllConflictZonesAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting all conflict zones");
            return new List<ConflictZone>();
        }
    }

    public async Task<List<ConflictZone>> GetActiveConflictsAsync()
    {
        try
        {
            var zones = await GetAllConflictZonesAsync();
            return zones.Where(z => z.Status == "Active").ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting active conflicts");
            return new List<ConflictZone>();
        }
    }

    public async Task<List<ConflictZone>> GetConflictsByCountryAsync(string countryCode)
    {
        try
        {
            var zones = await GetAllConflictZonesAsync();
            return zones.Where(z => z.Countries.Contains(countryCode)).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting conflicts by country");
            return new List<ConflictZone>();
        }
    }

    public async Task<List<ConflictZone>> GetConflictsByIntensityAsync(ConflictIntensity intensity)
    {
        try
        {
            var zones = await GetAllConflictZonesAsync();
            return zones.Where(z => z.Intensity == intensity).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting conflicts by intensity");
            return new List<ConflictZone>();
        }
    }

    public async Task<ConflictZone?> GetConflictDetailsAsync(string conflictId)
    {
        try
        {
            var zones = await GetAllConflictZonesAsync();
            return zones.FirstOrDefault(z => z.Id == conflictId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting conflict details");
            return null;
        }
    }

    public IObservable<ConflictZone> SubscribeToConflictUpdates()
    {
        return _conflictZoneSubject.AsObservable();
    }

    public async Task<ConflictStatistics> GetConflictStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var zones = await _databaseService.GetConflictZonesByDateRangeAsync(startDate, endDate);

            return new ConflictStatistics
            {
                ActiveConflicts = zones.Count(z => z.Status == "Active"),
                TotalCasualties = zones.Sum(z => z.Casualties),
                DisplacedPersons = zones.Sum(z => z.DisplacedPersons),
                CriticalZones = zones
                    .Where(z => z.Intensity >= ConflictIntensity.High)
                    .Select(z => z.Name)
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error calculating conflict statistics");
            return new ConflictStatistics();
        }
    }

    private async Task StartConflictZoneMonitoringAsync()
    {
        var cts = new CancellationTokenSource();

        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                var zones = await GetActiveConflictsAsync();

                foreach (var zone in zones.Where(z => z.Intensity >= ConflictIntensity.Critical))
                {
                    _conflictZoneSubject.OnNext(zone);
                }

                await Task.Delay(900000, cts.Token); // Her 15 dakikada bir
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in conflict zone monitoring");
                await Task.Delay(5000);
            }
        }
    }
}
