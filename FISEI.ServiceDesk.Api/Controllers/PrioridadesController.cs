using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrioridadesController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public PrioridadesController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.Prioridades.AsNoTracking().OrderBy(p => p.Peso).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.Prioridades.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Domain.Entities.Prioridad dto)
    {
        _db.Prioridades.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Domain.Entities.Prioridad dto)
    {
        var e = await _db.Prioridades.FindAsync(id);
        if (e is null) return NotFound();
        e.Nombre = dto.Nombre;
        e.Peso = dto.Peso;
        e.TiempoMaxHoras = dto.TiempoMaxHoras;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Prioridades.FindAsync(id);
        if (e is null) return NotFound();
        _db.Prioridades.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}