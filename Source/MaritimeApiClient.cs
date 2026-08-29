using GlobalIntelligence.Models;

namespace GlobalIntelligence.Services.ApiClients;

public interface IMaritimeApiClient
{
    Task<List<ShipData>> GetAllShipsAsync();
    Task<List<ShipData>> GetShipsInRegionAsync(double lat, double lon, double radiusKm);
    Task<ShipData?> GetShipDetailsAsync(int mmsi);
}

public class MaritimeApiClient : IMaritimeApiClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.marinetraffic.com";

    public MaritimeApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<ShipData>> GetAllShipsAsync() => new List<ShipData>();
    public async Task<List<ShipData>> GetShipsInRegionAsync(double lat, double lon, double radiusKm) => new List<ShipData>();
    public async Task<ShipData?> GetShipDetailsAsync(int mmsi) => null;
}
