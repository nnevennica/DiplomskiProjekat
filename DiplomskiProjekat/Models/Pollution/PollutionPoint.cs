namespace DiplomskiProjekat.Models.Pollution;

public sealed class PollutionPoint
{
    public string Label { get; init; } = "";

    public double Pm25Avg { get; init; }
    public double Pm10Avg { get; init; }
    public double O3Avg { get; init; }
}
