using Microsoft.Extensions.Caching.Memory;
using WeatherAnalytics.Api.Models;
using WeatherAnalytics.Api.Services;

namespace WeatherAnalytics.Api.Services;

public interface IWeatherCacheService
{
    Task<OpenWeatherResponse?> GetOrFetchWeatherAsync(string cityCode, Func<Task<OpenWeatherResponse?>> fetchFunc);
    List<CityComfortResult>? GetCachedRankedResults();
    void SetCachedRankedResults(List<CityComfortResult> results);
    Dictionary<string, string> GetCacheStatusSnapshot();
}

public class WeatherCacheService: IWeatherCacheService
{
    private readonly IMemoryCache _cache;
    private readonly Dictionary<string, string> _lastStatusPerCity = new();
    private static readonly TimeSpan RawWeatherTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan RankedResultsTtl = TimeSpan.FromMinutes(5);
    private const string RankedResultsKey = "comfort:ranked-results";

    public WeatherCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<OpenWeatherResponse?> GetOrFetchWeatherAsync(string cityCode, Func<Task<OpenWeatherResponse?>> fetchFunc)
    {
        var cacheKey = $"weather:{cityCode}";

        if (_cache.TryGetValue(cacheKey, out OpenWeatherResponse? cached))
        {
            _lastStatusPerCity[cityCode] = "HIT";
            return cached;
        }

        _lastStatusPerCity[cityCode] = "MISS";

        var fresh = await fetchFunc();
        if (fresh != null)
        {
            _cache.Set(cacheKey, fresh, RawWeatherTtl);
        }

        return fresh;
    }

    public List<CityComfortResult>? GetCachedRankedResults()
    {
        return _cache.TryGetValue(RankedResultsKey, out List<CityComfortResult>? results) ? results : null;
    }

    public void SetCachedRankedResults(List<CityComfortResult> results)
    {
        _cache.Set(RankedResultsKey, results, RankedResultsTtl);
    }

    public Dictionary<string, string> GetCacheStatusSnapshot()
    {
        return new Dictionary<string, string>(_lastStatusPerCity);
    }
}