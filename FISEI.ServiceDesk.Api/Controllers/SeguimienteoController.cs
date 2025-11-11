using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FISEI.ServiceDesk.Infrastructure.Persistence;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/incidencias/{incidenciaId:long}/[controller]")]
public class SeguimientoController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public SeguimientoController(ServiceDeskDbContext db) => _db = db;

    // GET /api/incidencias/{id}/seguimiento
    [HttpGet]
    public async Task<IActionResult> Get(long incidenciaId)
    {
        var existe = await _db.Incidencias.AnyAsync(i => i.Id == incidenciaId);
        if (!existe) return NotFound();

        var data = await _db.Seguimientos
            .Where(s => s.IncidenciaId == incidenciaId)
            .OrderByDescending(s => s.Fecha)
            .Select(s => new
            {
                s.Id,
                s.EstadoAnteriorId,
                s.EstadoNuevoId,
                s.Comentario,
                s.UsuarioId,
                s.Fecha
            }).ToListAsync();

        return Ok(data);
    }
}