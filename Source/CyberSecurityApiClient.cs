using GlobalIntelligence.Models;

namespace GlobalIntelligence.Services.ApiClients;

public interface ICyberSecurityApiClient
{
    Task<List<CyberThreat>> GetCVEsAsync(int limit);
    Task<List<CyberThreat>> GetMalwareAlertsAsync();
    Task<List<string>> GetBlacklistedIPsAsync();
}

public class CyberSecurityApiClient : ICyberSecurityApiClient
{
    private readonly HttpClient _httpClient;

    public CyberSecurityApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<CyberThreat>> GetCVEsAsync(int limit) => new List<CyberThreat>();
    public async Task<List<CyberThreat>> GetMalwareAlertsAsync() => new List<CyberThreat>();
    public async Task<List<string>> GetBlacklistedIPsAsync() => new List<string>();
}
