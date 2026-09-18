using Microsoft.AspNetCore.Mvc;
using WeatherAnalytics.Api.Services;

namespace WeatherAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CacheController : ControllerBase
{
    private readonly IWeatherCacheService _cacheService;

    public CacheController(IWeatherCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    [HttpGet("status")]
    public IActionResult GetCacheStatus()
    {
        var snapshot = _cacheService.GetCacheStatusSnapshot();
        return Ok(snapshot);
    }
}