using Microsoft.AspNetCore.Mvc;
using TicketSell.Api.Models;

namespace TicketSell.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    [HttpGet("success")]
    public IActionResult NotifyPaymentCompleted([FromQuery] long orderId)
    {
        return Ok();
    }

    [HttpGet("fail")]
    public IActionResult NotifyPaymentFailed([FromQuery] long orderId)
    {
        return Ok();
    }

    [HttpPost("notifications")]
    public IActionResult OnPaymentUpdates([FromBody] PaymentNotificationPayload payload)
    {
        return Ok();
    }
}