using FISEI.ServiceDesk.Domain.Entities;
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

    // GET /api/notificacionescrud?usuarioId=&leida=&page=&pageSize=
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? usuarioId = null,
        [FromQuery] bool? leida = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _db.Notificaciones.AsNoTracking().AsQueryable();

        if (usuarioId.HasValue)
            query = query.Where(n => n.UsuarioDestinoId == usuarioId.Value);

        if (leida.HasValue)
            query = query.Where(n => n.Leida == leida.Value);

        page = page <= 0 ? 1 : page;
        pageSize = pageSize is <= 0 or > 200 ? 50 : pageSize;

        var total = await query.CountAsync();

        var data = await query
            .OrderByDescending(n => n.Fecha)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Opcional: devolver total en header
        Response.Headers["X-Total-Count"] = total.ToString();

        return Ok(data);
    }

    // GET /api/notificacionescrud/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.Notificaciones.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    // POST /api/notificacionescrud
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Notificacion dto)
    {
        // Fecha la puede establecer el default SQL (SYSUTCDATETIME) o aquí:
        if (dto.Fecha == default) dto.Fecha = DateTime.UtcNow;

        _db.Notificaciones.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    // PUT /api/notificacionescrud/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Notificacion dto)
    {
        var e = await _db.Notificaciones.FindAsync(id);
        if (e is null) return NotFound();

        e.UsuarioDestinoId = dto.UsuarioDestinoId;
        e.Tipo = dto.Tipo;
        e.Referencia = dto.Referencia;
        e.Mensaje = dto.Mensaje;
        e.Leida = dto.Leida;
        // e.Fecha = dto.Fecha; // normalmente no se cambia

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/notificacionescrud/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Notificaciones.FindAsync(id);
        if (e is null) return NotFound();

        _db.Notificaciones.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PATCH /api/notificacionescrud/{id}/leer
    [HttpPatch("{id:int}/leer")]
    public async Task<IActionResult> MarkRead(int id)
    {
        var e = await _db.Notificaciones.FindAsync(id);
        if (e is null) return NotFound();
        if (!e.Leida)
        {
            e.Leida = true;
            await _db.SaveChangesAsync();
        }
        return NoContent();
    }

    // POST /api/notificacionescrud/leer-todas?usuarioId=
    [HttpPost("leer-todas")]
    public async Task<IActionResult> MarkAllRead([FromQuery] int usuarioId)
    {
        var rows = await _db.Notificaciones
            .Where(n => n.UsuarioDestinoId == usuarioId && !n.Leida)
            .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.Leida, true));
        return Ok(new { updated = rows });
    }
}