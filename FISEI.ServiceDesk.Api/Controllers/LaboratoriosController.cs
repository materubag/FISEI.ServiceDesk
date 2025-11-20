using FISEI.ServiceDesk.Domain.Entities;
using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LaboratoriosController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public LaboratoriosController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? activo = null)
    {
        var q = _db.Laboratorios.AsNoTracking().AsQueryable();
        if (activo.HasValue) q = q.Where(l => l.Activo == activo.Value);
        var list = await q.OrderBy(l => l.Codigo).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.Laboratorios.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Laboratorio dto)
    {
        _db.Laboratorios.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Laboratorio dto)
    {
        var e = await _db.Laboratorios.FindAsync(id);
        if (e is null) return NotFound();
        e.Codigo = dto.Codigo;
        e.Nombre = dto.Nombre;
        e.Edificio = dto.Edificio;
        e.Ubicacion = dto.Ubicacion;
        e.Activo = dto.Activo;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Laboratorios.FindAsync(id);
        if (e is null) return NotFound();
        _db.Laboratorios.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}