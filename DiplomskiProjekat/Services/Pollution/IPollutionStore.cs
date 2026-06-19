using DiplomskiProjekat.Models.Pollution;

namespace DiplomskiProjekat.Services.Pollution;

public interface IPollutionStore
{
    void Initialize(string city, List<PollutionPoint> series);
    IReadOnlyList<PollutionPoint> GetSeries(string city);
    PollutionPoint? GetCurrent(string city);
    PollutionPoint? Advance(string city);
    IEnumerable<string> GetCities();
}
