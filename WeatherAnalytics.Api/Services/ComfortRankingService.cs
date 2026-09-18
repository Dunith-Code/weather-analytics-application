using System.Globalization;
using WeatherAnalytics.Api.Models;

namespace WeatherAnalytics.Api.Services;

public interface IComfortRankingService
{
    Task<List<CityComfortResult>> GetRankedCitiesAsync();
}

public class ComfortRankingService : IComfortRankingService
{
    private readonly ICityRepository _cityRepository;
    private readonly IWeatherService _weatherService;
    private readonly IComfortIndexCalculator _comfortIndexCalculator;
    private readonly ILogger<ComfortRankingService> _logger;

    public ComfortRankingService(
        ICityRepository cityRepository,
        IWeatherService weatherService,
        IComfortIndexCalculator comfortIndexCalculator,
        ILogger<ComfortRankingService> logger
    )
    {
        _cityRepository = cityRepository;
        _weatherService = weatherService;
        _comfortIndexCalculator = comfortIndexCalculator;
        _logger = logger;
    }

    public async Task<List<CityComfortResult>> GetRankedCitiesAsync()
    {
        var cities = _cityRepository.GetAllCities();

        // fetch all cities' weather in parallel rather than sequentially
        var fetchTask = cities.Select(async city =>
        {
            var weather = await _weatherService.GetWeatherByCityCodeAsync(city.CityCode);
            return (City: city, Weather: weather);
        });

        var results = await Task.WhenAll(fetchTask);

        var comfortResults = new List<CityComfortResult>();

        foreach (var (city, weather) in results)
        {
            if (weather == null)
            {
                _logger.LogWarning("Skipping {CityName} - weather fetch failed", city.CityName);
                continue;
            }

            var input = new WeatherInput(
                TempCelsius: weather.Main.Temp,
                HumidityPercent: weather.Main.Humidity,
                WindSpeedMs: weather.Wind.Speed,
                CloudinessPercent: weather.Clouds.All,
                PressureHpa: weather.Main.Pressure,
                VisibilityMeters: weather.Visibility
            );

            var score = _comfortIndexCalculator.Calculate(input);

            comfortResults.Add(new CityComfortResult
            {
                CityName = weather.Name,
                WeatherDescription = weather.Weather.FirstOrDefault()?.Description ?? "N/A",
                TempCelsius = weather.Main.Temp.ToString(CultureInfo.InvariantCulture),
                ComfortScore = score
            });
        }

        // Rank from most to least comfortable
        var ranked = comfortResults
            .OrderByDescending(r => r.ComfortScore)
            .ToList();

        for (int i = 0; i < ranked.Count; i++)
        {
            ranked[i].Rank = i + 1;
        }

        return ranked;
    }
}