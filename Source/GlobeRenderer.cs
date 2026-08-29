using SkiaSharp;
using System.Diagnostics;

namespace GlobalIntelligenceMonitor.Services;

/// <summary>
/// 3D Dünya Küresi Renderer - Ultra HD kalitede SkiaSharp kullanımı
/// </summary>
public interface IGlobeRenderer
{
    void DrawGlobe(SKCanvas canvas, SKSize size, Dictionary<string, object> layers);
    void RotateGlobe(double deltaX, double deltaY);
    void ZoomGlobe(float zoomLevel);
    void DrawEarthquakesLayer(SKCanvas canvas, List<Models.EarthquakeData> earthquakes);
    void DrawFlightLayer(SKCanvas canvas, List<Models.AircraftData> aircraft);
    void DrawShipLayer(SKCanvas canvas, List<Models.ShipData> ships);
    void DrawCyberThreatsLayer(SKCanvas canvas, List<Models.CyberThreat> threats);
    void DrawNuclearThreatsLayer(SKCanvas canvas, List<Models.NuclearThreat> threats);
}

public class GlobeRenderer : IGlobeRenderer
{
    private float _rotationX = 0;
    private float _rotationY = 0;
    private float _zoomLevel = 1.0f;
    private const float MAX_ZOOM = 3.0f;
    private const float MIN_ZOOM = 0.5f;

    // Renk Paleti - Profesyonel istihbarat teması
    private static readonly SKColor[] _severityColors = new[]
    {
        SKColors.Green,      // Low
        SKColors.Yellow,     // Moderate
        SKColors.Orange,     // High
        SKColors.Red,        // Critical
        SKColors.DarkRed     // Extreme
    };

    private static readonly SKColor _oceanColor = new(30, 60, 114);
    private static readonly SKColor _landColor = new(70, 90, 60);
    private static readonly SKColor _gridColor = new(100, 100, 100, 100);
    private static readonly SKColor _backgroundColor = new(10, 15, 10);

    public void DrawGlobe(SKCanvas canvas, SKSize size, Dictionary<string, object> layers)
    {
        try
        {
            // Arka plan
            canvas.DrawColor(_backgroundColor);

            // Dünya merkezi
            var centerX = size.Width / 2;
            var centerY = size.Height / 2;
            var radius = Math.Min(size.Width, size.Height) / 2 * 0.8f * _zoomLevel;

            // Temel küre çiz
            DrawEarthSphere(canvas, centerX, centerY, radius);

            // Grid çiz
            DrawGrid(canvas, centerX, centerY, radius);

            // Katmanları çiz
            if (layers.ContainsKey("earthquakes") && layers["earthquakes"] is List<Models.EarthquakeData> earthquakes)
            {
                DrawEarthquakesLayer(canvas, earthquakes);
            }

            if (layers.ContainsKey("flights") && layers["flights"] is List<Models.AircraftData> aircraft)
            {
                DrawFlightLayer(canvas, aircraft);
            }

            if (layers.ContainsKey("ships") && layers["ships"] is List<Models.ShipData> ships)
            {
                DrawShipLayer(canvas, ships);
            }

            if (layers.ContainsKey("cyberThreats") && layers["cyberThreats"] is List<Models.CyberThreat> threats)
            {
                DrawCyberThreatsLayer(canvas, threats);
            }

            if (layers.ContainsKey("nuclearThreats") && layers["nuclearThreats"] is List<Models.NuclearThreat> nuclear)
            {
                DrawNuclearThreatsLayer(canvas, nuclear);
            }

            // İstatistikler ve kontrolü çiz
            DrawGlobeInfo(canvas, size);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error drawing globe: {ex.Message}");
        }
    }

    private void DrawEarthSphere(SKCanvas canvas, float centerX, float centerY, float radius)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        // Okyanus rengi
        paint.Color = _oceanColor;
        canvas.DrawCircle(centerX, centerY, radius, paint);

        // Basit landmass çiz (gerçek harita için texture kullanılmalı)
        paint.Color = _landColor;

        // Kuzey Amerika
        DrawLandmass(canvas, centerX - radius * 0.4f, centerY - radius * 0.2f, radius * 0.15f, paint);

        // Avrupa
        DrawLandmass(canvas, centerX + radius * 0.1f, centerY - radius * 0.3f, radius * 0.1f, paint);

        // Afrika
        DrawLandmass(canvas, centerX + radius * 0.2f, centerY + radius * 0.1f, radius * 0.12f, paint);

        // Asya
        DrawLandmass(canvas, centerX + radius * 0.3f, centerY - radius * 0.1f, radius * 0.2f, paint);

        // Avustralya
        DrawLandmass(canvas, centerX + radius * 0.4f, centerY + radius * 0.3f, radius * 0.08f, paint);

        // 3D efekt için gölge
        paint.Color = new SKColor(0, 0, 0, 50);
        canvas.DrawCircle(centerX + radius * 0.1f, centerY + radius * 0.1f, radius, paint);
    }

    private void DrawLandmass(SKCanvas canvas, float x, float y, float size, SKPaint paint)
    {
        canvas.DrawCircle(x, y, size, paint);
    }

    private void DrawGrid(SKCanvas canvas, float centerX, float centerY, float radius)
    {
        using var paint = new SKPaint
        {
            Color = _gridColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 0.5f,
            IsAntialias = true
        };

        // Ekvator
        canvas.DrawCircle(centerX, centerY, radius, paint);

        // Meridyen çizgileri
        for (int i = -180; i <= 180; i += 30)
        {
            var angle = i * Math.PI / 180;
            var x1 = centerX + (float)(radius * Math.Cos(angle));
            var y1 = centerY + (float)(radius * Math.Sin(angle) * 0.3f);
            var x2 = centerX + (float)(radius * Math.Cos(angle));
            var y2 = centerY - (float)(radius * Math.Sin(angle) * 0.3f);

            canvas.DrawLine(x1, y1, x2, y2, paint);
        }

        // Enlem çizgileri
        for (int i = -60; i <= 60; i += 30)
        {
            var r = radius * (float)Math.Cos(i * Math.PI / 180);
            var y = centerY - (float)(radius * Math.Sin(i * Math.PI / 180));
            canvas.DrawCircle(centerX, y, r, paint);
        }
    }

    public void DrawEarthquakesLayer(SKCanvas canvas, List<Models.EarthquakeData> earthquakes)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using var outlinePaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.5f
        };

        foreach (var eq in earthquakes)
        {
            var color = _severityColors[(int)eq.SeverityLevel];
            paint.Color = color;
            outlinePaint.Color = new SKColor(255, 255, 255, 100);

            // Koordinatları kanvasa çevir
            var (x, y) = LatLonToCanvas(eq.Latitude, eq.Longitude);

            // Büyüklüğe göre boyut
            var size = Math.Min(3 + eq.Magnitude * 1.5f, 15f);

            // Çember çiz
            canvas.DrawCircle(x, y, size, paint);
            canvas.DrawCircle(x, y, size, outlinePaint);

            // Etki alanını göster (büyük depremlerde)
            if (eq.Magnitude >= 6.0)
            {
                paint.Color = new SKColor(color.Red, color.Green, color.Blue, 30);
                canvas.DrawCircle(x, y, size * 3, paint);
            }
        }
    }

    public void DrawFlightLayer(SKCanvas canvas, List<Models.AircraftData> aircraft)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = new SKColor(0, 200, 255) // Uçak mavi rengi
        };

        using var pathPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = new SKColor(0, 150, 200, 80)
        };

        foreach (var aircraft_item in aircraft.Take(100)) // Performans için limit
        {
            var (x, y) = LatLonToCanvas(aircraft_item.Latitude, aircraft_item.Longitude);

            // Uçak ikonu çiz
            var size = aircraft_item.IsMilitary ? 5 : 3;
            canvas.DrawCircle(x, y, size, paint);

            // Rotasyonu göster
            var endX = x + (float)Math.Cos(aircraft_item.Heading * Math.PI / 180) * 20;
            var endY = y + (float)Math.Sin(aircraft_item.Heading * Math.PI / 180) * 20;
            canvas.DrawLine(x, y, endX, endY, pathPaint);
        }
    }

    public void DrawShipLayer(SKCanvas canvas, List<Models.ShipData> ships)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = new SKColor(0, 100, 200) // Gemi rengi
        };

        foreach (var ship in ships.Take(100))
        {
            var (x, y) = LatLonToCanvas(ship.Latitude, ship.Longitude);
            var size = ship.IsMilitary ? 4 : 2.5f;

            canvas.DrawRect(x - size, y - size, size * 2, size * 2, paint);
        }
    }

    public void DrawCyberThreatsLayer(SKCanvas canvas, List<Models.CyberThreat> threats)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        foreach (var threat in threats.Where(t => t.SourceCountry != null).Take(50))
        {
            var color = _severityColors[(int)threat.Severity];
            paint.Color = new SKColor(color.Red, color.Green, color.Blue, 200);

            // Tehdidin kaynağında kare çiz
            var sourceCountryCoords = GetCountryCenter(threat.SourceCountry);
            var (sx, sy) = LatLonToCanvas(sourceCountryCoords.Latitude, sourceCountryCoords.Longitude);

            canvas.DrawRect(sx - 2, sy - 2, 4, 4, paint);

            // Uyarı halesi
            paint.Color = new SKColor(color.Red, color.Green, color.Blue, 50);
            for (int i = 1; i <= 3; i++)
            {
                canvas.DrawCircle(sx, sy, 4 * i, paint);
            }
        }
    }

    public void DrawNuclearThreatsLayer(SKCanvas canvas, List<Models.NuclearThreat> threats)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = SKColors.Yellow
        };

        using var warnPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            Color = new SKColor(255, 100, 0, 150)
        };

        foreach (var threat in threats)
        {
            var (x, y) = LatLonToCanvas(threat.Latitude, threat.Longitude);
            var radius = threat.AlertRadiusKm / 100; // Ölçek

            // Tehdit konumu
            canvas.DrawCircle(x, y, 4, paint);

            // Uyarı radyusu
            canvas.DrawCircle(x, y, radius, warnPaint);
        }
    }

    public void RotateGlobe(double deltaX, double deltaY)
    {
        _rotationX = Math.Clamp(_rotationX + (float)deltaY * 0.5f, -90, 90);
        _rotationY = (_rotationY + (float)deltaX * 0.5f) % 360;
    }

    public void ZoomGlobe(float zoomLevel)
    {
        _zoomLevel = Math.Clamp(zoomLevel, MIN_ZOOM, MAX_ZOOM);
    }

    // ==================== Yardımcı Metotlar ====================

    private (float x, float y) LatLonToCanvas(double latitude, double longitude)
    {
        // Mercator projeksiyon
        var x = (float)(longitude + 180) / 360;
        var y = (float)(1 - Math.Log(Math.Tan(Math.PI * (latitude + 90) / 360)) / Math.PI) / 2;

        return (x, y);
    }

    private Models.GeoPoint GetCountryCenter(string countryCode)
    {
        return countryCode switch
        {
            "US" => new Models.GeoPoint { Latitude = 37.0902, Longitude = -95.7129 },
            "TR" => new Models.GeoPoint { Latitude = 38.9637, Longitude = 35.2433 },
            "RU" => new Models.GeoPoint { Latitude = 61.5240, Longitude = 105.3188 },
            "CN" => new Models.GeoPoint { Latitude = 35.8617, Longitude = 104.1954 },
            "GB" => new Models.GeoPoint { Latitude = 55.3781, Longitude = -3.4360 },
            _ => new Models.GeoPoint { Latitude = 0, Longitude = 0 }
        };
    }

    private void DrawGlobeInfo(SKCanvas canvas, SKSize size)
    {
        using var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = 12,
            IsAntialias = true,
            Typeface = SKTypeface.CreateDefault()
        };

        canvas.DrawText($"Zoom: {_zoomLevel:F1}x", 10, 20, paint);
        canvas.DrawText($"Rotation: X={_rotationX:F0}° Y={_rotationY:F0}°", 10, 35, paint);
    }
}

/// <summary>
/// Harita Renderer - 2D Harita Görünümü
/// </summary>
public interface IMapRenderer
{
    void DrawMap(SKCanvas canvas, SKSize size, Dictionary<string, object> layers);
}

public class MapRenderer : IMapRenderer
{
    public void DrawMap(SKCanvas canvas, SKSize size, Dictionary<string, object> layers)
    {
        using var paint = new SKPaint
        {
            Color = new SKColor(30, 60, 114),
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        canvas.DrawRect(0, 0, size.Width, size.Height, paint);

        // Harita katmanlarını çiz
        // Bu, WebMercator projeksiyon kullanmalı
    }
}

/// <summary>
/// Tema Servisi - Renk Şemaları
/// </summary>
public interface IThemeService
{
    void SetTheme(string themeName);
    SKColor GetPrimaryColor();
    SKColor GetSecondaryColor();
    SKColor GetAccentColor();
}

public class ThemeService : IThemeService
{
    private string _currentTheme = "dark";

    public void SetTheme(string themeName)
    {
        _currentTheme = themeName;
    }

    public SKColor GetPrimaryColor()
    {
        return _currentTheme == "dark" 
            ? new SKColor(10, 15, 10)  // Koyu arka plan
            : new SKColor(240, 240, 240); // Açık arka plan
    }

    public SKColor GetSecondaryColor()
    {
        return _currentTheme == "dark"
            ? new SKColor(50, 60, 50)
            : new SKColor(200, 200, 200);
    }

    public SKColor GetAccentColor()
    {
        return new SKColor(0, 200, 255); // Siyan - modern OSINT teması
    }
}
