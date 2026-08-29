using GlobalIntelligence.MVVM;
using System.Collections.ObjectModel;

namespace GlobalIntelligence.ViewModels;

public class ChartDataPoint
{
    public string Label { get; set; } = "";
    public double Value { get; set; }
    public string Category { get; set; } = "";
}

public class ChartsViewModelFull : BaseViewModel
{
    private ObservableCollection<ChartDataPoint> _earthquakeData;
    private ObservableCollection<ChartDataPoint> _threatData;
    private ObservableCollection<ChartDataPoint> _newsData;
    private string _selectedChart = "Earthquakes";
    private string _selectedTimeRange = "24h";

    public ObservableCollection<ChartDataPoint> EarthquakeData
    {
        get => _earthquakeData ??= new();
        set => SetProperty(value, nameof(EarthquakeData));
    }

    public ObservableCollection<ChartDataPoint> ThreatData
    {
        get => _threatData ??= new();
        set => SetProperty(value, nameof(ThreatData));
    }

    public ObservableCollection<ChartDataPoint> NewsData
    {
        get => _newsData ??= new();
        set => SetProperty(value, nameof(NewsData));
    }

    public string SelectedChart { get => _selectedChart; set => SetProperty(value, nameof(SelectedChart)); }
    public string SelectedTimeRange { get => _selectedTimeRange; set => SetProperty(value, nameof(SelectedTimeRange)); }

    public List<string> ChartOptions => new() { "Earthquakes", "Threats", "News", "Events" };
    public List<string> TimeRangeOptions => new() { "1h", "6h", "24h", "7d", "30d", "1y" };

    public ChartsViewModelFull()
    {
        Title = "Analytics & Charts";
        LoadChartData();
    }

    private void LoadChartData()
    {
        // Sample data
        EarthquakeData = new ObservableCollection<ChartDataPoint>
        {
            new() { Label = "00:00", Value = 2, Category = "Low" },
            new() { Label = "04:00", Value = 5, Category = "Moderate" },
            new() { Label = "08:00", Value = 3, Category = "Low" },
            new() { Label = "12:00", Value = 7, Category = "High" },
            new() { Label = "16:00", Value = 4, Category = "Moderate" },
            new() { Label = "20:00", Value = 6, Category = "Moderate" }
        };
    }

    public RelayCommand RefreshChartCommand => new(async () =>
    {
        IsBusy = true;
        try
        {
            await Task.Delay(500);
            LoadChartData();
        }
        finally
        {
            IsBusy = false;
        }
    });
}
