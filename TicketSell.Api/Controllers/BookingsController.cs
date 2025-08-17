using Microsoft.AspNetCore.Mvc;
using TicketSell.Api.Models.Requests;
using TicketSell.Api.Models.Responses;

namespace TicketSell.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<ListBookingsResponseItem>> ListBookings()
    {
        return Ok(new List<ListBookingsResponseItem>());
    }

    [HttpPost]
    public ActionResult<CreateBookingResponse> CreateBooking([FromBody] CreateBookingRequest request)
    {
        return Created(string.Empty, new CreateBookingResponse { Id = 1 });
    }

    [HttpPatch("initiatePayment")]
    public IActionResult InitiatePayment([FromBody] InitiatePaymentRequest request)
    {
        return StatusCode(302);
    }

    [HttpPatch("cancel")]
    public IActionResult CancelBooking([FromBody] CancelBookingRequest request)
    {
        return Ok();
    }
}