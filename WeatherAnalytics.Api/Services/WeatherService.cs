using System.Text.Json;
using WeatherAnalytics.Api.Models;

namespace WeatherAnalytics.Api.Services;

public interface IWeatherService
{
    Task<OpenWeatherResponse?> GetWeatherByCityCodeAsync(string cityCode);
    Task<List<ForecastPoint>> GetForecastByCityCodeAsync(string cityCode);
}

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(HttpClient httpClient, IConfiguration config, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<OpenWeatherResponse?> GetWeatherByCityCodeAsync(string cityCode)
    {
        var apiKey = _config["OpenWeatherMap:ApiKey"];
        var baseUrl = _config["OpenWeatherMap:BaseUrl"];

        var url = $"{baseUrl}?id={cityCode}&appid={apiKey}&units=metric";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("OpenWeatherMap request failed for city {cityCode} with status {StatusCode}", cityCode, response.StatusCode);
                return null;
            }
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OpenWeatherResponse>(json);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error fetching weather for city {CityCode}", cityCode);
            return null;
        }
    }

    public async Task<List<ForecastPoint>> GetForecastByCityCodeAsync(string cityCode)
    {
        var apiKey = _config["OpenWeatherMap:ApiKey"];
        var url = $"https://api.openweathermap.org/data/2.5/forecast?id={cityCode}&appid={apiKey}&units=metric";

        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<ForecastPoint>();

            var json = await response.Content.ReadAsStringAsync();
            var forecast = JsonSerializer.Deserialize<ForecastResponse>(json);

            return forecast?.List
                .Take(8) // next 24 hours (3-hour intervals)
                .Select(entry => new ForecastPoint
                {
                    Time = entry.DateText,
                    TempCelsius = entry.Main.Temp
                })
                .ToList() ?? new List<ForecastPoint>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching forecast for city {CityCode}", cityCode);
            return new List<ForecastPoint>();
        }
    }
}