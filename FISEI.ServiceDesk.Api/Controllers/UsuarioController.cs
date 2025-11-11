using System.ComponentModel.DataAnnotations;
using FISEI.ServiceDesk.Infrastructure.Persistence;
using FISEI.ServiceDesk.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public UsuariosController(ServiceDeskDbContext db) => _db = db;

    public record UsuarioDto(Guid Id, string Nombre, string Correo, int RolId, bool Activo, DateTime FechaRegistro);
    public class UsuarioCreateDto
    {
        [Required] public string Nombre { get; set; } = default!;
        [Required, EmailAddress] public string Correo { get; set; } = default!;
        [Required, MinLength(6)] public string Password { get; set; } = default!;
        [Required] public int RolId { get; set; }
        public bool Activo { get; set; } = true;
    }
    public class UsuarioUpdateDto
    {
        [Required] public string Nombre { get; set; } = default!;
        [Required, EmailAddress] public string Correo { get; set; } = default!;
        [Required] public int RolId { get; set; }
        [Required] public bool Activo { get; set; }
        public string? NewPassword { get; set; }
    }

    // GET /api/usuarios?page=1&pageSize=50&search=mateo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize is <= 0 or > 200 ? 50 : pageSize;

        var query = _db.Usuarios.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => u.Nombre.Contains(search) || u.Correo.Contains(search));
        }

        var data = await query
            .OrderBy(u => u.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UsuarioDto(u.Id, u.Nombre, u.Correo, u.RolId, u.Activo, u.FechaRegistro))
            .ToListAsync();

        return Ok(data);
    }

    // GET /api/usuarios/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> GetById([FromRoute] Guid id)
    {
        var u = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (u is null) return NotFound();
        return Ok(new UsuarioDto(u.Id, u.Nombre, u.Correo, u.RolId, u.Activo, u.FechaRegistro));
    }

    // POST /api/usuarios
    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Create([FromBody] UsuarioCreateDto dto)
    {
        if (await _db.Usuarios.AnyAsync(x => x.Correo == dto.Correo)) return Conflict("Correo ya existe.");
        var (hash, salt) = PasswordHasher.HashPassword(dto.Password);
        var entity = new Domain.Entities.Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Correo = dto.Correo,
            PasswordHash = hash,
            PasswordSalt = salt,
            RolId = dto.RolId,
            Activo = dto.Activo,
            FechaRegistro = DateTime.UtcNow
        };
        _db.Usuarios.Add(entity);
        await _db.SaveChangesAsync();
        var result = new UsuarioDto(entity.Id, entity.Nombre, entity.Correo, entity.RolId, entity.Activo, entity.FechaRegistro);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    // PUT /api/usuarios/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UsuarioUpdateDto dto)
    {
        var u = await _db.Usuarios.FindAsync(id);
        if (u is null) return NotFound();

        if (!string.Equals(u.Correo, dto.Correo, StringComparison.OrdinalIgnoreCase)
            && await _db.Usuarios.AnyAsync(x => x.Correo == dto.Correo))
            return Conflict("Correo ya existe.");

        u.Nombre = dto.Nombre;
        u.Correo = dto.Correo;
        u.RolId = dto.RolId;
        u.Activo = dto.Activo;

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            var (hash, salt) = PasswordHasher.HashPassword(dto.NewPassword);
            u.PasswordHash = hash;
            u.PasswordSalt = salt;
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/usuarios/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var u = await _db.Usuarios.FindAsync(id);
        if (u is null) return NotFound();

        _db.Usuarios.Remove(u);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}