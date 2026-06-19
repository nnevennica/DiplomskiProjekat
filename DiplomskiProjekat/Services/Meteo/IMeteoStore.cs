using DiplomskiProjekat.Models.Meteo;

namespace DiplomskiProjekat.Services.Meteo;

public interface IMeteoStore
{
    void Initialize(string city, List<MeteoPoint> series);

    IReadOnlyList<MeteoPoint> GetSeries(string city);
    MeteoPoint? GetCurrent(string city);

    void AdvanceAll(); 
    IEnumerable<string> GetCities();
}

