using DiplomskiProjekat.Services.Meteo;
using DiplomskiProjekat.Services.Pollution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomskiProjekat.Controllers;

[ApiController]
[Route("api/cities")]
public sealed class CitiesController : ControllerBase
{
    private readonly IMeteoStore _meteo;
    private readonly IPollutionStore _pollution;

    public CitiesController(IMeteoStore meteo, IPollutionStore pollution)
    {
        _meteo = meteo;
        _pollution = pollution;
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetCities()
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var c in _meteo.GetCities()) set.Add(c);
        foreach (var c in _pollution.GetCities()) set.Add(c);
        return Ok(set.OrderBy(x => x).ToList());
    }
}
