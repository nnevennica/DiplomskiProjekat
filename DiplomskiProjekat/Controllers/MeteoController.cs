using DiplomskiProjekat.Services.Meteo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomskiProjekat.Controllers;

[ApiController]
[Route("api/meteo")]
[Authorize]
public sealed class MeteoController : ControllerBase
{
    private readonly IMeteoStore _store;

    public MeteoController(IMeteoStore store)
    {
        _store = store;
    }

    [HttpGet("cities")]
    public IActionResult GetCities()
        => Ok(_store.GetCities());

    [HttpGet("{city}/series")]
    public IActionResult GetSeries(string city)
        => Ok(_store.GetSeries(city));

    [HttpGet("{city}/current")]
    public IActionResult GetCurrent(string city)
        => Ok(_store.GetCurrent(city));
}
