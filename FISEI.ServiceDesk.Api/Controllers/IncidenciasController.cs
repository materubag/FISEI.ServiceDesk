using FISEI.ServiceDesk.Api.Services;
using FISEI.ServiceDesk.Application.DTOs.Comentarios;
using FISEI.ServiceDesk.Application.DTOs.Incidencias;
using FISEI.ServiceDesk.Domain.Entities;
using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FISEI.ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidenciasController : ControllerBase
{
    private readonly ServiceDeskDbContext _db;
    private readonly IRealtimeNotifier _notifier;

    public IncidenciasController(ServiceDeskDbContext db, IRealtimeNotifier notifier)
    {
        _db = db;
        _notifier = notifier;
    }

    // POST /api/incidencias
    [HttpPost]
    public async Task<ActionResult<IncidenciaDto>> Crear([FromBody] CreateIncidenciaDto dto)
    {
        // Validaciones mínimas
        var existeCreador = await _db.Usuarios.AnyAsync(u => u.Id == dto.CreadorId);
        if (!existeCreador) return BadRequest("CreadorId no existe.");

        var estadoReportado = await _db.EstadosIncidencia.Where(e => e.Codigo == "REPORTADO").Select(e => e.Id).FirstAsync();

        var entity = new Incidencia
        {
            CreadorId = dto.CreadorId,
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            PrioridadId = dto.PrioridadId,
            ServicioId = dto.ServicioId,
            EstadoId = estadoReportado
        };

        _db.Incidencias.Add(entity);
        await _db.SaveChangesAsync();

        // Seguimiento inicial
        _db.Seguimientos.Add(new Seguimiento
        {
            IncidenciaId = entity.Id,
            UsuarioId = dto.CreadorId,
            EstadoAnteriorId = null,
            EstadoNuevoId = estadoReportado,
            Comentario = "Incidencia creada"
        });

        await _db.SaveChangesAsync();

        // Notificación a Técnicos y Administradores (persistente + tiempo real)
        var tecnicosYAdmins = await _db.Usuarios.Where(u => u.RolId == 2 || u.RolId == 3).Select(u => u.Id).ToListAsync();
        var refCodigo = $"INC-{entity.Id:000000}";
        foreach (var uid in tecnicosYAdmins)
        {
            _db.Notificaciones.Add(new Notificacion
            {
                UsuarioDestinoId = uid,
                Tipo = "NUEVA_INCIDENCIA",
                Referencia = refCodigo,
                Mensaje = $"Nueva incidencia {refCodigo}: {entity.Titulo}"
            });
        }
        await _db.SaveChangesAsync();

        await _notifier.NotifyAllAsync($"Nueva incidencia {refCodigo}");

        var result = new IncidenciaDto
        {
            Id = entity.Id,
            Codigo = refCodigo,
            Titulo = entity.Titulo,
            Descripcion = entity.Descripcion,
            EstadoId = entity.EstadoId,
            PrioridadId = entity.PrioridadId,
            ServicioId = entity.ServicioId,
            CreadorId = entity.CreadorId,
            TecnicoAsignadoId = entity.TecnicoAsignadoId,
            FechaCreacion = entity.FechaCreacion
        };

        return CreatedAtAction(nameof(ObtenerMias), new { usuarioId = dto.CreadorId }, result);
    }

    // GET /api/incidencias/mias?usuarioId=GUID
    [HttpGet("mias")]
    public async Task<ActionResult<IEnumerable<IncidenciaDto>>> ObtenerMias([FromQuery] Guid usuarioId)
    {
        var query = _db.Incidencias
            .Where(i => i.CreadorId == usuarioId)
            .OrderByDescending(i => i.Id)
            .Select(i => new IncidenciaDto
            {
                Id = i.Id,
                Codigo = $"INC-{i.Id:000000}",
                Titulo = i.Titulo,
                Descripcion = i.Descripcion,
                EstadoId = i.EstadoId,
                PrioridadId = i.PrioridadId,
                ServicioId = i.ServicioId,
                CreadorId = i.CreadorId,
                TecnicoAsignadoId = i.TecnicoAsignadoId,
                FechaCreacion = i.FechaCreacion
            });

        var list = await query.ToListAsync();
        return Ok(list);
    }

    // PATCH /api/incidencias/{id}/estado
    [HttpPatch("{id:long}/estado")]
    public async Task<ActionResult> CambiarEstado([FromRoute] long id, [FromBody] CambiarEstadoDto dto)
    {
        var inc = await _db.Incidencias.FindAsync(id);
        if (inc is null) return NotFound();

        var estadoAnterior = inc.EstadoId;
        inc.CambiarEstado(dto.NuevoEstadoId);
        if (dto.NuevoEstadoId == await _db.EstadosIncidencia.Where(e => e.Codigo == "RESUELTO").Select(e => e.Id).FirstAsync())
            inc.FechaResolucion = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _db.Seguimientos.Add(new Seguimiento
        {
            IncidenciaId = inc.Id,
            UsuarioId = dto.ActorId,
            EstadoAnteriorId = estadoAnterior,
            EstadoNuevoId = dto.NuevoEstadoId,
            Comentario = dto.Comentario
        });
        await _db.SaveChangesAsync();

        var refCodigo = $"INC-{inc.Id:000000}";

        // Notificación al creador
        _db.Notificaciones.Add(new Notificacion
        {
            UsuarioDestinoId = inc.CreadorId,
            Tipo = "ESTADO_CAMBIADO",
            Referencia = refCodigo,
            Mensaje = $"Tu incidencia {refCodigo} cambió de estado."
        });
        await _db.SaveChangesAsync();

        await _notifier.NotifyAllAsync($"Incidencia {refCodigo} cambió de estado");

        return NoContent();
    }

    // POST /api/incidencias/{id}/comentarios
    [HttpPost("{id:long}/comentarios")]
    public async Task<ActionResult> AgregarComentario([FromRoute] long id, [FromBody] CreateComentarioDto dto)
    {
        var inc = await _db.Incidencias.FindAsync(id);
        if (inc is null) return NotFound();

        _db.ComentariosIncidencia.Add(new ComentarioIncidencia
        {
            IncidenciaId = id,
            UsuarioId = dto.UsuarioId,
            Texto = dto.Texto,
            EsInterno = dto.EsInterno
        });
        await _db.SaveChangesAsync();

        var refCodigo = $"INC-{id:000000}";

        // Notifica a la contraparte básica (si comenta técnico, notifica al creador; si comenta estudiante y hay técnico asignado, notifícalo)
        var actor = await _db.Usuarios.Where(u => u.Id == dto.UsuarioId).Select(u => new { u.Id, u.RolId }).FirstOrDefaultAsync();
        if (actor is not null)
        {
            if (actor.RolId == 2 || actor.RolId == 3) // técnico o admin
            {
                _db.Notificaciones.Add(new Notificacion
                {
                    UsuarioDestinoId = inc.CreadorId,
                    Tipo = "NUEVO_COMENTARIO",
                    Referencia = refCodigo,
                    Mensaje = $"Nuevo comentario en {refCodigo}"
                });
            }
            else // estudiante
            {
                if (inc.TecnicoAsignadoId.HasValue)
                {
                    _db.Notificaciones.Add(new Notificacion
                    {
                        UsuarioDestinoId = inc.TecnicoAsignadoId.Value,
                        Tipo = "NUEVO_COMENTARIO",
                        Referencia = refCodigo,
                        Mensaje = $"Nuevo comentario en {refCodigo}"
                    });
                }
            }
            await _db.SaveChangesAsync();
        }

        await _notifier.NotifyAllAsync($"Nuevo comentario en {refCodigo}");

        return NoContent();
    }
}