using GlobalIntelligence.MVVM;
using System.Collections.ObjectModel;

namespace GlobalIntelligence.ViewModels;

public class GlobeViewModelFull : BaseViewModel
{
    private ObservableCollection<object> _earthquakes;
    private ObservableCollection<object> _aircraft;
    private ObservableCollection<object> _ships;
    private double _globeRotationX;
    private double _globeRotationY;
    private float _globeZoom = 1.0f;
    private bool _show3DView = true;
    private bool _showEarthquakesLayer = true;
    private bool _showFlightsLayer = false;
    private bool _showShipsLayer = false;
    private bool _showCyberLayer = false;
    private bool _showNuclearLayer = false;

    public ObservableCollection<object> Earthquakes => _earthquakes ??= new();
    public ObservableCollection<object> Aircraft => _aircraft ??= new();
    public ObservableCollection<object> Ships => _ships ??= new();

    public double GlobeRotationX { get => _globeRotationX; set => SetProperty(value, nameof(GlobeRotationX)); }
    public double GlobeRotationY { get => _globeRotationY; set => SetProperty(value, nameof(GlobeRotationY)); }
    public float GlobeZoom { get => _globeZoom; set => SetProperty(value, nameof(GlobeZoom)); }
    
    public bool Show3DView { get => _show3DView; set => SetProperty(value, nameof(Show3DView)); }
    public bool ShowEarthquakesLayer { get => _showEarthquakesLayer; set => SetProperty(value, nameof(ShowEarthquakesLayer)); }
    public bool ShowFlightsLayer { get => _showFlightsLayer; set => SetProperty(value, nameof(ShowFlightsLayer)); }
    public bool ShowShipsLayer { get => _showShipsLayer; set => SetProperty(value, nameof(ShowShipsLayer)); }
    public bool ShowCyberLayer { get => _showCyberLayer; set => SetProperty(value, nameof(ShowCyberLayer)); }
    public bool ShowNuclearLayer { get => _showNuclearLayer; set => SetProperty(value, nameof(ShowNuclearLayer)); }

    public GlobeViewModelFull()
    {
        Title = "3D Globe Monitor";
    }

    public RelayCommand RotateLeftCommand => new(() => GlobeRotationY -= 10);
    public RelayCommand RotateRightCommand => new(() => GlobeRotationY += 10);
    public RelayCommand ZoomInCommand => new(() => GlobeZoom = Math.Min(GlobeZoom + 0.2f, 3f));
    public RelayCommand ZoomOutCommand => new(() => GlobeZoom = Math.Max(GlobeZoom - 0.2f, 0.5f));
    public RelayCommand ToggleLayerCommand => new<string>(layer =>
    {
        switch(layer)
        {
            case "earthquake": ShowEarthquakesLayer = !ShowEarthquakesLayer; break;
            case "flights": ShowFlightsLayer = !ShowFlightsLayer; break;
            case "ships": ShowShipsLayer = !ShowShipsLayer; break;
            case "cyber": ShowCyberLayer = !ShowCyberLayer; break;
            case "nuclear": ShowNuclearLayer = !ShowNuclearLayer; break;
        }
    });

    public override async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            // Load all data
            await Task.Delay(500);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
