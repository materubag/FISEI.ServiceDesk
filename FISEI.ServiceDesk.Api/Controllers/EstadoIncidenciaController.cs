using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadosIncidenciaController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public EstadosIncidenciaController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.EstadosIncidencia.AsNoTracking().OrderBy(e => e.Orden).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.EstadosIncidencia.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Domain.Entities.EstadoIncidencia dto)
    {
        _db.EstadosIncidencia.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Domain.Entities.EstadoIncidencia dto)
    {
        var e = await _db.EstadosIncidencia.FindAsync(id);
        if (e is null) return NotFound();
        e.Codigo = dto.Codigo;
        e.EsFinal = dto.EsFinal;
        e.Orden = dto.Orden;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.EstadosIncidencia.FindAsync(id);
        if (e is null) return NotFound();
        _db.EstadosIncidencia.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}