namespace GlobalIntelligence.Services;

public interface IWeatherService
{
    Task<WeatherData?> GetWeatherAsync(double latitude, double longitude);
    Task<List<SevereWeatherAlert>> GetSevereWeatherAlertsAsync();
}

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<WeatherData?> GetWeatherAsync(double latitude, double longitude)
    {
        // OpenWeatherMap API integration
        return null;
    }

    public async Task<List<SevereWeatherAlert>> GetSevereWeatherAlertsAsync()
    {
        return new List<SevereWeatherAlert>();
    }
}
