using System.Text.Json.Serialization;

namespace WeatherAnalytics.Api.Models;

public class ForecastResponse
{
    [JsonPropertyName("list")]
    public List<ForecastEntry> List { get; set; } = new();
}

public class ForecastEntry
{
    [JsonPropertyName("dt_txt")]
    public string DateText { get; set; } = string.Empty;

    [JsonPropertyName("main")]
    public MainData Main { get; set; } = new();
}

public class ForecastPoint
{
    public string Time { get; set; } = string.Empty;
    public double TempCelsius { get; set; }
}