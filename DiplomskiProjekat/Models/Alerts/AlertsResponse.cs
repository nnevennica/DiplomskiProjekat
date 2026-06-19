namespace DiplomskiProjekat.Models.Alerts;

public class AlertsResponse
{
    public string City { get; set; } = "";
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public List<AlertItem> Items { get; set; } = new();
}

public class AlertItem
{
    public string Id { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public string SimTimeLabel { get; set; } = "";

    public string Type { get; set; } = "";
    public string Level { get; set; } = "";
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
}