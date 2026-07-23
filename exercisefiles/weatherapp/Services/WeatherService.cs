using System.Globalization;
using WeatherApp.Models;

namespace WeatherApp.Services;

public sealed class WeatherService(IWeatherProvider weatherProvider) : IWeatherService
{
    private readonly IWeatherProvider _weatherProvider = weatherProvider;

    public async Task<WeatherResponse> GetWeatherAsync(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City must be supplied.", nameof(city));
        }

        var normalizedCity = city.Trim();
        if (normalizedCity.Length < 2 || normalizedCity.Length > 64)
        {
            throw new ArgumentException("City name must be between 2 and 64 characters.", nameof(city));
        }

        if (!normalizedCity.All(ch => char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch) || ch == '-'))
        {
            throw new ArgumentException("City contains unsupported characters.", nameof(city));
        }

        var result = await _weatherProvider.GetWeatherAsync(normalizedCity);

        var textInfo = CultureInfo.InvariantCulture.TextInfo;
        var cityName = textInfo.ToTitleCase(result.City.Trim().ToLowerInvariant());

        return result with
        {
            City = cityName
        };
    }
}
