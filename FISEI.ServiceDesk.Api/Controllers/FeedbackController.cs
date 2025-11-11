using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FISEI.ServiceDesk.Infrastructure.Persistence;
using FISEI.ServiceDesk.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/incidencias/{incidenciaId:long}/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public FeedbackController(ServiceDeskDbContext db) => _db = db;

    public record FeedbackCreateDto(byte Puntuacion, string? Comentario);

    // POST /api/incidencias/{id}/feedback
    [HttpPost]
    [Authorize(Policy = "Estudiante")]
    public async Task<IActionResult> Crear(long incidenciaId, [FromBody] FeedbackCreateDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (userIdClaim is null) return Unauthorized();
        var userId = Guid.Parse(userIdClaim);

        var inc = await _db.Incidencias.FindAsync(incidenciaId);
        if (inc is null) return NotFound("Incidencia no existe");
        if (inc.CreadorId != userId) return Forbid("No eres dueño de la incidencia");
        if (inc.EstadoId != await _db.EstadosIncidencia.Where(e => e.Codigo == "RESUELTO").Select(e => e.Id).FirstAsync())
            return BadRequest("Solo se puede dar feedback cuando está RESUELTO");
        if (await _db.Feedbacks.AnyAsync(f => f.IncidenciaId == incidenciaId))
            return Conflict("Ya existe feedback");

        var fb = new FeedbackIncidencia
        {
            IncidenciaId = incidenciaId,
            UsuarioId = userId,
            Puntuacion = dto.Puntuacion,
            Comentario = dto.Comentario,
            Fecha = DateTime.UtcNow
        };
        _db.Feedbacks.Add(fb);

        // Cambiar a CERRADO
        var estadoCerradoId = await _db.EstadosIncidencia.Where(e => e.Codigo == "CERRADO").Select(e => e.Id).FirstAsync();
        inc.EstadoId = estadoCerradoId;
        inc.Cerrada = true;
        inc.FechaUltimoCambio = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // Notificar al técnico asignado y administradores
        var refCodigo = $"INC-{inc.Id:000000}";
        var destinatarios = new List<Guid>();
        if (inc.TecnicoAsignadoId.HasValue) destinatarios.Add(inc.TecnicoAsignadoId.Value);
        destinatarios.AddRange(await _db.Usuarios.Where(u => u.RolId == 3).Select(u => u.Id).ToListAsync());
        foreach (var uid in destinatarios.Distinct())
        {
            _db.Notificaciones.Add(new Notificacion
            {
                UsuarioDestinoId = uid,
                Tipo = "FEEDBACK",
                Referencia = refCodigo,
                Mensaje = $"Feedback {dto.Puntuacion} en {refCodigo}"
            });
        }
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Crear), new { incidenciaId }, null);
    }
}