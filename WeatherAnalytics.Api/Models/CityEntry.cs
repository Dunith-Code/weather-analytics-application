namespace WeatherAnalytics.Api.Models;

public class CityListWrapper
{
    public List<CityEntry> List { get; set; } = new();
}

public class CityEntry
{
    public string CityCode { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string Temp { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}