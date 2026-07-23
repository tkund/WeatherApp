namespace WeatherApp.Models;

public sealed record CitySuggestion
{
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string DisplayName => string.IsNullOrWhiteSpace(Country) ? City : $"{City}, {Country}";
}
