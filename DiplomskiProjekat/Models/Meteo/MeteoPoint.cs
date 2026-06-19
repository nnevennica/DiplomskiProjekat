namespace DiplomskiProjekat.Models.Meteo
{
    public class MeteoPoint
    {
        public int Day { get; set; }
        public int Hour { get; set; } 
        public string Label { get; set; } = "";
        public double TempAvg { get; set; }
        public double HumAvg { get; set; }
        public double PressAvg { get; set; }
    }
}
