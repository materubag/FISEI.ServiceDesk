using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public RolesController(ServiceDeskDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.Roles.AsNoTracking().OrderBy(r => r.Id).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.Roles.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Domain.Entities.Rol dto)
    {
        _db.Roles.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Domain.Entities.Rol dto)
    {
        var e = await _db.Roles.FindAsync(id);
        if (e is null) return NotFound();
        e.Nombre = dto.Nombre;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Roles.FindAsync(id);
        if (e is null) return NotFound();
        _db.Roles.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}