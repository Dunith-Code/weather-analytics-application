using System.Text.Json;
using WeatherAnalytics.Api.Models;

namespace WeatherAnalytics.Api.Services;

public interface ICityRepository
{
    List<CityEntry> GetAllCities();
}

public class CityRepository : ICityRepository
{
    private readonly List<CityEntry> _cities;

    public CityRepository(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "cities.json");
        var json = File.ReadAllText(path);

        var wrapper = JsonSerializer.Deserialize<CityListWrapper>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        _cities = wrapper?.List ?? new List<CityEntry>();
    }

    public List<CityEntry> GetAllCities() => _cities;
}