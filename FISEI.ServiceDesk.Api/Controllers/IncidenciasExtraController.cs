using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

// Extiende Incidencias con CRUD general
[ApiController]
[Route("api/incidencias")]
public class IncidenciasExtraController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public IncidenciasExtraController(ServiceDeskDbContext db) => _db = db;

    // GET /api/incidencias?estadoId=&prioridadId=&servicioId=&search=&page=&pageSize=
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? estadoId, [FromQuery] int? prioridadId,
        [FromQuery] int? servicioId, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize is <= 0 or > 200 ? 50 : pageSize;

        var q = _db.Incidencias.AsNoTracking().Where(i => i.Activo);
        if (estadoId.HasValue) q = q.Where(i => i.EstadoId == estadoId.Value);
        if (prioridadId.HasValue) q = q.Where(i => i.PrioridadId == prioridadId.Value);
        if (servicioId.HasValue) q = q.Where(i => i.ServicioId == servicioId.Value);
        if (!string.IsNullOrWhiteSpace(search)) 
            q = q.Where(i => EF.Functions.Like(i.Titulo, $"%{search}%") || EF.Functions.Like(i.Descripcion, $"%{search}%"));

        var list = await q
            .OrderByDescending(i => i.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new
            {
                i.Id,
                Codigo = $"INC-{i.Id:000000}",
                i.Titulo,
                i.Descripcion,
                i.EstadoId,
                i.PrioridadId,
                i.ServicioId,
                i.CreadorId,
                i.TecnicoAsignadoId,
                i.FechaCreacion
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET /api/incidencias/{id}
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById([FromRoute] long id)
    {
        var i = await _db.Incidencias.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.Activo);
        if (i is null) return NotFound();

        string? tecnicoNombre = null;
        if (i.TecnicoAsignadoId.HasValue)
        {
            tecnicoNombre = await _db.Usuarios
                .Where(u => u.Id == i.TecnicoAsignadoId.Value)
                .Select(u => u.Nombre)
                .FirstOrDefaultAsync();
        }

        return Ok(new
        {
            i.Id,
            Codigo = $"INC-{i.Id:000000}",
            i.Titulo,
            i.Descripcion,
            i.EstadoId,
            i.PrioridadId,
            i.ServicioId,
            i.CreadorId,
            i.TecnicoAsignadoId,
            TecnicoAsignadoNombre = tecnicoNombre,
            i.FechaCreacion,
            i.FechaUltimoCambio,
            i.FechaResolucion,
            i.Cerrada
        });
    }

    // PUT /api/incidencias/{id}
    public class IncidenciaUpdateDto
    {
        public string Titulo { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
        public int PrioridadId { get; set; }
        public int ServicioId { get; set; }
        public int? TecnicoAsignadoId { get; set; }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] IncidenciaUpdateDto dto)
    {
        var i = await _db.Incidencias.FindAsync(id);
        if (i is null || !i.Activo) return NotFound();

        i.Titulo = dto.Titulo;
        i.Descripcion = dto.Descripcion;
        i.PrioridadId = dto.PrioridadId;
        i.ServicioId = dto.ServicioId;
        i.TecnicoAsignadoId = dto.TecnicoAsignadoId;
        i.FechaUltimoCambio = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/incidencias/{id} (borrado lógico)
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var i = await _db.Incidencias.FindAsync(id);
        if (i is null || !i.Activo) return NotFound();
        i.Activo = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}