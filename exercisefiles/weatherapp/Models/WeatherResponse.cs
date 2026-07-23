namespace WeatherApp.Models;

public sealed record WeatherResponse
{
    public string City { get; init; } = string.Empty;
    public decimal TemperatureC { get; init; }
    public string Condition { get; init; } = string.Empty;
    public int Humidity { get; init; }
    public decimal WindSpeedKph { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
}
