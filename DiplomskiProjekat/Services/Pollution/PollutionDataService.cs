using System.Globalization;
using DiplomskiProjekat.Models.Pollution;

namespace DiplomskiProjekat.Services.Pollution;

public sealed class PollutionDataService
{
    public List<PollutionPoint> LoadHourlyAverages(string csvPath, int day)
    {
        var raw = LoadRaw(csvPath);

        return raw
            .Where(r => r.Day == day)
            .GroupBy(r => r.Hour)
            .OrderBy(g => g.Key)
            .Select(g => new PollutionPoint
            {
                Label = $"{g.Key:00}:00",
                Pm25Avg = Math.Round(g.Average(x => x.Pm25), 2),
                Pm10Avg = Math.Round(g.Average(x => x.Pm10), 2),
                O3Avg = Math.Round(g.Average(x => x.O3), 2),
            })
            .ToList();
    }

    private static List<PollutionRawRow> LoadRaw(string path)
    {
        var rows = new List<PollutionRawRow>();

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

            if (!double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var pm25)) continue;
            if (!double.TryParse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture, out var pm10)) continue;
            if (!double.TryParse(parts[5], NumberStyles.Float, CultureInfo.InvariantCulture, out var o3)) continue;

            rows.Add(new PollutionRawRow
            {
                Station = ParseStationNumber(stationText),
                Day = day,
                Hour = hour,
                Pm25 = pm25,
                Pm10 = pm10,
                O3 = o3
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
