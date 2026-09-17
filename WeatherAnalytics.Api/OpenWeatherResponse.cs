using System.Text.Json.Serialization;

namespace WeatherAnalytics.Api.Models;

public class OpenWeatherResponse
{
    [JsonPropertyName("weather")]
    public List<WeatherDescription> Weather { get; set; } = new();

    [JsonPropertyName("main")]
    public MainData Main { get; set; } = new();

    [JsonPropertyName("wind")]
    public WindData Wind { get; set; } = new();
    
    [JsonPropertyName("clouds")]
    public CloudsData Clouds { get; set; } = new();

    [JsonPropertyName("visibility")]
    public double Visibility { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public long Id { get; set; }
}

public class WeatherDescription
{
    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public class MainData
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("humidity")]
    public double Humidity { get; set; }

    [JsonPropertyName("pressure")]
    public double Pressure { get; set; }
}

public class WindData
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}

public class CloudsData
{
    [JsonPropertyName("all")]
    public double All { get; set; }
}