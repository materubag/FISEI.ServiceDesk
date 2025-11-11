using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiciosController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public ServiciosController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? categoriaId = null)
    {
        var q = _db.Servicios.AsNoTracking().AsQueryable();
        if (categoriaId.HasValue) q = q.Where(s => s.CategoriaId == categoriaId.Value);
        return Ok(await q.OrderBy(s => s.Nombre).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.Servicios.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Domain.Entities.Servicio dto)
    {
        _db.Servicios.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Domain.Entities.Servicio dto)
    {
        var e = await _db.Servicios.FindAsync(id);
        if (e is null) return NotFound();
        e.Nombre = dto.Nombre;
        e.CategoriaId = dto.CategoriaId;
        e.Activo = dto.Activo;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Servicios.FindAsync(id);
        if (e is null) return NotFound();
        _db.Servicios.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}