namespace DiplomskiProjekat.Services.Pollution;

public sealed class PollutionSimulationHostedService : BackgroundService
{
    private readonly IPollutionStore _store;

    public PollutionSimulationHostedService(IPollutionStore store)
    {
        _store = store;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var city in _store.GetCities())
            {
                _store.Advance(city);
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
