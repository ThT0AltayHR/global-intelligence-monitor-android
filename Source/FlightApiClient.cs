using GlobalIntelligence.Models;

namespace GlobalIntelligence.Services.ApiClients;

public interface IFlightApiClient
{
    Task<List<AircraftData>> GetAllAircraftAsync();
    Task<List<AircraftData>> GetAircraftInRegionAsync(double lat, double lon, double radiusKm);
    Task<AircraftData?> GetAircraftDetailsAsync(string callsign);
    Task<List<FlightPath>> GetFlightHistoryAsync(string callsign);
}

public class FlightApiClient : IFlightApiClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.flightradar24.com";

    public FlightApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<AircraftData>> GetAllAircraftAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/aircraft");
            response.EnsureSuccessStatusCode();
            return new List<AircraftData>();
        }
        catch { return new List<AircraftData>(); }
    }

    public async Task<List<AircraftData>> GetAircraftInRegionAsync(double lat, double lon, double radiusKm)
    {
        return new List<AircraftData>();
    }

    public async Task<AircraftData?> GetAircraftDetailsAsync(string callsign)
    {
        return null;
    }

    public async Task<List<FlightPath>> GetFlightHistoryAsync(string callsign)
    {
        return new List<FlightPath>();
    }
}
