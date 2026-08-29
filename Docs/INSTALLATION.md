# Installation Guide

## System Requirements

### Minimum Requirements
- OS: Windows 10/macOS 10.14/Linux (Ubuntu 18.04+)
- RAM: 4GB
- Storage: 2GB free space
- .NET SDK: 8.0 or higher
- Android SDK: API Level 21+

### Recommended
- RAM: 8GB+
- Storage: 10GB
- Latest Android SDK (API 34+)
- Visual Studio 2022 17.8+ or VS Code with C# extension

## Installation Steps

### 1. Clone Repository
```bash
git clone https://github.com/yourusername/global-intelligence-monitor.git
cd global-intelligence-monitor
```

### 2. Install .NET Workloads
```bash
dotnet workload restore
dotnet workload install maui
```

### 3. Restore NuGet Packages
```bash
dotnet restore
```

### 4. Configure API Keys
Create `.env` file:
```
NEWS_API_KEY=your_key_here
GUARDIAN_API_KEY=your_key_here
NVD_API_KEY=your_key_here
SHODAN_API_KEY=your_key_here
FIREBASE_PROJECT_ID=your_project_id
```

### 5. Build for Development
```bash
dotnet maui run -f net8.0-android
```

### 6. Release Build
```bash
dotnet publish -f net8.0-android -c Release -p:AndroidKeyStore=true
```

## Troubleshooting

### Issue: Workload installation fails
**Solution**: Update .NET SDK to latest version
```bash
dotnet sdk check
```

### Issue: Build fails with missing dependencies
**Solution**: Clear NuGet cache
```bash
dotnet nuget locals all --clear
dotnet restore
```

### Issue: Emulator doesn't start
**Solution**: Use Android device directly or:
```bash
emulator -avd YourAVDName -gpu on
```
