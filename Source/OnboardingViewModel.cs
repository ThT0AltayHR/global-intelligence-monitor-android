using GlobalIntelligence.MVVM;
using System.Collections.ObjectModel;

namespace GlobalIntelligence.ViewModels;

public class OnboardingPage
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string IconEmoji { get; set; } = "";
    public ObservableCollection<string> Features { get; set; } = new();
}

public class OnboardingViewModel : BaseViewModel
{
    private ObservableCollection<OnboardingPage> _onboardingPages;
    private int _currentPageIndex;
    private string _nextButtonText = "Next";

    public ObservableCollection<OnboardingPage> OnboardingPages
    {
        get => _onboardingPages;
        set => SetProperty(value, nameof(OnboardingPages));
    }

    public int CurrentPageIndex
    {
        get => _currentPageIndex;
        set => SetProperty(value, nameof(CurrentPageIndex));
    }

    public string NextButtonText
    {
        get => _nextButtonText;
        set => SetProperty(value, nameof(NextButtonText));
    }

    public ObservableCollection<bool> Indicators { get; } = new();

    public OnboardingViewModel()
    {
        Title = "Welcome";
        InitializePages();
    }

    private void InitializePages()
    {
        var pages = new ObservableCollection<OnboardingPage>
        {
            new OnboardingPage
            {
                Title = "Global Intelligence",
                Description = "Monitor earthquakes, cyber threats, and global events in real-time",
                IconEmoji = "🌍",
                Features = new() { "Real-time Updates", "3D Globe Visualization", "Multi-source Data" }
            },
            new OnboardingPage
            {
                Title = "Smart Alerts",
                Description = "Get notified about events that matter to you",
                IconEmoji = "🔔",
                Features = new() { "Customizable Alerts", "Multiple Channels", "Priority Filtering" }
            },
            new OnboardingPage
            {
                Title = "Advanced Analytics",
                Description = "Deep insights into global patterns and trends",
                IconEmoji = "📊",
                Features = new() { "Historical Data", "Trend Analysis", "Predictions" }
            }
        };

        OnboardingPages = pages;
        UpdateIndicators();
    }

    private void UpdateIndicators()
    {
        Indicators.Clear();
        for (int i = 0; i < OnboardingPages.Count; i++)
        {
            Indicators.Add(i == CurrentPageIndex);
        }
    }

    public RelayCommand NextCommand => new(() =>
    {
        if (CurrentPageIndex < OnboardingPages.Count - 1)
        {
            CurrentPageIndex++;
            UpdateIndicators();
            NextButtonText = CurrentPageIndex == OnboardingPages.Count - 1 ? "Finish" : "Next";
        }
        else
        {
            // Navigate to main app
        }
    });

    public RelayCommand SkipCommand => new(() =>
    {
        // Skip onboarding
    });

    public RelayCommand<int> PageChangeCommand => new(page =>
    {
        CurrentPageIndex = page;
        UpdateIndicators();
        NextButtonText = page == OnboardingPages.Count - 1 ? "Finish" : "Next";
    });
}
