using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FISEI.ServiceDesk.Infrastructure.Persistence;
using FISEI.ServiceDesk.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/incidencias/{incidenciaId:int}/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    public FeedbackController(ServiceDeskDbContext db) => _db = db;

    public record FeedbackCreateDto(byte Puntuacion, string? Comentario);

    // POST /api/incidencias/{incidenciaId}/feedback
    [HttpPost]
    [Authorize(Policy = "Estudiante")]
    public async Task<IActionResult> Crear(int incidenciaId, [FromBody] FeedbackCreateDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (userIdClaim is null) return Unauthorized();

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized("Id de usuario inválido en el token.");

        var inc = await _db.Incidencias.FindAsync(incidenciaId);
        if (inc is null) return NotFound("Incidencia no existe");
        if (inc.CreadorId != userId) return Forbid("No eres dueño de la incidencia");

        // Estado RESUELTO y CERRADO
        var estados = await _db.EstadosIncidencia
            .Where(e => e.Codigo == "RESUELTO" || e.Codigo == "CERRADO")
            .ToDictionaryAsync(e => e.Codigo, e => e.Id);

        if (!estados.TryGetValue("RESUELTO", out var estadoResueltoId) ||
            !estados.TryGetValue("CERRADO", out var estadoCerradoId))
            return StatusCode(500, "Estados básicos (RESUELTO/CERRADO) no están sembrados.");

        if (inc.EstadoId != estadoResueltoId)
            return BadRequest("Solo se puede dar feedback cuando la incidencia está RESUELTO.");
        if (await _db.Feedbacks.AnyAsync(f => f.IncidenciaId == incidenciaId))
            return Conflict("Ya existe feedback para esta incidencia.");

        // Crear feedback
        var fb = new FeedbackIncidencia
        {
            IncidenciaId = incidenciaId,
            UsuarioId = userId,
            Puntuacion = dto.Puntuacion,
            Comentario = dto.Comentario,
            Fecha = DateTime.UtcNow
        };
        _db.Feedbacks.Add(fb);

        // Cerrar incidencia
        inc.EstadoId = estadoCerradoId;
        inc.Cerrada = true;
        inc.FechaUltimoCambio = DateTime.UtcNow;

        // Seguimiento opcional del cambio a CERRADO
        _db.Seguimientos.Add(new Seguimiento
        {
            IncidenciaId = inc.Id,
            EstadoAnteriorId = estadoResueltoId,
            EstadoNuevoId = estadoCerradoId,
            UsuarioId = userId,
            Comentario = $"Feedback {dto.Puntuacion}",
            Fecha = DateTime.UtcNow
        });

        // Notificaciones
        var refCodigo = $"INC-{inc.Id:000000}";
        var destinatarios = new List<int>();

        if (inc.TecnicoAsignadoId.HasValue)
            destinatarios.Add(inc.TecnicoAsignadoId.Value);

        destinatarios.AddRange(await _db.Usuarios.Where(u => u.RolId == 3).Select(u => u.Id).ToListAsync());

        foreach (var uid in destinatarios.Distinct())
        {
            _db.Notificaciones.Add(new Notificacion
            {
                UsuarioDestinoId = uid,
                Tipo = "FEEDBACK",
                Referencia = refCodigo,
                Mensaje = $"Feedback {dto.Puntuacion} en {refCodigo}",
                Leida = false,
                Fecha = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Crear), new { incidenciaId }, null);
    }
}