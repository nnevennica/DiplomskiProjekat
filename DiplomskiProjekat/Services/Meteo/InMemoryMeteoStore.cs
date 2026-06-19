using System.Collections.Concurrent;
using DiplomskiProjekat.Models.Meteo;

namespace DiplomskiProjekat.Services.Meteo;

public sealed class InMemoryMeteoStore : IMeteoStore
{
    private sealed class CityState
    {
        public readonly object LockObj = new();

        public List<MeteoPoint> Full = new();
        public List<MeteoPoint> SoFar = new();

        public int Index = -1;
        public MeteoPoint? Current;
    }

    private readonly ConcurrentDictionary<string, CityState> _cities =
        new(StringComparer.OrdinalIgnoreCase);

    public void Initialize(string city, List<MeteoPoint> series)
    {
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required.", nameof(city));

        var state = new CityState
        {
            Full = series ?? new List<MeteoPoint>(),
            SoFar = new List<MeteoPoint>(),
            Index = -1,
            Current = null
        };

        _cities[city] = state;
    }

    public IReadOnlyList<MeteoPoint> GetSeries(string city)
    {
        if (!_cities.TryGetValue(city, out var state)) return Array.Empty<MeteoPoint>();
        lock (state.LockObj) return state.SoFar.ToList();
    }

    public MeteoPoint? GetCurrent(string city)
    {
        if (!_cities.TryGetValue(city, out var state)) return null;
        lock (state.LockObj) return state.Current;
    }

    public void AdvanceAll()
    {
        foreach (var kvp in _cities)
        {
            var state = kvp.Value;
            lock (state.LockObj)
            {
                if (state.Full.Count == 0) continue;

                state.Index++;

                if (state.Index >= state.Full.Count)
                {
                    state.Index = 0;
                    state.SoFar.Clear();
                }

                state.Current = state.Full[state.Index];
                state.SoFar.Add(state.Current);
            }
        }
    }

    public IEnumerable<string> GetCities() => _cities.Keys;
}
