using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TicketSell.Api.Infrastructure;
using TicketSell.Api.Infrastructure.Extensions;
using TicketSell.Api.Infrastructure.Options;
using TicketSell.Api.Models.Requests;
using TicketSell.Api.Models.Responses;

namespace TicketSell.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(
    ITicketSellRepository repository,
    IOptions<TicketSellSettings> settings) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ListBookingsResponseItem>>> ListBookings()
    {
        var user = HttpContext.GetCurrentCustomer();
        return Ok(await repository.GetBookings(user.UserId));
    }

    [HttpGet("/{id:long}")]
    public async Task<ActionResult<ListBookingsResponseItem>> Get([FromRoute] long id)
    {
        var user = HttpContext.GetCurrentCustomer();
        var booking = await repository.GetBooking(user.UserId, id);
        return Ok(new ListBookingsResponseItem
        {
            Id = booking.Id,
            EventId = booking.EventId,
            Seats = booking.Seats.Select(x => new ListEventsResponseItemSeat {Id = x}).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<CreateBookingResponse>> CreateBooking([FromBody] CreateBookingRequest request)
    {
        var user = HttpContext.GetCurrentCustomer();
        var bookingId = await repository.AddBooking(user.UserId, request.EventId);
        return Created(
            $"{settings.Value.HostName}/{bookingId}",
            new CreateBookingResponse { Id = bookingId });
    }

    [HttpPatch("initiatePayment")]
    public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequest request)
    {
        var user = HttpContext.GetCurrentCustomer();
        var booking = await repository.InitiatePayment(user.UserId, request.BookingId);
        return Redirect($"{settings.Value.PaymentUrl}");
    }

    [HttpPatch("cancel")]
    public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequest request)
    {
        var user = HttpContext.GetCurrentCustomer();
        await repository.CancelPayment(user.UserId, request.BookingId);
        return Ok();
    }
}