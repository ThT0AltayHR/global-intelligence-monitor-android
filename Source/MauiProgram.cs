using GlobalIntelligenceMonitor.Services;
using GlobalIntelligenceMonitor.Views;
using GlobalIntelligenceMonitor.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Serilog;
using Serilog.Core;

namespace GlobalIntelligenceMonitor;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Serilog Konfigürasyonu
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                Path.Combine(FileSystem.AppDataDirectory, "logs", "log-.txt"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("Roboto-Bold.ttf", "RobotoBold");
            })
            .ConfigureServices()
            .ConfigureViewModels()
            .ConfigureViews();

        return builder.Build();
    }

    private static MauiAppBuilder ConfigureServices(this MauiAppBuilder builder)
    {
        // Core Services
        builder.Services.AddSingleton<IEarthquakeService, EarthquakeService>();
        builder.Services.AddSingleton<IFlightTrackingService, FlightTrackingService>();
        builder.Services.AddSingleton<IShipTrackingService, ShipTrackingService>();
        builder.Services.AddSingleton<INewsAggregatorService, NewsAggregatorService>();
        builder.Services.AddSingleton<ICyberThreatsService, CyberThreatsService>();
        builder.Services.AddSingleton<INuclearThreatsService, NuclearThreatsService>();
        builder.Services.AddSingleton<IConflictZonesService, ConflictZonesService>();
        builder.Services.AddSingleton<IWeatherService, WeatherService>();
        builder.Services.AddSingleton<IPowerOutagesService, PowerOutagesService>();
        
        // OSINT Services
        builder.Services.AddSingleton<IOsintService, OsintService>();
        builder.Services.AddSingleton<INewsScraperService, NewsScraperService>();
        builder.Services.AddSingleton<IGeoLocationService, GeoLocationService>();
        
        // Real-time Services
        builder.Services.AddSingleton<IRealtimeDataService, RealtimeDataService>();
        builder.Services.AddSingleton<IWebSocketService, WebSocketService>();
        
        // Database Services
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<ICacheService, CacheService>();
        
        // Graphics & Rendering
        builder.Services.AddSingleton<IGlobeRenderer, GlobeRenderer>();
        builder.Services.AddSingleton<IMapRenderer, MapRenderer>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        
        // API Clients
        builder.Services.AddHttpClient<IEarthquakeApiClient, EarthquakeApiClient>();
        builder.Services.AddHttpClient<IAviationApiClient, AviationApiClient>();
        builder.Services.AddHttpClient<IMaritimeApiClient, MaritimeApiClient>();
        builder.Services.AddHttpClient<INewsApiClient, NewsApiClient>();
        builder.Services.AddHttpClient<ICyberSecurityApiClient, CyberSecurityApiClient>();

        return builder;
    }

    private static MauiAppBuilder ConfigureViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<GlobeViewModel>();
        builder.Services.AddTransient<EarthquakeViewModel>();
        builder.Services.AddTransient<FlightTrackingViewModel>();
        builder.Services.AddTransient<ShipTrackingViewModel>();
        builder.Services.AddTransient<NewsViewModel>();
        builder.Services.AddTransient<CyberThreatsViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<AlertsViewModel>();
        builder.Services.AddTransient<OsintToolViewModel>();

        return builder;
    }

    private static MauiAppBuilder ConfigureViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<DashboardView>();
        builder.Services.AddTransient<GlobeView>();
        builder.Services.AddTransient<EarthquakeView>();
        builder.Services.AddTransient<FlightTrackingView>();
        builder.Services.AddTransient<ShipTrackingView>();
        builder.Services.AddTransient<NewsView>();
        builder.Services.AddTransient<CyberThreatsView>();
        builder.Services.AddTransient<SettingsView>();
        builder.Services.AddTransient<AlertsView>();
        builder.Services.AddTransient<OsintToolView>();

        return builder;
    }
}
