using GlobalIntelligenceMonitor.Models;

namespace GlobalIntelligenceMonitor.Services;

public interface IEarthquakeService
{
    /// <summary>
    /// Son depremleri getir
    /// </summary>
    Task<List<EarthquakeData>> GetRecentEarthquakesAsync(int hours = 24);

    /// <summary>
    /// Bölgesel depremleri getir
    /// </summary>
    Task<List<EarthquakeData>> GetEarthquakesByRegionAsync(double latitude, double longitude, double radiusKm);

    /// <summary>
    /// Ülkeye göre depremleri getir
    /// </summary>
    Task<List<EarthquakeData>> GetEarthquakesByCountryAsync(string countryCode);

    /// <summary>
    /// Minimum büyüklüğe göre filtrelenmiş depremleri getir
    /// </summary>
    Task<List<EarthquakeData>> GetEarthquakesByMagnitudeAsync(double minimumMagnitude);

    /// <summary>
    /// Belirli bir deprem hakkında detay getir
    /// </summary>
    Task<EarthquakeData?> GetEarthquakeDetailsAsync(string earthquakeId);

    /// <summary>
    /// Canlı deprem akışına abone ol
    /// </summary>
    IObservable<EarthquakeData> SubscribeToLiveEarthquakes();

    /// <summary>
    /// Belirli bir ülkeden tsunami uyarısı al
    /// </summary>
    Task<List<EarthquakeData>> GetTsunamiWarningsAsync(string countryCode);

    /// <summary>
    /// Deprem istatistikleri
    /// </summary>
    Task<EarthquakeStatistics> GetEarthquakeStatisticsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Ardışık sarsıntıları getir
    /// </summary>
    Task<List<EarthquakeData>> GetAftershecksAsync(string mainShockId);
}

public interface IFlightTrackingService
{
    Task<List<AircraftData>> GetAllAircraftAsync();
    Task<List<AircraftData>> GetAircraftByRegionAsync(double latitude, double longitude, double radiusKm);
    Task<AircraftData?> GetAircraftDetailsAsync(string callsign);
    Task<List<AircraftData>> GetAircraftByAirlineAsync(string airlineCode);
    Task<List<AircraftData>> GetMilitaryAircraftAsync();
    IObservable<AircraftData> SubscribeToFlightUpdates();
    Task<FlightPath> GetFlightHistoryAsync(string callsign, DateTime startTime, int hoursBack = 24);
}

public interface IShipTrackingService
{
    Task<List<ShipData>> GetAllShipsAsync();
    Task<List<ShipData>> GetShipsByRegionAsync(double latitude, double longitude, double radiusKm);
    Task<ShipData?> GetShipDetailsAsync(string mmsi);
    Task<List<ShipData>> GetShipsByFlagAsync(string countryCode);
    Task<List<ShipData>> GetMilitaryVesselsAsync();
    Task<List<ShipData>> GetShipsInPortAsync(string portCode);
    IObservable<ShipData> SubscribeToShipUpdates();
    Task<ShipPath> GetShipHistoryAsync(string mmsi, int hoursBack = 24);
    Task<List<string>> GetMaritimeChokePointsAsync();
}

public interface INewsAggregatorService
{
    Task<List<NewsItem>> GetLatestNewsAsync(int count = 50);
    Task<List<NewsItem>> GetNewsByCategoryAsync(NewsCategory category, int count = 20);
    Task<List<NewsItem>> GetNewsByCountryAsync(string countryCode, int count = 20);
    Task<List<NewsItem>> GetBreakingNewsAsync();
    Task<List<NewsItem>> GetNewsNearLocationAsync(double latitude, double longitude, double radiusKm);
    Task<NewsItem?> GetNewsDetailsAsync(string newsId);
    IObservable<NewsItem> SubscribeToBreakingNews();
    Task<List<string>> GetNewsSourcesToLocalCountryAsync();
}

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

public interface IWeatherService
{
    Task<WeatherData> GetWeatherAsync(double latitude, double longitude);
    Task<List<SevereWeatherAlert>> GetSevereWeatherAlertsAsync();
    Task<List<SevereWeatherAlert>> GetWeatherAlertsByCountryAsync(string countryCode);
    IObservable<WeatherData> SubscribeToWeatherUpdates();
}

public interface IPowerOutagesService
{
    Task<List<PowerOutage>> GetActivePowerOutagesAsync();
    Task<List<PowerOutage>> GetPowerOutagesByCountryAsync(string countryCode);
    Task<PowerOutage?> GetPowerOutageDetailsAsync(string outageId);
    IObservable<PowerOutage> SubscribeToPowerOutages();
}

// ==================== Yardımcı Modeller ====================

public class EarthquakeStatistics
{
    public int TotalEarthquakes { get; set; }
    public double AverageMagnitude { get; set; }
    public int MajorEarthquakes { get; set; } // >= 6.0
    public int TsunamiWarnings { get; set; }
    public List<string> MostAffectedCountries { get; set; } = new();
}

public class FlightPath
{
    public string Callsign { get; set; } = "";
    public List<GeoPoint> Path { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class ShipPath
{
    public string MMSI { get; set; } = "";
    public List<GeoPoint> Path { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class CyberSecurityStats
{
    public int TotalThreats { get; set; }
    public int NewThreats { get; set; }
    public int ZeroDayVulnerabilities { get; set; }
    public List<string> MostAffectedCountries { get; set; } = new();
    public List<string> MostAffectedSectors { get; set; } = new();
}

public class ConflictStatistics
{
    public int ActiveConflicts { get; set; }
    public int TotalCasualties { get; set; }
    public int DisplacedPersons { get; set; }
    public List<string> CriticalZones { get; set; } = new();
}

public class WeatherData
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Description { get; set; } = "";
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SevereWeatherAlert
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CountryCode { get; set; } = "";
    public string Region { get; set; } = "";
    public string AlertType { get; set; } = ""; // Fırtına, Tornado, Sel vb.
    public SeverityLevel Severity { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
    public string Description { get; set; } = "";
    public GeoPoint? Location { get; set; }
}
