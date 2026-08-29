using GlobalIntelligence.MVVM;
using System.Collections.ObjectModel;

namespace GlobalIntelligence.ViewModels;

public class MapViewViewModel : BaseViewModel
{
    private ObservableCollection<object> _mapMarkers;
    private double _mapCenterLatitude = 39.9334;
    private double _mapCenterLongitude = 32.8597;
    private float _mapZoom = 2.5f;
    private bool _showTraffic = false;
    private bool _showSatellite = false;
    private string _selectedMarkerType = "All";

    public ObservableCollection<object> MapMarkers
    {
        get => _mapMarkers ??= new();
        set => SetProperty(value, nameof(MapMarkers));
    }

    public double MapCenterLatitude { get => _mapCenterLatitude; set => SetProperty(value, nameof(MapCenterLatitude)); }
    public double MapCenterLongitude { get => _mapCenterLongitude; set => SetProperty(value, nameof(MapCenterLongitude)); }
    public float MapZoom { get => _mapZoom; set => SetProperty(value, nameof(MapZoom)); }
    public bool ShowTraffic { get => _showTraffic; set => SetProperty(value, nameof(ShowTraffic)); }
    public bool ShowSatellite { get => _showSatellite; set => SetProperty(value, nameof(ShowSatellite)); }
    public string SelectedMarkerType { get => _selectedMarkerType; set => SetProperty(value, nameof(SelectedMarkerType)); }

    public List<string> MarkerTypeOptions => new() { "All", "Earthquakes", "Threats", "News", "Events" };

    public MapViewViewModel()
    {
        Title = "Interactive Map";
    }

    public RelayCommand ZoomInCommand => new(() => MapZoom = Math.Min(MapZoom + 0.5f, 10f));
    public RelayCommand ZoomOutCommand => new(() => MapZoom = Math.Max(MapZoom - 0.5f, 1f));
    public RelayCommand ResetMapCommand => new(() =>
    {
        MapCenterLatitude = 39.9334;
        MapCenterLongitude = 32.8597;
        MapZoom = 2.5f;
    });
    public RelayCommand ToggleTrafficCommand => new(() => ShowTraffic = !ShowTraffic);
    public RelayCommand ToggleSatelliteCommand => new(() => ShowSatellite = !ShowSatellite);
}
