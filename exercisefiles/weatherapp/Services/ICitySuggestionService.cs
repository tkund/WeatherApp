using WeatherApp.Models;

namespace WeatherApp.Services;

public interface ICitySuggestionService
{
    Task<IReadOnlyList<CitySuggestion>> GetSuggestionsAsync(string query);
}
