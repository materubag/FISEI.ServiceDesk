using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionesCrudController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public NotificacionesCrudController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? usuarioId = null, [FromQuery] bool? leida = null)
    {
        var q = _db.Notificaciones.AsNoTracking().AsQueryable();
        if (usuarioId.HasValue) q = q.Where(n => n.UsuarioDestinoId == usuarioId.Value);
        if (leida.HasValue) q = q.Where(n => n.Leida == leida.Value);
        var list = await q.OrderByDescending(n => n.Fecha).Take(200).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id)
    {
        var e = await _db.Notificaciones.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Domain.Entities.Notificacion dto)
    {
        _db.Notificaciones.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] Domain.Entities.Notificacion dto)
    {
        var e = await _db.Notificaciones.FindAsync(id);
        if (e is null) return NotFound();
        e.UsuarioDestinoId = dto.UsuarioDestinoId;
        e.Tipo = dto.Tipo;
        e.Referencia = dto.Referencia;
        e.Mensaje = dto.Mensaje;
        e.Leida = dto.Leida;
        // Fecha generalmente es solo lectura, tocar si quieres
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var e = await _db.Notificaciones.FindAsync(id);
        if (e is null) return NotFound();
        _db.Notificaciones.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}