using GlobalIntelligence.Models;

namespace GlobalIntelligence.Services;

public interface IPowerOutageService
{
    Task<List<PowerOutage>> GetActivePowerOutagesAsync();
    Task<List<PowerOutage>> GetPowerOutagesInRegionAsync(double lat, double lon, double radiusKm);
}

public class PowerOutageService : IPowerOutageService
{
    private readonly HttpClient _httpClient;

    public PowerOutageService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<PowerOutage>> GetActivePowerOutagesAsync()
    {
        return new List<PowerOutage>();
    }

    public async Task<List<PowerOutage>> GetPowerOutagesInRegionAsync(double lat, double lon, double radiusKm)
    {
        return new List<PowerOutage>();
    }
}
