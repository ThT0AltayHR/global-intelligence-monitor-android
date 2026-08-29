using System;
using Newtonsoft.Json;

namespace GlobalIntelligenceMonitor.Models;

/// <summary>
/// Deprem Veri Modeli
/// </summary>
public class EarthquakeData
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("time")]
    public DateTime Time { get; set; }

    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("depth")]
    public double Depth { get; set; } // km

    [JsonProperty("magnitude")]
    public double Magnitude { get; set; }

    [JsonProperty("place")]
    public string Place { get; set; } = "";

    [JsonProperty("country")]
    public string Country { get; set; } = "";

    [JsonProperty("source")]
    public string Source { get; set; } = "USGS"; // USGS, Kandilli, EMSC vb.

    [JsonProperty("felt_reports")]
    public int FeltReports { get; set; }

    [JsonProperty("tsunami_warning")]
    public bool TsunamiWarning { get; set; }

    [JsonProperty("severity_level")]
    public SeverityLevel SeverityLevel { get; set; }

    [JsonProperty("affected_cities")]
    public List<string> AffectedCities { get; set; } = new();

    [JsonProperty("radius_km")]
    public double RadiusKm { get; set; } // Etkilenen alan

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Uçak Takip Modeli - ADS-B verisi
/// </summary>
public class AircraftData
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("callsign")]
    public string Callsign { get; set; } = ""; // Uçuş numarası

    [JsonProperty("aircraft_type")]
    public string AircraftType { get; set; } = "";

    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("altitude")]
    public double Altitude { get; set; } // feet

    [JsonProperty("speed")]
    public double Speed { get; set; } // knots

    [JsonProperty("heading")]
    public double Heading { get; set; } // derece

    [JsonProperty("origin")]
    public string Origin { get; set; } = "";

    [JsonProperty("destination")]
    public string Destination { get; set; } = "";

    [JsonProperty("airline")]
    public string Airline { get; set; } = "";

    [JsonProperty("squawk")]
    public string Squawk { get; set; } = "";

    [JsonProperty("is_military")]
    public bool IsMilitary { get; set; }

    [JsonProperty("is_helicopter")]
    public bool IsHelicopter { get; set; }

    [JsonProperty("last_update")]
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

    [JsonProperty("signal_strength")]
    public int SignalStrength { get; set; } // dBm

    [JsonProperty("vertical_rate")]
    public double VerticalRate { get; set; } // feet/minute
}

/// <summary>
/// Gemi Takip Modeli - AIS verisi
/// </summary>
public class ShipData
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("mmsi")]
    public string MMSI { get; set; } = ""; // Maritime Mobile Service Identity

    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("callsign")]
    public string CallSign { get; set; } = "";

    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("speed")]
    public double Speed { get; set; } // knots

    [JsonProperty("heading")]
    public double Heading { get; set; } // derece

    [JsonProperty("ship_type")]
    public string ShipType { get; set; } = ""; // Gemi tipi

    [JsonProperty("flag")]
    public string Flag { get; set; } = ""; // Bayrak ülkesi

    [JsonProperty("destination")]
    public string Destination { get; set; } = "";

    [JsonProperty("draught")]
    public double Draught { get; set; } // metre

    [JsonProperty("imo")]
    public string IMO { get; set; } = "";

    [JsonProperty("vessel_size")]
    public VesselSize VesselSize { get; set; }

    [JsonProperty("is_military")]
    public bool IsMilitary { get; set; }

    [JsonProperty("last_port")]
    public string LastPort { get; set; } = "";

    [JsonProperty("last_update")]
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

    [JsonProperty("port_of_registry")]
    public string PortOfRegistry { get; set; } = "";
}

/// <summary>
/// Haber Modeli
/// </summary>
public class NewsItem
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("title")]
    public string Title { get; set; } = "";

    [JsonProperty("description")]
    public string Description { get; set; } = "";

    [JsonProperty("content")]
    public string Content { get; set; } = "";

    [JsonProperty("source")]
    public string Source { get; set; } = ""; // Haber kaynağı

    [JsonProperty("url")]
    public string Url { get; set; } = "";

    [JsonProperty("image_url")]
    public string ImageUrl { get; set; } = "";

    [JsonProperty("published_at")]
    public DateTime PublishedAt { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; } = "";

    [JsonProperty("category")]
    public NewsCategory Category { get; set; }

    [JsonProperty("priority")]
    public PriorityLevel Priority { get; set; }

    [JsonProperty("related_coordinates")]
    public GeoPoint? Location { get; set; }

    [JsonProperty("keywords")]
    public List<string> Keywords { get; set; } = new();

    [JsonProperty("is_breaking")]
    public bool IsBreaking { get; set; }

    [JsonProperty("language")]
    public string Language { get; set; } = "en";

    [JsonProperty("verified")]
    public bool Verified { get; set; }

    public DateTime CachedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Siber Tehdit Modeli
/// </summary>
public class CyberThreat
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("threat_type")]
    public CyberThreatType ThreatType { get; set; }

    [JsonProperty("severity")]
    public SeverityLevel Severity { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; } = "";

    [JsonProperty("affected_countries")]
    public List<string> AffectedCountries { get; set; } = new();

    [JsonProperty("affected_sectors")]
    public List<string> AffectedSectors { get; set; } = new();

    [JsonProperty("reported_by")]
    public string ReportedBy { get; set; } = "";

    [JsonProperty("source_ip")]
    public string? SourceIp { get; set; }

    [JsonProperty("source_country")]
    public string? SourceCountry { get; set; }

    [JsonProperty("cve_ids")]
    public List<string> CVEIds { get; set; } = new();

    [JsonProperty("detected_at")]
    public DateTime DetectedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("remediation")]
    public string Remediation { get; set; } = "";

    [JsonProperty("related_urls")]
    public List<string> RelatedUrls { get; set; } = new();
}

/// <summary>
/// Nükleer Tehdit Modeli
/// </summary>
public class NuclearThreat
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("type")]
    public NuclearEventType Type { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; } = "";

    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("facility_name")]
    public string FacilityName { get; set; } = "";

    [JsonProperty("facility_type")]
    public string FacilityType { get; set; } = ""; // Reaktör, Depo, Test Salanı vb.

    [JsonProperty("severity")]
    public SeverityLevel Severity { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; } = "";

    [JsonProperty("reported_at")]
    public DateTime ReportedAt { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = "";

    [JsonProperty("alert_radius_km")]
    public double AlertRadiusKm { get; set; }

    [JsonProperty("affected_population")]
    public int AffectedPopulation { get; set; }

    [JsonProperty("sources")]
    public List<string> Sources { get; set; } = new();
}

/// <summary>
/// Çatışma Bölgesi Modeli
/// </summary>
public class ConflictZone
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("countries")]
    public List<string> Countries { get; set; } = new();

    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("intensity")]
    public ConflictIntensity Intensity { get; set; }

    [JsonProperty("type")]
    public ConflictType Type { get; set; }

    [JsonProperty("start_date")]
    public DateTime StartDate { get; set; }

    [JsonProperty("casualties")]
    public int Casualties { get; set; }

    [JsonProperty("displaced_persons")]
    public int DisplacedPersons { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = "";

    [JsonProperty("involved_parties")]
    public List<string> InvolvedParties { get; set; } = new();

    [JsonProperty("recent_events")]
    public List<string> RecentEvents { get; set; } = new();

    [JsonProperty("humanitarian_crisis")]
    public bool HumanitarianCrisis { get; set; }

    [JsonProperty("last_update")]
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Enerji Kesintisi Modeli
/// </summary>
public class PowerOutage
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("country")]
    public string Country { get; set; } = "";

    [JsonProperty("region")]
    public string Region { get; set; } = "";

    [JsonProperty("affected_cities")]
    public List<string> AffectedCities { get; set; } = new();

    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("estimated_affected_people")]
    public long EstimatedAffectedPeople { get; set; }

    [JsonProperty("estimated_duration_hours")]
    public double EstimatedDurationHours { get; set; }

    [JsonProperty("cause")]
    public string Cause { get; set; } = "";

    [JsonProperty("reported_at")]
    public DateTime ReportedAt { get; set; }

    [JsonProperty("status")]
    public OutageStatus Status { get; set; }

    [JsonProperty("expected_restoration")]
    public DateTime? ExpectedRestoration { get; set; }

    [JsonProperty("critical_facilities_affected")]
    public List<string> CriticalFacilitiesAffected { get; set; } = new();
}

/// <summary>
/// Coğrafik Nokta
/// </summary>
public class GeoPoint
{
    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("altitude")]
    public double? Altitude { get; set; }
}

/// <summary>
/// Bildirim Modeli
/// </summary>
public class AlertNotification
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("type")]
    public AlertType Type { get; set; }

    [JsonProperty("severity")]
    public SeverityLevel Severity { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = "";

    [JsonProperty("message")]
    public string Message { get; set; } = "";

    [JsonProperty("location")]
    public GeoPoint? Location { get; set; }

    [JsonProperty("related_data_id")]
    public string? RelatedDataId { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("is_read")]
    public bool IsRead { get; set; }

    [JsonProperty("notification_channels")]
    public List<NotificationChannel> NotificationChannels { get; set; } = new();

    [JsonProperty("action_url")]
    public string? ActionUrl { get; set; }
}

// ==================== Enumlar ====================

public enum SeverityLevel
{
    Low = 0,
    Moderate = 1,
    High = 2,
    Critical = 3,
    Extreme = 4
}

public enum NewsCategory
{
    General = 0,
    Earthquake = 1,
    Conflict = 2,
    Cyber = 3,
    Nuclear = 4,
    Weather = 5,
    Military = 6,
    Markets = 7,
    Infrastructure = 8,
    Disaster = 9,
    Political = 10
}

public enum PriorityLevel
{
    Low = 0,
    Normal = 1,
    High = 2,
    Critical = 3,
    BreakingNews = 4
}

public enum CyberThreatType
{
    DDoS = 0,
    Malware = 1,
    Ransomware = 2,
    Phishing = 3,
    SQLInjection = 4,
    Vulnerability = 5,
    DataBreach = 6,
    Botnet = 7,
    ZeroDay = 8,
    APT = 9 // Advanced Persistent Threat
}

public enum NuclearEventType
{
    Accident = 0,
    Test = 1,
    Incident = 2,
    Inspection = 3,
    Emergency = 4,
    Warning = 5
}

public enum ConflictType
{
    Armed = 0,
    Civil = 1,
    Terrorism = 2,
    Insurgency = 3,
    Border = 4,
    Sectarian = 5,
    Ethnic = 6
}

public enum ConflictIntensity
{
    Low = 0,
    Moderate = 1,
    High = 2,
    Severe = 3,
    Critical = 4
}

public enum VesselSize
{
    Small = 0,
    Medium = 1,
    Large = 2,
    Supertanker = 3,
    Bulk = 4,
    Container = 5,
    Cargo = 6
}

public enum OutageStatus
{
    Ongoing = 0,
    Resolved = 1,
    Partial = 2,
    Investigating = 3
}

public enum AlertType
{
    Earthquake = 0,
    Weather = 1,
    Conflict = 2,
    Cyber = 3,
    Nuclear = 4,
    News = 5,
    Aviation = 6,
    Maritime = 7,
    Infrastructure = 8
}

public enum NotificationChannel
{
    InApp = 0,
    Push = 1,
    Email = 2,
    SMS = 3,
    Sound = 4,
    Vibration = 5
}
