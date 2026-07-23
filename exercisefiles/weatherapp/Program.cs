using WeatherApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IWeatherProvider, OpenMeteoWeatherProvider>();
builder.Services.AddHttpClient<ICitySuggestionService, CitySuggestionService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.MapGet("/api/weather", async (string city, IWeatherService weatherService) =>
{
    try
    {
        var result = await weatherService.GetWeatherAsync(city);
        return Results.Ok(result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, title: "Weather lookup failed");
    }
});

app.MapGet("/api/cities", async (string query, ICitySuggestionService citySuggestionService) =>
{
    var suggestions = await citySuggestionService.GetSuggestionsAsync(query);
    return Results.Ok(suggestions);
});

app.MapFallbackToFile("/index.html");

app.Run();
