using DiplomskiProjekat.Models.Alerts;
using DiplomskiProjekat.Services.Alerts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomskiProjekat.Controllers;

[ApiController]
[Route("api/alerts")]
[Authorize]
public sealed class AlertsController : ControllerBase
{
    private readonly AlertsService _alerts;

    public AlertsController(AlertsService alerts)
    {
        _alerts = alerts;
    }

    [HttpGet("{city}")]
    public ActionResult<AlertsResponse> Get(string city)
        => Ok(_alerts.GetAlerts(city));
}