# Deployment Guide

## Prerequisites
- .NET 8.0 SDK
- Android SDK (API 21+)
- Git

## Building for Android

### Debug Build
```bash
dotnet maui run -f net8.0-android
```

### Release Build
```bash
dotnet publish -f net8.0-android -c Release -p:AndroidKeyStore=false
```

### Signed Release APK
```bash
dotnet publish -f net8.0-android -c Release -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=true -p:AndroidSigningKeyAlias=mykey -p:AndroidSigningKeyPass=pass -p:AndroidSigningStorePass=storepass
```

## Continuous Integration with GitHub Actions

See `.github/workflows/android-build.yml`

## App Store Deployment

1. Create Google Play Console account
2. Create app listing
3. Build and sign APK
4. Upload to Google Play Console
5. Submit for review
