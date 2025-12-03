using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EscalacionesController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public EscalacionesController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? incidenciaId = null)
    {
        var q = _db.Escalaciones.AsNoTracking().AsQueryable();
        if (incidenciaId.HasValue) q = q.Where(e => e.IncidenciaId == incidenciaId.Value);
        var list = await q.OrderByDescending(e => e.Fecha).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id)
    {
        var e = await _db.Escalaciones.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var e = await _db.Escalaciones.FindAsync(id);
        if (e is null) return NotFound();
        _db.Escalaciones.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}