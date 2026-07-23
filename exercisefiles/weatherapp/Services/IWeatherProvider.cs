using WeatherApp.Models;

namespace WeatherApp.Services;

public interface IWeatherProvider
{
    Task<WeatherResponse> GetWeatherAsync(string city);
}
