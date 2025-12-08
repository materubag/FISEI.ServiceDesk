using FISEI.ServiceDesk.Api.Services;
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
        if (!await _db.Usuarios.AnyAsync(u => u.Id == dto.CreadorId))
            return BadRequest("CreadorId no existe.");

        var estadoReportadoId = await _db.EstadosIncidencia
            .Where(e => e.Codigo == "REPORTADO")
            .Select(e => e.Id)
            .FirstOrDefaultAsync();
        if (estadoReportadoId == 0)
        {
            estadoReportadoId = await _db.EstadosIncidencia
                .Where(e => e.Codigo == "ABIERTO")
                .Select(e => e.Id)
                .FirstOrDefaultAsync();
            if (estadoReportadoId == 0)
                return BadRequest("No se encontró estado inicial (REPORTADO/ABIERTO) en el catálogo.");
        }

        var entity = new Incidencia
        {
            CreadorId = dto.CreadorId,
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            PrioridadId = dto.PrioridadId,
            ServicioId = dto.ServicioId,
            EstadoId = estadoReportadoId
        };

        _db.Incidencias.Add(entity);
        if (dto.LaboratorioId.HasValue)
        {
            _db.Entry(entity).Property<int?>("LaboratorioId").CurrentValue = dto.LaboratorioId.Value;
            await _db.SaveChangesAsync();
        }
        await _db.SaveChangesAsync();

        _db.Seguimientos.Add(new Seguimiento
        {
            IncidenciaId = entity.Id,
            UsuarioId = dto.CreadorId,
            EstadoAnteriorId = null,
            EstadoNuevoId = estadoReportadoId,
            Comentario = "Incidencia creada"
        });

        var slaDef = await _db.SLA_Definiciones
            .Where(d => d.Activo && d.PrioridadId == entity.PrioridadId)
            .OrderByDescending(d => d.Id)
            .FirstOrDefaultAsync();

        if (slaDef is not null)
        {
            var ahora = DateTime.UtcNow;
            _db.SLA_Incidencias.Add(new SLA_Incidencia
            {
                IncidenciaId = entity.Id,
                FechaLimiteRespuesta = ahora.AddHours(slaDef.HorasRespuesta),
                FechaLimiteResolucion = ahora.AddHours(slaDef.HorasResolucion),
                CumplidoRespuesta = false,
                CumplidoResolucion = false,
                CreadoUtc = ahora
            });
        }

        var tecnicosYAdmins = await _db.Usuarios
            .Where(u => u.RolId == 2 || u.RolId == 3)
            .Select(u => u.Id)
            .ToListAsync();

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
        await _notifier.NotifyTecnicosAsync($"Nueva incidencia {refCodigo}");

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

    // GET /api/incidencias/mias?usuarioId=#
    [HttpGet("mias")]
    public async Task<ActionResult<IEnumerable<IncidenciaDto>>> ObtenerMias([FromQuery] int usuarioId)
    {
        var list = await _db.Incidencias
            .Where(i => i.CreadorId == usuarioId && i.Activo)
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
            })
            .ToListAsync();

        return Ok(list);
    }

    // PATCH /api/incidencias/{id}/estado
    [HttpPatch("{id:int}/estado")]
    public async Task<ActionResult> CambiarEstado([FromRoute] int id, [FromBody] CambiarEstadoDto dto)
    {
        var inc = await _db.Incidencias.FindAsync(id);
        if (inc is null || !inc.Activo) return NotFound();

        var estadoAnterior = inc.EstadoId;
        inc.CambiarEstado(dto.NuevoEstadoId);
        inc.FechaUltimoCambio = DateTime.UtcNow;

        var estadoResueltoId = await _db.EstadosIncidencia
            .Where(e => e.Codigo == "RESUELTO")
            .Select(e => e.Id)
            .FirstAsync();
        if (dto.NuevoEstadoId == estadoResueltoId)
            inc.FechaResolucion = DateTime.UtcNow;

        _db.Seguimientos.Add(new Seguimiento
        {
            IncidenciaId = inc.Id,
            UsuarioId = dto.ActorId,
            EstadoAnteriorId = estadoAnterior,
            EstadoNuevoId = dto.NuevoEstadoId,
            Comentario = dto.Comentario
        });

        // También registrar el comentario en la tabla ComentariosIncidencia si viene texto
        if (!string.IsNullOrWhiteSpace(dto.Comentario))
        {
            _db.ComentariosIncidencia.Add(new FISEI.ServiceDesk.Domain.Entities.ComentarioIncidencia
            {
                IncidenciaId = inc.Id,
                UsuarioId = dto.ActorId,
                Texto = dto.Comentario,
                // Si el DTO no tiene EsInterno, asumimos público (false)
                EsInterno = false,
                Fecha = DateTime.UtcNow
            });
        }

        var refCodigo = $"INC-{inc.Id:000000}";
        _db.Notificaciones.Add(new Notificacion
        {
            UsuarioDestinoId = inc.CreadorId,
            Tipo = "ESTADO_CAMBIADO",
            Referencia = refCodigo,
            Mensaje = $"Tu incidencia {refCodigo} cambió de estado."
        });

        await _db.SaveChangesAsync();
        await _notifier.NotifyUserAsync(inc.CreadorId, $"Incidencia {refCodigo} cambió de estado");

        return NoContent();
    }

    // PATCH /api/incidencias/{id}/asignar
    [HttpPatch("{id:int}/asignar")]
    public async Task<ActionResult> AsignarTecnico([FromRoute] int id, [FromBody] AsignarTecnicoDto dto)
    {
        var inc = await _db.Incidencias.FindAsync(id);
        if (inc is null || !inc.Activo) return NotFound();

        // Verificar que el técnico existe y tiene RolId = 2
        var tecnico = await _db.Usuarios.FindAsync(dto.TecnicoId);
        if (tecnico is null || tecnico.RolId != 2)
            return BadRequest("El usuario especificado no es un técnico válido.");

        inc.TecnicoAsignadoId = dto.TecnicoId;
        inc.FechaUltimoCambio = DateTime.UtcNow;

        // Registrar en seguimiento
        _db.Seguimientos.Add(new Seguimiento
        {
            IncidenciaId = inc.Id,
            UsuarioId = dto.AsignadoPorId,
            EstadoAnteriorId = inc.EstadoId,
            EstadoNuevoId = inc.EstadoId,
            Comentario = $"Incidencia asignada a {tecnico.Nombre}"
        });

        // Notificar al técnico asignado
        var refCodigo = $"INC-{inc.Id:000000}";
        _db.Notificaciones.Add(new Notificacion
        {
            UsuarioDestinoId = dto.TecnicoId,
            Tipo = "ASIGNACION",
            Referencia = refCodigo,
            Mensaje = $"Se te ha asignado la incidencia {refCodigo}: {inc.Titulo}"
        });

        await _db.SaveChangesAsync();
        await _notifier.NotifyUserAsync(dto.TecnicoId, $"Nueva asignación: {refCodigo}");

        return NoContent();
    }

    public record AsignarTecnicoDto(int TecnicoId, int AsignadoPorId);

    // Importante: eliminamos el POST de comentarios aquí para evitar duplicidad de rutas
}