using Microsoft.AspNetCore.Mvc;
using WeatherAnalytics.Api.Services;
using WeatherAnalytics.Api.Models;

namespace WeatherAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICityRepository _cityRepository;
    private readonly IWeatherService _weatherService;
    private readonly IComfortIndexCalculator _comfortIndexCalculator;

    public CitiesController(ICityRepository cityRepository, IWeatherService weatherService, IComfortIndexCalculator comfortIndexCalculator)
    {
        _cityRepository = cityRepository;
        _weatherService = weatherService;
        _comfortIndexCalculator = comfortIndexCalculator;
    }

    [HttpGet]
    public IActionResult GetCities()
    {
        var cities = _cityRepository.GetAllCities();
        return Ok(cities);
    }

    [HttpGet("test-weather/{cityCode}")]
    public async Task<IActionResult> TestWeather(string cityCode)
    {
        var weather = await _weatherService.GetWeatherByCityCodeAsync(cityCode);
        if (weather == null) return NotFound("Could not fetch weather data");

        var input = new WeatherAnalytics.Api.Models.WeatherInput(
            TempCelsius: weather.Main.Temp,
            HumidityPercent: weather.Main.Humidity,
            WindSpeedMs: weather.Wind.Speed,
            CloudinessPercent: weather.Clouds.All,
            PressureHpa: weather.Main.Pressure,
            VisibilityMeters: weather.Visibility
        );

        var score = _comfortIndexCalculator.Calculate(input);

        return Ok(new { weather.Name, ComfortScore = score, RawWeather = weather });
    }
}