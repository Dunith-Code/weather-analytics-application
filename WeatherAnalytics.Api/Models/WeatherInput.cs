namespace WeatherAnalytics.Api.Models;

public record WeatherInput(
    double TempCelsius,
    double HumidityPercent,
    double WindSpeedMs,
    double CloudinessPercent,
    double PressureHpa,
    double VisibilityMeters
);