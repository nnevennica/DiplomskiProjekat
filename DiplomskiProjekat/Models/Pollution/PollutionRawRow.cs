namespace DiplomskiProjekat.Models.Pollution;

public sealed class PollutionRawRow
{
    public int Station { get; set; }
    public int Day { get; set; }
    public int Hour { get; set; }

    public double Pm25 { get; set; }
    public double Pm10 { get; set; }
    public double O3 { get; set; }
}
