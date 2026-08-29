using GlobalIntelligence.Models;

namespace GlobalIntelligence.Services;

public interface IRealtimeDataService
{
    event EventHandler<EarthquakeData>? EarthquakeUpdated;
    event EventHandler<NewsItem>? NewsUpdated;
    event EventHandler<CyberThreat>? ThreatUpdated;

    Task StartListeningAsync();
    Task StopListeningAsync();
    bool IsConnected { get; }
}

public class RealtimeDataService : IRealtimeDataService
{
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isConnected;

    public event EventHandler<EarthquakeData>? EarthquakeUpdated;
    public event EventHandler<NewsItem>? NewsUpdated;
    public event EventHandler<CyberThreat>? ThreatUpdated;

    public bool IsConnected => _isConnected;

    public async Task StartListeningAsync()
    {
        _isConnected = true;
        _cancellationTokenSource = new CancellationTokenSource();
        
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            await Task.Delay(5000);
        }
    }

    public async Task StopListeningAsync()
    {
        _isConnected = false;
        _cancellationTokenSource?.Cancel();
    }

    protected virtual void OnEarthquakeUpdated(EarthquakeData quake) =>
        EarthquakeUpdated?.Invoke(this, quake);
}
