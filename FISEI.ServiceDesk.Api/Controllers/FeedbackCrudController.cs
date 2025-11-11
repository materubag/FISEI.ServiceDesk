using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackCrudController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public FeedbackCrudController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? incidenciaId = null)
    {
        var q = _db.Feedbacks.AsNoTracking().AsQueryable();
        if (incidenciaId.HasValue) q = q.Where(f => f.IncidenciaId == incidenciaId.Value);
        var list = await q.OrderByDescending(f => f.Fecha).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id)
    {
        var e = await _db.Feedbacks.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Domain.Entities.FeedbackIncidencia dto)
    {
        _db.Feedbacks.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] Domain.Entities.FeedbackIncidencia dto)
    {
        var e = await _db.Feedbacks.FindAsync(id);
        if (e is null) return NotFound();
        e.Puntuacion = dto.Puntuacion;
        e.Comentario = dto.Comentario;
        // UsuarioId e IncidenciaId normalmente no deberían cambiar
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var e = await _db.Feedbacks.FindAsync(id);
        if (e is null) return NotFound();
        _db.Feedbacks.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}