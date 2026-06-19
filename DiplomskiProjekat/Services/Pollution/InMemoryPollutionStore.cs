using System.Collections.Concurrent;
using DiplomskiProjekat.Models.Pollution;

namespace DiplomskiProjekat.Services.Pollution;

public sealed class InMemoryPollutionStore : IPollutionStore
{
    private sealed class CityState
    {
        public List<PollutionPoint> Series { get; init; } = new();
        public int Index { get; set; } = 0;
    }

    private readonly ConcurrentDictionary<string, CityState> _cities = new(StringComparer.OrdinalIgnoreCase);

    public void Initialize(string city, List<PollutionPoint> series)
        => _cities[city] = new CityState { Series = series, Index = 0 };

    public IReadOnlyList<PollutionPoint> GetSeries(string city)
        => _cities.TryGetValue(city, out var state) ? state.Series : Array.Empty<PollutionPoint>();

    public PollutionPoint? GetCurrent(string city)
    {
        if (!_cities.TryGetValue(city, out var state) || state.Series.Count == 0) return null;
        var i = Math.Clamp(state.Index, 0, state.Series.Count - 1);
        return state.Series[i];
    }

    public PollutionPoint? Advance(string city)
    {
        if (!_cities.TryGetValue(city, out var state) || state.Series.Count == 0) return null;

        state.Index++;
        if (state.Index >= state.Series.Count) state.Index = state.Series.Count - 1; 
        return state.Series[state.Index];
    }

    public IEnumerable<string> GetCities() => _cities.Keys;
}
