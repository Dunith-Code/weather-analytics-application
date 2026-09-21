using WeatherAnalytics.Api.Models;
using WeatherAnalytics.Api.Services;
using Xunit;

namespace WeatherAnalytics.Api.Tests;

public class ComfortIndexCalculatorTests
{
    private readonly ComfortIndexCalculator _calculator = new();

    [Fact]
    public void Calculate_IdealConditions_ReturnsHighScore()
    {
        // Near-ideal: ~21°C, moderate humidity, calm wind, clear, ideal pressure, good visibility
        var input = new WeatherInput(
            TempCelsius: 21,
            HumidityPercent: 50,
            WindSpeedMs: 3,
            CloudinessPercent: 40,
            PressureHpa: 1015,
            VisibilityMeters: 10000
        );

        var score = _calculator.Calculate(input);

        Assert.True(score >= 90, $"Expected near-ideal conditions to score >= 90, got {score}");
    }

    [Fact]
    public void Calculate_HotAndHumid_ReturnsLowerScore()
    {
        var input = new WeatherInput(
            TempCelsius: 34,
            HumidityPercent: 85,
            WindSpeedMs: 2,
            CloudinessPercent: 90,
            PressureHpa: 1008,
            VisibilityMeters: 10000
        );

        var score = _calculator.Calculate(input);

        Assert.True(score < 60, $"Expected hot/humid conditions to score < 60, got {score}");
    }

    [Fact]
    public void Calculate_ExtremeCold_ReturnsLowScore()
    {
        var input = new WeatherInput(
            TempCelsius: -10,
            HumidityPercent: 60,
            WindSpeedMs: 5,
            CloudinessPercent: 50,
            PressureHpa: 1015,
            VisibilityMeters: 10000
        );

        var score = _calculator.Calculate(input);

        Assert.True(score <= 40, $"Expected extreme cold to score low, got {score}");
    }

    [Fact]
    public void Calculate_ScoreIsAlwaysWithinValidRange()
    {
        // Extreme edge case: very hot, very humid, very windy, fully overcast, low pressure, zero visibility
        var input = new WeatherInput(
            TempCelsius: 50,
            HumidityPercent: 100,
            WindSpeedMs: 40,
            CloudinessPercent: 100,
            PressureHpa: 950,
            VisibilityMeters: 0
        );

        var score = _calculator.Calculate(input);

        Assert.InRange(score, 0, 100);
    }

    [Fact]
    public void Calculate_HigherWindSpeed_ReducesScore()
    {
        var baseInput = new WeatherInput(
            TempCelsius: 21, HumidityPercent: 50, WindSpeedMs: 2,
            CloudinessPercent: 40, PressureHpa: 1015, VisibilityMeters: 10000
        );
        var windyInput = baseInput with { WindSpeedMs = 15 };

        var baseScore = _calculator.Calculate(baseInput);
        var windyScore = _calculator.Calculate(windyInput);

        Assert.True(windyScore < baseScore, "Higher wind speed should reduce comfort score");
    }

    [Fact]
    public void Calculate_PoorVisibility_ReducesScore()
    {
        var baseInput = new WeatherInput(
            TempCelsius: 21, HumidityPercent: 50, WindSpeedMs: 2,
            CloudinessPercent: 40, PressureHpa: 1015, VisibilityMeters: 10000
        );
        var foggyInput = baseInput with { VisibilityMeters = 500 };

        var baseScore = _calculator.Calculate(baseInput);
        var foggyScore = _calculator.Calculate(foggyInput);

        Assert.True(foggyScore < baseScore, "Poor visibility should reduce comfort score");
    }
}