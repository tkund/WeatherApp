using System.Net.Http.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public sealed class CitySuggestionService(HttpClient httpClient) : ICitySuggestionService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IReadOnlyList<CitySuggestion>> GetSuggestionsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<CitySuggestion>();
        }

        var response = await _httpClient.GetFromJsonAsync<GeocodingResponse>(
            $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(query)}&count=5&language=en&format=json");

        if (response?.Results is null || response.Results.Count == 0)
        {
            return Array.Empty<CitySuggestion>();
        }

        return response.Results
            .Select(r => new CitySuggestion
            {
                City = r.Name,
                Country = r.Country
            })
            .ToList();
    }

    private sealed class GeocodingResponse
    {
        public List<GeocodingResult>? Results { get; init; }
    }

    private sealed class GeocodingResult
    {
        public string Name { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
    }
}
