using Microsoft.AspNetCore.Mvc;

namespace TicketSell.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    [HttpPost("reset")]
    public IActionResult ResetDatabase()
    {
        return Ok();
    }
}