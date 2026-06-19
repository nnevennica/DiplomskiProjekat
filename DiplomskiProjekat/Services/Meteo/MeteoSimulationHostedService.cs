using DiplomskiProjekat.Services.Meteo;

namespace DiplomskiProjekat.Services.Meteo;

public sealed class MeteoSimulationHostedService : BackgroundService
{
    private readonly IMeteoStore _store;

    public MeteoSimulationHostedService(IMeteoStore store)
    {
        _store = store;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(300, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            _store.AdvanceAll();
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // 5s = 1h
        }
    }
}