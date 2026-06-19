using DiplomskiProjekat.Services.Pollution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomskiProjekat.Controllers;

[ApiController]
[Route("api/pollution")]
public sealed class PollutionController : ControllerBase
{
    private readonly IPollutionStore _store;

    public PollutionController(IPollutionStore store)
    {
        _store = store;
    }

    [HttpGet("{city}/series")]
    [Authorize]
    public IActionResult GetSeries(string city)
        => Ok(_store.GetSeries(city));

    [HttpGet("{city}/current")]
    [Authorize]
    public IActionResult GetCurrent(string city)
        => Ok(_store.GetCurrent(city));
}
