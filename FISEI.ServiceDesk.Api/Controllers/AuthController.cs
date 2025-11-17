using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FISEI.ServiceDesk.Application.DTOs.Auth;
using FISEI.ServiceDesk.Infrastructure.Persistence;
using FISEI.ServiceDesk.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FISEI.ServiceDesk.Domain.Entities;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    private readonly IConfiguration _cfg;

    public AuthController(ServiceDeskDbContext db, IConfiguration cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Correo == dto.Correo && u.Activo);
        if (user is null) return Unauthorized("Credenciales inválidas");
        if (!PasswordHasher.Verify(dto.Password, user.PasswordHash, user.PasswordSalt)) return Unauthorized("Credenciales inválidas");

        var rol = await _db.Roles.FindAsync(user.RolId);
        if (rol is null) return Unauthorized("Rol inválido");

        var jwtCfg = _cfg.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtCfg["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(4);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, rol.Nombre),
            new Claim("name", user.Nombre),
            new Claim("correo", user.Correo)
        };

        var token = new JwtSecurityToken(
            issuer: jwtCfg["Issuer"],
            audience: jwtCfg["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return Ok(new LoginResponseDto
        {
            UserId = user.Id,
            Nombre = user.Nombre,
            Rol = rol.Nombre,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiraUtc = expires
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] LoginRequestDto dto, [FromQuery] string nombre, [FromQuery] int rolId)
    {
        if (await _db.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
            return Conflict("Correo ya registrado");

        var (hash, salt) = PasswordHasher.HashPassword(dto.Password);
        var user = new Usuario
        {
            // Id autogenerado (IDENTITY)
            Nombre = nombre,
            Correo = dto.Correo,
            PasswordHash = hash,
            PasswordSalt = salt,
            RolId = rolId,
            Activo = true,
            FechaRegistro = DateTime.UtcNow
        };
        _db.Usuarios.Add(user);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Register), new { user.Id }, null);
    }
}