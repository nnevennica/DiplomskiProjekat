using DiplomskiProjekat.Models.Alerts;
using DiplomskiProjekat.Services.Meteo;
using DiplomskiProjekat.Services.Pollution;

namespace DiplomskiProjekat.Services.Alerts;

public class AlertsService
{
    private readonly IMeteoStore _meteo;
    private readonly IPollutionStore _pollution;

    public AlertsService(IMeteoStore meteo, IPollutionStore pollution)
    {
        _meteo = meteo;
        _pollution = pollution;
    }

    public AlertsResponse GetAlerts(string city)
    {
        var res = new AlertsResponse
        {
            City = city,
            TimestampUtc = DateTime.UtcNow
        };

        var meteoAlerts = GetMeteoAlerts(city);
        var pollutionAlerts = GetPollutionAlerts(city);

        res.Items.AddRange(meteoAlerts.Items);
        res.Items.AddRange(pollutionAlerts.Items);

        return res;
    }

    public AlertsResponse GetMeteoAlerts(string city)
    {
        var now = DateTime.UtcNow;

        var res = new AlertsResponse
        {
            City = city,
            TimestampUtc = now
        };

        var m = _meteo.GetCurrent(city);
        if (m == null) return res;

        var simTimeLabel = m.Label;

        var icyRoadRisk = m.TempAvg < 5;
        var highTemperature = m.TempAvg > 27;
        var highHumidity = m.HumAvg > 80;
        var highPressure = m.PressAvg > 1020;

        if (icyRoadRisk)
        {
            res.Items.Add(new AlertItem
            {
                Id = $"{city}-ice-{m.Label}",
                CreatedAtUtc = now,
                SimTimeLabel = simTimeLabel,
                Type = "meteo",
                Level = "warn",
                Title = "Moguća poledica",
                Message = $"Temperatura je {m.TempAvg:0.#}°C. Postoji mogućnost poledice i klizavih puteva."
            });
        }

        if (highTemperature)
        {
            res.Items.Add(new AlertItem
            {
                Id = $"{city}-heat-{m.Label}",
                CreatedAtUtc = now,
                SimTimeLabel = simTimeLabel,
                Type = "meteo",
                Level = "warn",
                Title = "Povišena temperatura",
                Message = $"Temperatura je {m.TempAvg:0.#}°C. Očekuje se jako sunce i povećano toplotno opterećenje."
            });
        }

        if (highHumidity)
        {
            res.Items.Add(new AlertItem
            {
                Id = $"{city}-humidity-{m.Label}",
                CreatedAtUtc = now,
                SimTimeLabel = simTimeLabel,
                Type = "meteo",
                Level = "info",
                Title = "Povišena vlažnost vazduha",
                Message = $"Vlažnost vazduha iznosi {m.HumAvg:0.#}%, što je iznad uobičajenih vrednosti."
            });
        }

        if (highPressure)
        {
            res.Items.Add(new AlertItem
            {
                Id = $"{city}-pressure-{m.Label}",
                CreatedAtUtc = now,
                SimTimeLabel = simTimeLabel,
                Type = "meteo",
                Level = "info",
                Title = "Pritisak vazduha iznad normalnih vrednosti",
                Message = $"Pritisak vazduha iznosi {m.PressAvg:0.#} hPa, što je iznad normalnih vrednosti."
            });
        }

        return res;
    }

    public AlertsResponse GetPollutionAlerts(string city)
    {
        var now = DateTime.UtcNow;

        var res = new AlertsResponse
        {
            City = city,
            TimestampUtc = now
        };

        var p = _pollution.GetCurrent(city);
        if (p == null) return res;

        var simTimeLabel = p.Label;

        const double pm25Limit = 15.0;
        const double pm10Limit = 45.0;
        const double o3Limit = 100.0;

        var exceeded = new List<string>();

        if (p.Pm25Avg > pm25Limit)
            exceeded.Add($"PM2.5 = {p.Pm25Avg:0.#}");

        if (p.Pm10Avg > pm10Limit)
            exceeded.Add($"PM10 = {p.Pm10Avg:0.#}");

        if (p.O3Avg > o3Limit)
            exceeded.Add($"O₃ = {p.O3Avg:0.#}");

        if (exceeded.Count > 0)
        {
            res.Items.Add(new AlertItem
            {
                Id = $"{city}-pollution-{p.Label}",
                CreatedAtUtc = now,
                SimTimeLabel = simTimeLabel,
                Type = "pollution",
                Level = "warn",
                Title = "Kvalitet vazduha je pogoršan",
                Message = $"Povišene su vrednosti: {string.Join(", ", exceeded)}. Preporučuje se oprez pri dužem boravku napolju."
            });
        }

        return res;
    }
}