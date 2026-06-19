namespace DiplomskiProjekat.Models.Meteo;

public sealed class MeteoRawRow
{
    public int Station { get; set; }
    public int Day { get; set; }
    public int Hour { get; set; }

    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double Pressure { get; set; }
}