using FISEI.ServiceDesk.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/auth/dev-hash")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AuthDevController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    public AuthDevController(IWebHostEnvironment env) => _env = env;

    public record DevHashRequest(string Password);

    [HttpPost]
    public ActionResult Generar([FromBody] DevHashRequest req)
    {
        if (!_env.IsDevelopment()) return NotFound(); // evita uso en prod
        if (string.IsNullOrWhiteSpace(req.Password)) return BadRequest("Password requerido.");

        var (hash, salt) = PasswordHasher.HashPassword(req.Password);
        return Ok(new { hash, salt });
    }
}