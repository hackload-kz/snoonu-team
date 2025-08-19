using Microsoft.AspNetCore.Mvc;
using TicketSell.Api.Infrastructure;
using TicketSell.Api.Models;

namespace TicketSell.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(ITicketSellRepository repository) : ControllerBase
{
    [HttpGet("success")]
    public async Task<IActionResult> NotifyPaymentCompleted([FromQuery] long orderId)
    {
        await repository.ApprovePayment(orderId);
        return Ok();
    }

    [HttpGet("fail")]
    public async Task<IActionResult>  NotifyPaymentFailed([FromQuery] long orderId)
    {
        await repository.CancelPayment(orderId);
        return Ok();
    }

    [HttpPost("notifications")]
    public IActionResult OnPaymentUpdates([FromBody] PaymentNotificationPayload payload)
    {
        return Ok();
    }
}