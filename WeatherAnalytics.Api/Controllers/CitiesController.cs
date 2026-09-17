using Microsoft.AspNetCore.Mvc;
using WeatherAnalytics.Api.Services;

namespace WeatherAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICityRepository _cityRepository;
    private readonly IWeatherService _weatherService;

    public CitiesController(ICityRepository cityRepository, IWeatherService weatherService)
    {
        _cityRepository = cityRepository;
        _weatherService = weatherService;
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
        return Ok(weather);
    }
}