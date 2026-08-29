using GlobalIntelligence.Models;

namespace GlobalIntelligence.Extensions;

public static class EarthquakeExtensions
{
    public static string GetSeverityEmoji(this EarthquakeData quake) =>
        quake.Magnitude switch
        {
            < 2.0 => "🟢",
            < 4.0 => "🟡",
            < 5.5 => "🟠",
            < 7.0 => "🔴",
            _ => "⚫"
        };

    public static string FormatLocation(this EarthquakeData quake) =>
        $"{quake.Latitude:F2}°N {quake.Longitude:F2}°E - {quake.Depth:F1}km";

    public static bool HasTsunamiPotential(this EarthquakeData quake) =>
        quake.Magnitude >= 7.0 && quake.Depth < 70;
}

public static class NewsExtensions
{
    public static string GetCategoryIcon(this NewsItem news) =>
        news.Category switch
        {
            NewsCategory.General => "📰",
            NewsCategory.Disaster => "🆘",
            NewsCategory.Conflict => "⚔️",
            NewsCategory.Tech => "💻",
            _ => "📌"
        };

    public static bool IsBreakingNews(this NewsItem news) =>
        news.IsBreaking && (DateTime.UtcNow - news.PublishedAt).TotalHours < 1;
}

public static class CyberThreatExtensions
{
    public static string GetThreatLevel(this CyberThreat threat) =>
        threat.Severity switch
        {
            SeverityLevel.Low => "🟢 Low",
            SeverityLevel.Moderate => "🟡 Moderate",
            SeverityLevel.High => "🟠 High",
            SeverityLevel.Critical => "🔴 Critical",
            SeverityLevel.Extreme => "⚫ Extreme",
            _ => "Unknown"
        };
}
