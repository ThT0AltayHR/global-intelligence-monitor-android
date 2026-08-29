using Microsoft.Maui.Controls;

namespace GlobalIntelligenceMonitor;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}