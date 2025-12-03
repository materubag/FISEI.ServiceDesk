using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/sla/definiciones")]
public class SlaDefinicionesController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public SlaDefinicionesController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.SLA_Definiciones.AsNoTracking().OrderBy(d => d.PrioridadId).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.SLA_Definiciones.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Domain.Entities.SLA_Definicion dto)
    {
        _db.SLA_Definiciones.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Domain.Entities.SLA_Definicion dto)
    {
        var e = await _db.SLA_Definiciones.FindAsync(id);
        if (e is null) return NotFound();
        e.PrioridadId = dto.PrioridadId;
        e.ServicioId = dto.ServicioId;
        e.HorasRespuesta = dto.HorasRespuesta;
        e.HorasResolucion = dto.HorasResolucion;
        e.Activo = dto.Activo;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.SLA_Definiciones.FindAsync(id);
        if (e is null) return NotFound();
        _db.SLA_Definiciones.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}