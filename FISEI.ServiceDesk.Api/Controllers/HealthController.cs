using Microsoft.AspNetCore.Mvc;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "OK", utc = DateTime.UtcNow });
}