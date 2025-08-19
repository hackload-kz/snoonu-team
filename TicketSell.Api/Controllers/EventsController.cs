using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSell.Api.Infrastructure;
using TicketSell.Api.Models.Responses;

namespace TicketSell.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(ITicketSellRepository repository) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ListEventsResponseItem>>> ListEvents(
        [FromQuery] string? query,
        [FromQuery] DateTimeOffset? date,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var events = await repository.GetEvents(query, date, page, pageSize, cancellationToken);

        return Ok(events.Select(x => new ListEventsResponseItem
        {
            Id = x.Id,
            Title = x.Title
        }));
    }
}