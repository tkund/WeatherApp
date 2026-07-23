using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Tests;

[TestClass]
public sealed class WeatherServiceTests
{
    [TestMethod]
    public async Task GetWeatherAsync_ShouldReturnWeatherForKnownCity()
    {
        var provider = new FakeWeatherProvider();
        var service = new WeatherService(provider);

        var result = await service.GetWeatherAsync("Madrid");

        Assert.AreEqual("Madrid", result.City);
        Assert.IsTrue(result.TemperatureC > -100);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Condition));
    }

    [TestMethod]
    public async Task GetWeatherAsync_ShouldRejectEmptyCity()
    {
        var provider = new FakeWeatherProvider();
        var service = new WeatherService(provider);

        await Assert.ThrowsExceptionAsync<ArgumentException>(() => service.GetWeatherAsync(string.Empty));
    }

    [TestMethod]
    public async Task GetWeatherAsync_ShouldTrimAndNormalizeCityName()
    {
        var provider = new FakeWeatherProvider();
        var service = new WeatherService(provider);

        var result = await service.GetWeatherAsync("   barcelona   ");

        Assert.AreEqual("Barcelona", result.City);
    }

    private sealed class FakeWeatherProvider : IWeatherProvider
    {
        public Task<WeatherResponse> GetWeatherAsync(string city)
        {
            var normalizedCity = city.Trim();
            var displayCity = normalizedCity.Equals("barcelona", StringComparison.OrdinalIgnoreCase)
                ? "barcelona"
                : normalizedCity;

            return Task.FromResult(new WeatherResponse
            {
                City = displayCity,
                TemperatureC = 23,
                Condition = "Sunny",
                Humidity = 55,
                WindSpeedKph = 14,
                LastUpdatedUtc = DateTime.UtcNow
            });
        }
    }
}
