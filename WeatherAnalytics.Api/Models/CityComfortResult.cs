namespace WeatherAnalytics.Api.Models;

public class CityComfortResult
{
    public string CityCode { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string WeatherDescription { get; set; } = string.Empty;
    public string TempCelsius { get; set; } = string.Empty;
    public double ComfortScore { get; set; }
    public int Rank { get; set; }
}