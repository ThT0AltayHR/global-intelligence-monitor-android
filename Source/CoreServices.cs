using GlobalIntelligenceMonitor.Models;
using Realm;
using Serilog;

namespace GlobalIntelligenceMonitor.Services;

/// <summary>
/// Veritabanı Servisi - Realm ORM kullanır
/// </summary>
public interface IDatabaseService
{
    Task SaveEarthquakeAsync(EarthquakeData earthquake);
    Task<EarthquakeData?> GetEarthquakeByIdAsync(string id);
    Task<List<EarthquakeData>> GetEarthquakesByDateRangeAsync(DateTime start, DateTime end);
    Task<List<EarthquakeData>> GetRecentEarthquakesAsync(int limit = 100);
    
    Task SaveNewsItemAsync(NewsItem news);
    Task<List<NewsItem>> GetNewsByCategoryAsync(NewsCategory category, int limit = 50);
    Task<List<NewsItem>> GetRecentNewsAsync(int limit = 50);
    
    Task SaveCyberThreatAsync(CyberThreat threat);
    Task<List<CyberThreat>> GetActiveCyberThreatsAsync();
    
    Task SaveAlertAsync(AlertNotification alert);
    Task<List<AlertNotification>> GetUnreadAlertsAsync();
    
    Task ClearOldDataAsync(int daysToKeep = 30);
}

public class DatabaseService : IDatabaseService
{
    private Realm? _realm;
    private readonly ILogger _logger = Log.ForContext<DatabaseService>();

    public DatabaseService()
    {
        InitializeRealm();
    }

    private void InitializeRealm()
    {
        try
        {
            var config = new RealmConfiguration
            {
                SchemaVersion = 1,
                IsReadonly = false
            };

            _realm = Realm.GetInstance(config);
            _logger.Information("Realm database initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error initializing Realm database");
        }
    }

    public async Task SaveEarthquakeAsync(EarthquakeData earthquake)
    {
        try
        {
            if (_realm == null) InitializeRealm();

            await _realm!.WriteAsync(() =>
            {
                _realm.Add(earthquake, update: true);
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving earthquake");
        }
    }

    public async Task<EarthquakeData?> GetEarthquakeByIdAsync(string id)
    {
        try
        {
            if (_realm == null) InitializeRealm();

            var result = await Task.Run(() =>
                _realm?.All<EarthquakeData>().FirstOrDefault(e => e.Id == id)
            );

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting earthquake by id");
            return null;
        }
    }

    public async Task<List<EarthquakeData>> GetEarthquakesByDateRangeAsync(DateTime start, DateTime end)
    {
        try
        {
            if (_realm == null) InitializeRealm();

            var result = await Task.Run(() =>
                _realm?.All<EarthquakeData>()
                    .Where(e => e.Time >= start && e.Time <= end)
                    .ToList()
            );

            return result ?? new List<EarthquakeData>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting earthquakes by date range");
            return new List<EarthquakeData>();
        }
    }

    public async Task<List<EarthquakeData>> GetRecentEarthquakesAsync(int limit = 100)
    {
        try
        {
            if (_realm == null) InitializeRealm();

            var result = await Task.Run(() =>
                _realm?.All<EarthquakeData>()
                    .OrderByDescending(e => e.Time)
                    .Take(limit)
                    .ToList()
            );

            return result ?? new List<EarthquakeData>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting recent earthquakes");
            return new List<EarthquakeData>();
        }
    }

    public async Task SaveNewsItemAsync(NewsItem news)
    {
        try
        {
            if (_realm == null) InitializeRealm();

            await _realm!.WriteAsync(() =>
            {
                _realm.Add(news, update: true);
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving news item");
        }
    }

    public async Task<List<NewsItem>> GetNewsByCategoryAsync(NewsCategory category, int limit = 50)
    {
        try
        {
            if (_realm == null) InitializeRealm();

            var result = await Task.Run(() =>
                _realm?.All<NewsItem>()
                    .Where(n => n.Category == category)
                    .OrderByDescending(n => n.PublishedAt)
                    .Take(limit)
                    .ToList()
            );

            return result ?? new List<NewsItem>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting news by category");
            return new List<NewsItem>();
        }
    }

    public async Task<List<NewsItem>> GetRecentNewsAsync(int limit = 50)
    {
        try
        {
            if (_realm == null) InitializeRealm();

            var result = await Task.Run(() =>
                _realm?.All<NewsItem>()
                    .OrderByDescending(n => n.PublishedAt)
                    .Take(limit)
                    .ToList()
            );

            return result ?? new List<NewsItem>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting recent news");
            return new List<NewsItem>();
        }
    }

    public async Task SaveCyberThreatAsync(CyberThreat threat)
    {
        try
        {
            if (_realm == null) InitializeRealm();

            await _realm!.WriteAsync(() =>
            {
                _realm.Add(threat, update: true);
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving cyber threat");
        }
    }

    public async Task<List<CyberThreat>> GetActiveCyberThreatsAsync()
    {
        try
        {
            if (_realm == null) InitializeRealm();

            var result = await Task.Run(() =>
                _realm?.All<CyberThreat>()
                    .Where(t => t.UpdatedAt > DateTime.UtcNow.AddHours(-24))
                    .OrderByDescending(t => t.Severity)
                    .ToList()
            );

            return result ?? new List<CyberThreat>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting active cyber threats");
            return new List<CyberThreat>();
        }
    }

    public async Task SaveAlertAsync(AlertNotification alert)
    {
        try
        {
            if (_realm == null) InitializeRealm();

            await _realm!.WriteAsync(() =>
            {
                _realm.Add(alert, update: true);
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving alert");
        }
    }

    public async Task<List<AlertNotification>> GetUnreadAlertsAsync()
    {
        try
        {
            if (_realm == null) InitializeRealm();

            var result = await Task.Run(() =>
                _realm?.All<AlertNotification>()
                    .Where(a => !a.IsRead)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList()
            );

            return result ?? new List<AlertNotification>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting unread alerts");
            return new List<AlertNotification>();
        }
    }

    public async Task ClearOldDataAsync(int daysToKeep = 30)
    {
        try
        {
            if (_realm == null) InitializeRealm();

            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);

            await _realm!.WriteAsync(() =>
            {
                var oldEarthquakes = _realm.All<EarthquakeData>()
                    .Where(e => e.Time < cutoffDate)
                    .ToList();

                foreach (var eq in oldEarthquakes)
                {
                    _realm.Remove(eq);
                }

                var oldNews = _realm.All<NewsItem>()
                    .Where(n => n.PublishedAt < cutoffDate)
                    .ToList();

                foreach (var news in oldNews)
                {
                    _realm.Remove(news);
                }
            });

            _logger.Information($"Cleared data older than {daysToKeep} days");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error clearing old data");
        }
    }
}

/// <summary>
/// Bildirim Servisi
/// </summary>
public interface INotificationService
{
    Task SendAlertAsync(AlertNotification alert);
    Task SendLocalNotificationAsync(string title, string message);
    Task SendPushNotificationAsync(string title, string message, Dictionary<string, string>? data = null);
}

public class NotificationService : INotificationService
{
    private readonly ILogger _logger = Log.ForContext<NotificationService>();

    public async Task SendAlertAsync(AlertNotification alert)
    {
        try
        {
            var tasks = new List<Task>();

            if (alert.NotificationChannels.Contains(NotificationChannel.Push))
            {
                tasks.Add(SendPushNotificationAsync(alert.Title, alert.Message));
            }

            if (alert.NotificationChannels.Contains(NotificationChannel.InApp))
            {
                // In-app notification
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Toast göster
                });
            }

            if (alert.NotificationChannels.Contains(NotificationChannel.Sound))
            {
                // Ses oynat
                PlayAlertSound();
            }

            if (alert.NotificationChannels.Contains(NotificationChannel.Vibration))
            {
                // Titreşim
                Vibration.Default.Vibrate(500);
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error sending alert");
        }
    }

    public async Task SendLocalNotificationAsync(string title, string message)
    {
        try
        {
            // Maui yerel bildirim
            await LocalNotificationRequest(new NotificationRequest
            {
                Title = title,
                Description = message,
                BadgeNumber = 1
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error sending local notification");
        }
    }

    public async Task SendPushNotificationAsync(string title, string message, Dictionary<string, string>? data = null)
    {
        try
        {
            // Firebase Cloud Messaging entegrasyonu burada yapılacak
            await Task.Delay(100); // Dummy
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error sending push notification");
        }
    }

    private void PlayAlertSound()
    {
        // Ses oynatma kodu
        try
        {
            // Platform-spesifik ses oynatma
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error playing alert sound");
        }
    }

    private async Task LocalNotificationRequest(NotificationRequest request)
    {
        try
        {
            // Maui bildirim gönder
            await NotificationCenter.Current.SendAsync(request);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error sending local notification request");
        }
    }

    private class NotificationRequest
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int BadgeNumber { get; set; }
    }
}

/// <summary>
/// Ayarlar Servisi
/// </summary>
public interface ISettingsService
{
    Task<T?> GetSetting<T>(string key, T? defaultValue = default);
    Task SaveSettingAsync<T>(string key, T value);
    Task<bool> GetBoolSetting(string key, bool defaultValue = false);
    Task SaveBoolSettingAsync(string key, bool value);
}

public class SettingsService : ISettingsService
{
    private readonly ILogger _logger = Log.ForContext<SettingsService>();

    public async Task<T?> GetSetting<T>(string key, T? defaultValue = default)
    {
        try
        {
            var value = Preferences.Get(key, null);
            
            if (value == null)
                return defaultValue;

            return JsonConvert.DeserializeObject<T>(value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting setting");
            return defaultValue;
        }
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        try
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            Preferences.Set(key, jsonValue);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving setting");
        }
    }

    public async Task<bool> GetBoolSetting(string key, bool defaultValue = false)
    {
        try
        {
            return Preferences.Get(key, defaultValue);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting bool setting");
            return defaultValue;
        }
    }

    public async Task SaveBoolSettingAsync(string key, bool value)
    {
        try
        {
            Preferences.Set(key, value);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving bool setting");
        }
    }
}

/// <summary>
/// Cache Servisi
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task ClearAsync();
}

public class CacheService : ICacheService
{
    private readonly Dictionary<string, (object Value, DateTime Expiration)> _cache = new();
    private readonly ILogger _logger = Log.ForContext<CacheService>();

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            if (_cache.TryGetValue(key, out var item))
            {
                if (item.Expiration > DateTime.UtcNow)
                {
                    return (T)item.Value;
                }
                else
                {
                    _cache.Remove(key);
                }
            }

            return default;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting cache");
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var exp = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromHours(1));
            _cache[key] = (value!, exp);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error setting cache");
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            _cache.Remove(key);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error removing cache");
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            _cache.Clear();
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error clearing cache");
        }
    }
}
