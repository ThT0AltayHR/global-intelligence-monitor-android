namespace GlobalIntelligence.Config;

public static class AppConstants
{
    public const string AppName = "Global Intelligence Monitor";
    public const string AppVersion = "1.0.0";
    
    public const string USGS_API = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary";
    public const string NewsAPI_Endpoint = "https://newsapi.org/v2";
    public const string Guardian_API = "https://open-platform.theguardian.com/search";
    
    public const int CacheDurationMinutes = 30;
    public const int MaxNotificationCount = 1000;
    public const int DatabaseRetentionDays = 30;
}
