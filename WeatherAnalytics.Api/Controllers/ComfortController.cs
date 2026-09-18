using Microsoft.AspNetCore.Mvc;
using WeatherAnalytics.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace WeatherAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComfortController : ControllerBase
{
    private readonly IComfortRankingService _comfortRankingService;

    public ComfortController(IComfortRankingService comfortRankingService)
    {
        _comfortRankingService = comfortRankingService;
    }

    [HttpGet("ranked-cities")]
    public async Task<IActionResult> GetRankedCities()
    {
        var rankedCities = await _comfortRankingService.GetRankedCitiesAsync();
        return Ok(rankedCities);
    }
}