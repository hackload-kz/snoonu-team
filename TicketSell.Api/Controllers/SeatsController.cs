using Microsoft.AspNetCore.Mvc;
using TicketSell.Api.Models;
using TicketSell.Api.Models.Requests;
using TicketSell.Api.Models.Responses;

namespace TicketSell.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<ListSeatsResponseItem>> ListSeats(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery(Name = "event_id")] long eventId,
        [FromQuery] int? row,
        [FromQuery] SeatStatusEnum? status)
    {
        return Ok(new List<ListSeatsResponseItem>());
    }

    [HttpPatch("select")]
    public IActionResult SelectSeat([FromBody] SelectSeatRequest request)
    {
        return Ok();
    }

    [HttpPatch("release")]
    public IActionResult ReleaseSeat([FromBody] ReleaseSeatRequest request)
    {
        return Ok();
    }
}
