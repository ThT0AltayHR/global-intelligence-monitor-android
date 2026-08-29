using GlobalIntelligence.Services;
using GlobalIntelligence.Services.ApiClients;
using GlobalIntelligence.ViewModels;
using GlobalIntelligence.Views;
using GlobalIntelligence.MVVM;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalIntelligence.Config;

public static class DIExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        // Services
        services.AddSingleton<IEarthquakeService, EarthquakeService>();
        services.AddSingleton<INewsAggregatorService, NewsAggregatorService>();
        services.AddSingleton<ICyberThreatsService, CyberThreatsService>();
        services.AddSingleton<INuclearThreatsService, NuclearThreatsService>();
        services.AddSingleton<IConflictZonesService, ConflictZonesService>();
        services.AddSingleton<IWeatherService, WeatherService>();
        services.AddSingleton<IPowerOutageService, PowerOutageService>();
        services.AddSingleton<IFlightTrackingService, FlightTrackingService>();
        services.AddSingleton<IShipTrackingService, ShipTrackingService>();
        services.AddSingleton<IDataExportService, DataExportService>();

        // Infrastructure Services
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IGeoLocationService, GeoLocationService>();

        // API Clients
        services.AddHttpClient<IEarthquakeApiClient, EarthquakeApiClient>();
        services.AddHttpClient<INewsApiClient, NewsApiClient>();
        services.AddHttpClient<IFlightApiClient, FlightApiClient>();
        services.AddHttpClient<IMaritimeApiClient, MaritimeApiClient>();
        services.AddHttpClient<ICyberSecurityApiClient, CyberSecurityApiClient>();

        return services;
    }

    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<GlobeViewModelFull>();
        services.AddTransient<NewsViewModel>();
        services.AddTransient<AlertsViewModel>();
        services.AddTransient<CyberThreatsViewModel>();
        services.AddTransient<SettingsViewModelFull>();
        services.AddTransient<MapViewViewModel>();
        services.AddTransient<ChartsViewModelFull>();
        services.AddTransient<OnboardingViewModel>();
        services.AddTransient<DetailViewModel>();

        return services;
    }

    public static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddSingleton<OnboardingView>();
        services.AddSingleton<DashboardViewFull>();
        services.AddSingleton<GlobeViewXAML>();
        services.AddSingleton<MapViewXAML>();
        services.AddSingleton<ChartsViewXAML>();
        services.AddSingleton<AlertsViewFull>();
        services.AddSingleton<SettingsView>();
        services.AddSingleton<DetailViewXAML>();

        return services;
    }
}
