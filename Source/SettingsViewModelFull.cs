using GlobalIntelligence.MVVM;

namespace GlobalIntelligence.ViewModels;

public class SettingsViewModelFull : BaseViewModel
{
    private string _userName = "User Name";
    private string _userEmail = "user@example.com";
    private bool _pushNotificationsEnabled = true;
    private bool _soundEnabled = true;
    private bool _vibrationEnabled = true;
    private bool _emailNotificationsEnabled = false;
    private string _earthquakeThreshold = "5.0 Magnitude";
    private string _cyberThresholdLevel = "High";
    private string _cacheSize = "125 MB";
    private string _databaseSize = "234 MB";
    private string _dataRetentionDays = "30 Days";
    private bool _isDarkTheme = true;
    private double _fontSizeScale = 1.0;
    private bool _compactModeEnabled = false;
    private bool _reduceMotionEnabled = false;
    private bool _biometricEnabled = false;
    private bool _vpnEnabled = false;
    private string _twoFactorStatus = "Enable 2FA";
    private string _appVersion = "v1.0.0";

    public string UserName { get => _userName; set => SetProperty(value, nameof(UserName)); }
    public string UserEmail { get => _userEmail; set => SetProperty(value, nameof(UserEmail)); }
    public bool PushNotificationsEnabled { get => _pushNotificationsEnabled; set => SetProperty(value, nameof(PushNotificationsEnabled)); }
    public bool SoundEnabled { get => _soundEnabled; set => SetProperty(value, nameof(SoundEnabled)); }
    public bool VibrationEnabled { get => _vibrationEnabled; set => SetProperty(value, nameof(VibrationEnabled)); }
    public bool EmailNotificationsEnabled { get => _emailNotificationsEnabled; set => SetProperty(value, nameof(EmailNotificationsEnabled)); }
    
    public string EarthquakeThreshold { get => _earthquakeThreshold; set => SetProperty(value, nameof(EarthquakeThreshold)); }
    public string CyberThresholdLevel { get => _cyberThresholdLevel; set => SetProperty(value, nameof(CyberThresholdLevel)); }
    
    public string CacheSize { get => _cacheSize; set => SetProperty(value, nameof(CacheSize)); }
    public string DatabaseSize { get => _databaseSize; set => SetProperty(value, nameof(DatabaseSize)); }
    public string DataRetentionDays { get => _dataRetentionDays; set => SetProperty(value, nameof(DataRetentionDays)); }
    
    public bool IsDarkTheme { get => _isDarkTheme; set => SetProperty(value, nameof(IsDarkTheme)); }
    public double FontSizeScale { get => _fontSizeScale; set => SetProperty(value, nameof(FontSizeScale)); }
    public bool CompactModeEnabled { get => _compactModeEnabled; set => SetProperty(value, nameof(CompactModeEnabled)); }
    public bool ReduceMotionEnabled { get => _reduceMotionEnabled; set => SetProperty(value, nameof(ReduceMotionEnabled)); }
    
    public bool BiometricEnabled { get => _biometricEnabled; set => SetProperty(value, nameof(BiometricEnabled)); }
    public bool VpnEnabled { get => _vpnEnabled; set => SetProperty(value, nameof(VpnEnabled)); }
    public string TwoFactorStatus { get => _twoFactorStatus; set => SetProperty(value, nameof(TwoFactorStatus)); }
    
    public string AppVersion { get => _appVersion; set => SetProperty(value, nameof(AppVersion)); }

    public List<string> ThresholdOptions => new() { "2.0", "3.0", "4.0", "5.0", "6.0", "7.0+" };
    public List<string> SeverityOptions => new() { "Low", "Moderate", "High", "Critical", "Extreme" };
    public List<string> RetentionOptions => new() { "7 Days", "14 Days", "30 Days", "90 Days", "1 Year" };
    public List<string> VpnProviders => new() { "None", "NordVPN", "ExpressVPN", "Surfshark" };

    public SettingsViewModelFull()
    {
        Title = "Settings";
    }

    public RelayCommand ClearCacheCommand => new(async () =>
    {
        IsBusy = true;
        try
        {
            // Clear cache logic
            CacheSize = "0 MB";
        }
        finally
        {
            IsBusy = false;
        }
    });

    public RelayCommand ExportDataCommand => new(async () =>
    {
        // Export data logic
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            Application.Current?.MainPage?.DisplayAlert("Export", "Data exported successfully", "OK");
        });
    });

    public RelayCommand<string> SetThemeCommand => new(theme =>
    {
        IsDarkTheme = theme == "dark";
        // Apply theme
    });

    public RelayCommand Toggle2FACommand => new(() =>
    {
        var enabled = TwoFactorStatus == "Enable 2FA";
        TwoFactorStatus = enabled ? "Disable 2FA" : "Enable 2FA";
    });
}
