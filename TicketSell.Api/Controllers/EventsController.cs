using Microsoft.AspNetCore.Mvc;
using TicketSell.Api.Models.Responses;

namespace TicketSell.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<ListEventsResponseItem>> ListEvents(
        [FromQuery] string? query,
        [FromQuery] DateTime? date,
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        return Ok(new List<ListEventsResponseItem>());
    }
}