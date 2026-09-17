namespace WeatherAnalytics.Api.Services;

using WeatherAnalytics.Api.Models;

public interface IComfortIndexCalculator
{
    double Calculate(WeatherInput input);
}

public class ComfortIndexCalculator : IComfortIndexCalculator
{
    public double Calculate(WeatherInput input)
    {
        double scoreDI = ScoreDiscomfortIndex(input.TempCelsius, input.HumidityPercent);
        double scoreWind = ScoreWind(input.WindSpeedMs);
        double scoreDew = ScoreDewPoint(input.TempCelsius, input.HumidityPercent);
        double scoreCloud = ScoreBand(input.CloudinessPercent, idealMid: 40, halfWidth: 20, decay: 0.5);
        double scorePressure = ScoreBand(input.PressureHpa, idealMid: 1015, halfWidth: 5, decay: 0.5);
        double scoreVisibility = ScoreVisibility(input.VisibilityMeters);

        double total = 0.60 * scoreDI
                    + 0.15 * scoreWind
                    + 0.10 * scoreDew
                    + 0.05 * scoreCloud
                    + 0.05 * scorePressure
                    + 0.05 * scoreVisibility;
        
        return Math.Clamp(Math.Round(total, 1), 0, 100);
    }

    // Thom's Discomfort Index
    private static double ScoreDiscomfortIndex(double tempC, double humidity)
    {
        double di = tempC - 0.55 * (1 - 0.01 * humidity) * (tempC - 14.5);
        return Math.Max(0, 100 - Math.Abs(di - 21) * 6);
    }

    // static ideal range
    private static double ScoreWind(double windSpeedMs)
    {
        double excess = Math.Max(0, windSpeedMs - 5);
        return Math.Max(0, 100 - excess * 8);
    }

    // Dew point
    private static double ScoreDewPoint(double tempC, double humidity)
    {
        double alpha = Math.Log(humidity / 100.0) + (17.27 * tempC) / (237.3 + tempC);
        double dewPoint = (237.3 * alpha) / (17.27 - alpha);

        if (dewPoint <= 15) return 100;
        return Math.Max(0, 100 - (dewPoint - 15) * 5);
    }

    private static double ScoreBand(double value, double idealMid, double halfWidth, double decay)
    {
        double distance = Math.Max(0, Math.Abs(value - idealMid) - halfWidth);
        return Math.Max(0, 100 - distance * decay);
    }

    private static double ScoreVisibility(double visibilityMeters)
    {
        if (visibilityMeters >= 8000) return 100;
        return Math.Max(0, 100 - (8000 - visibilityMeters) * (100.0 / 8000.0));
    }
}