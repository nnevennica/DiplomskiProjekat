using System.Globalization;
using DiplomskiProjekat.Models.Meteo;

namespace DiplomskiProjekat.Services.Meteo;

public sealed class MeteoDataService
{
    public List<MeteoPoint> LoadHourlyAverages(string csvPath, int day)
    {
        var raw = LoadRaw(csvPath);

        return raw
            .Where(r => r.Day == day)
            .GroupBy(r => r.Hour)
            .OrderBy(g => g.Key)
            .Select(g => new MeteoPoint
            {
                Label = $"{g.Key:00}:00",
                TempAvg = Math.Round(g.Average(x => x.Temperature), 2),
                HumAvg = Math.Round(g.Average(x => x.Humidity), 2),
                PressAvg = Math.Round(g.Average(x => x.Pressure), 2),
            })
            .ToList();
    }

    private static List<MeteoRawRow> LoadRaw(string path)
    {
        var rows = new List<MeteoRawRow>();

        foreach (var line in File.ReadLines(path))
        {
            var s = line.Trim();

            if (string.IsNullOrWhiteSpace(s)) continue;
            if (s.StartsWith("#")) continue;

            var parts = s.Split(';');
            if (parts.Length < 6) continue;

            var stationText = parts[0].Trim();

            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var day)) continue;
            if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var hour)) continue;

            if (!double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var temp)) continue;
            if (!double.TryParse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture, out var hum)) continue;
            if (!double.TryParse(parts[5], NumberStyles.Float, CultureInfo.InvariantCulture, out var press)) continue;

            rows.Add(new MeteoRawRow
            {
                Station = ParseStationNumber(stationText),
                Day = day,
                Hour = hour,
                Temperature = temp,
                Humidity = hum,
                Pressure = press
            });
        }

        return rows;
    }

    private static int ParseStationNumber(string value)
    {
        var digits = new string(value.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var n) ? n : 0;
    }
}
