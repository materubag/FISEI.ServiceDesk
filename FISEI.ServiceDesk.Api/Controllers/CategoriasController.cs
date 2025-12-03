using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public CategoriasController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.Categorias.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Domain.Entities.Categoria dto)
    {
        _db.Categorias.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Domain.Entities.Categoria dto)
    {
        var e = await _db.Categorias.FindAsync(id);
        if (e is null) return NotFound();
        e.Nombre = dto.Nombre;
        e.Activo = dto.Activo;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Categorias.FindAsync(id);
        if (e is null) return NotFound();
        _db.Categorias.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}