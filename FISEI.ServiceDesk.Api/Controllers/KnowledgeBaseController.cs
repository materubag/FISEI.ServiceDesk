using FISEI.ServiceDesk.Domain.Entities;
using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/kb")]
public class KnowledgeBaseController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public KnowledgeBaseController(ServiceDeskDbContext db) => _db = db;

    // GET /api/kb?search=&servicioId=&laboratorioId=&autorId=&page=&pageSize=
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? search = null,
        [FromQuery] int? servicioId = null,
        [FromQuery] int? laboratorioId = null,
        [FromQuery] int? autorId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var q = _db.ArticulosConocimiento.AsNoTracking().Where(a => a.Activo);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(a =>
                a.Titulo.Contains(s) ||
                a.Contenido.Contains(s) ||
                (a.Etiquetas != null && a.Etiquetas.Contains(s)));
        }
        if (servicioId.HasValue) q = q.Where(a => a.ServicioId == servicioId.Value);
        if (laboratorioId.HasValue) q = q.Where(a => a.LaboratorioId == laboratorioId.Value);
        if (autorId.HasValue) q = q.Where(a => a.AutorId == autorId.Value);

        page = page <= 0 ? 1 : page;
        pageSize = pageSize is <= 0 or > 100 ? 20 : pageSize;

        var total = await q.CountAsync();
        var data = await q
            .OrderByDescending(a => a.UltimaActualizacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var a = await _db.ArticulosConocimiento.FindAsync(id);
        return a is null ? NotFound() : Ok(a);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ArticuloConocimiento dto)
    {
        dto.Id = 0;
        dto.FechaCreacion = DateTime.UtcNow;
        dto.UltimaActualizacion = dto.FechaCreacion;
        _db.ArticulosConocimiento.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    // Crear artículo desde una incidencia (toma servicio/lab de la incidencia si no vienen)
    [HttpPost("/api/incidencias/{incidenciaId:int}/kb")]
    public async Task<IActionResult> CreateFromIncidencia([FromRoute] int incidenciaId, [FromBody] ArticuloConocimiento dto)
    {
        var inc = await _db.Incidencias.AsNoTracking().FirstOrDefaultAsync(i => i.Id == incidenciaId);
        if (inc is null) return NotFound("Incidencia no existe.");

        dto.Id = 0;
        dto.IncidenciaOrigenId = incidenciaId;
        dto.ServicioId ??= inc.ServicioId;

        // Si agregaste LaboratorioId a Incidencia como propiedad shadow, podrías recuperarla así:
        var labId = _db.Entry(inc).Property<int?>("LaboratorioId").CurrentValue;
        dto.LaboratorioId ??= labId;

        dto.FechaCreacion = DateTime.UtcNow;
        dto.UltimaActualizacion = dto.FechaCreacion;

        _db.ArticulosConocimiento.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ArticuloConocimiento dto)
    {
        var a = await _db.ArticulosConocimiento.FindAsync(id);
        if (a is null) return NotFound();

        a.Titulo = dto.Titulo;
        a.Contenido = dto.Contenido;
        a.Referencias = dto.Referencias;
        a.Etiquetas = dto.Etiquetas;
        a.ServicioId = dto.ServicioId;
        a.LaboratorioId = dto.LaboratorioId;
        a.AutorId = dto.AutorId;
        a.Activo = dto.Activo;
        a.UltimaActualizacion = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var a = await _db.ArticulosConocimiento.FindAsync(id);
        if (a is null) return NotFound();
        _db.ArticulosConocimiento.Remove(a);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}