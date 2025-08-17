using Microsoft.AspNetCore.Mvc;
using TicketSell.Api.Models.Responses;

namespace TicketSell.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    [HttpGet]
    public ActionResult<AnalyticsResponse> GetEventAnalytics([FromQuery] long id)
    {
        return Ok(new AnalyticsResponse());
    }
}