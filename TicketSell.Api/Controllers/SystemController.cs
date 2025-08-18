using Microsoft.AspNetCore.Mvc;
using TicketSell.Api.Infrastructure;

namespace TicketSell.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    [HttpPost("reset")]
    public IActionResult ResetDatabase(ITicketSellRepository ticketSellRepository)
    {
        ticketSellRepository.ResetDatabase();
        return Ok();
    }
}