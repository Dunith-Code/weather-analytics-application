using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherAnalytics.Api.Services;

namespace WeatherAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ForecastController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public ForecastController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("{cityCode}")]
    public async Task<IActionResult> GetForecast(string cityCode)
    {
        var forecast = await _weatherService.GetForecastByCityCodeAsync(cityCode);
        return Ok(forecast);
    }
}