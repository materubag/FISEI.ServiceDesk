using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/incidencias/{incidenciaId:long}/[controller]")]
public class ComentariosIncidenciaController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public ComentariosIncidenciaController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromRoute] long incidenciaId)
    {
        var existe = await _db.Incidencias.AnyAsync(i => i.Id == incidenciaId);
        if (!existe) return NotFound();
        var list = await _db.ComentariosIncidencia
            .Where(c => c.IncidenciaId == incidenciaId)
            .OrderByDescending(c => c.Fecha)
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get([FromRoute] long incidenciaId, [FromRoute] long id)
    {
        var c = await _db.ComentariosIncidencia.FirstOrDefaultAsync(x => x.Id == id && x.IncidenciaId == incidenciaId);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long incidenciaId, [FromRoute] long id)
    {
        var c = await _db.ComentariosIncidencia.FirstOrDefaultAsync(x => x.Id == id && x.IncidenciaId == incidenciaId);
        if (c is null) return NotFound();
        _db.ComentariosIncidencia.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}