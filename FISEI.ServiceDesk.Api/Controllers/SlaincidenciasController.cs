using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/sla/incidencias")]
public class SlaIncidenciasController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public SlaIncidenciasController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? incidenciaId = null)
    {
        var q = _db.SLA_Incidencias.AsNoTracking().AsQueryable();
        if (incidenciaId.HasValue) q = q.Where(s => s.IncidenciaId == incidenciaId.Value);
        var list = await q.OrderByDescending(s => s.CreadoUtc).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id)
    {
        var e = await _db.SLA_Incidencias.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var e = await _db.SLA_Incidencias.FindAsync(id);
        if (e is null) return NotFound();
        _db.SLA_Incidencias.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}