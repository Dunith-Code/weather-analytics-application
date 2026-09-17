using Microsoft.AspNetCore.Mvc;
using WeatherAnalytics.Api.Services;

namespace WeatherAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICityRepository _cityRepository;

    public CitiesController(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    [HttpGet]
    public IActionResult GetCities()
    {
        var cities = _cityRepository.GetAllCities();
        return Ok(cities);
    }
}