using Xunit;
using GlobalIntelligence.Services;

namespace GlobalIntelligence.Tests;

public class EarthquakeServiceTests
{
    [Fact]
    public async Task GetRecentEarthquakes_ReturnsData()
    {
        // Arrange
        var service = new EarthquakeService(null);

        // Act
        var result = await service.GetRecentEarthquakesAsync(24);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetEarthquakesByMagnitude_FiltersCorrectly()
    {
        // Test magnitude filtering
    }

    [Theory]
    [InlineData(2.0)]
    [InlineData(5.0)]
    [InlineData(7.0)]
    public async Task Severity_Classification_IsAccurate(double magnitude)
    {
        // Test different magnitudes
    }
}
