using System.Net.Http.Json;
using System.Text.Json.Serialization;
using WeatherApp.Models;

namespace WeatherApp.Services;

public sealed class OpenMeteoWeatherProvider(HttpClient httpClient) : IWeatherProvider
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<WeatherResponse> GetWeatherAsync(string city)
    {
        var geoResponse = await _httpClient.GetFromJsonAsync<GeocodingResponse>(
            $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1&language=en&format=json");

        if (geoResponse?.Results is null || geoResponse.Results.Count == 0)
        {
            throw new InvalidOperationException($"Weather data could not be located for '{city}'.");
        }

        var location = geoResponse.Results[0];

        var forecastResponse = await _httpClient.GetFromJsonAsync<ForecastResponse>(
            $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude:F5}&longitude={location.Longitude:F5}&current=temperature_2m,relative_humidity_2m,wind_speed_10m,weather_code&timezone=auto");

        if (forecastResponse?.Current is null)
        {
            throw new InvalidOperationException($"Weather data could not be retrieved for '{city}'.");
        }

        return new WeatherResponse
        {
            City = location.Name,
            TemperatureC = Math.Round(forecastResponse.Current.TemperatureC, 1),
            Condition = MapWeatherCodeToCondition(forecastResponse.Current.WeatherCode),
            Humidity = forecastResponse.Current.RelativeHumidity,
            WindSpeedKph = Math.Round(forecastResponse.Current.WindSpeedKph, 1),
            LastUpdatedUtc = DateTime.UtcNow
        };
    }

    private static string MapWeatherCodeToCondition(int code) => code switch
    {
        0 => "Clear sky",
        1 or 2 => "Partly cloudy",
        3 => "Overcast",
        >= 45 and <= 48 => "Fog",
        >= 51 and <= 67 => "Rain",
        >= 71 and <= 77 => "Snow",
        >= 80 and <= 82 => "Rain showers",
        >= 85 and <= 86 => "Snow showers",
        >= 95 => "Thunderstorm",
        _ => "Variable conditions"
    };

    private sealed class GeocodingResponse
    {
        public List<GeocodingResult>? Results { get; init; }
    }

    private sealed class GeocodingResult
    {
        public string Name { get; init; } = string.Empty;
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }

    private sealed class ForecastResponse
    {
        public ForecastCurrent? Current { get; init; }
    }

    private sealed class ForecastCurrent
    {
        [JsonPropertyName("temperature_2m")]
        public decimal TemperatureC { get; init; }

        [JsonPropertyName("relative_humidity_2m")]
        public int RelativeHumidity { get; init; }

        [JsonPropertyName("wind_speed_10m")]
        public decimal WindSpeedKph { get; init; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; init; }
    }
}
