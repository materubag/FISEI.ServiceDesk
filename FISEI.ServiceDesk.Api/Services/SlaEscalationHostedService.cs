using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FISEI.ServiceDesk.Infrastructure.Persistence;
using FISEI.ServiceDesk.Domain.Entities;

namespace FISEI.ServiceDesk.Api.Services;

public class SlaEscalationHostedService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<SlaEscalationHostedService> _logger;
    public SlaEscalationHostedService(IServiceProvider sp, ILogger<SlaEscalationHostedService> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SLA Escalation Service iniciado.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ServiceDeskDbContext>();

                // Si la BD aún no está disponible, espera y reintenta
                if (!await db.Database.CanConnectAsync(stoppingToken))
                {
                    _logger.LogWarning("BD no disponible aún. Reintentando en 30s.");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    continue;
                }

                var ahora = DateTime.UtcNow;

                var estadoReportadoId = await db.EstadosIncidencia
                    .Where(e => e.Codigo == "REPORTADO")
                    .Select(e => e.Id)
                    .SingleAsync(stoppingToken);

                var pendientesRespuesta = await (
                    from sla in db.SLA_Incidencias
                    join inc in db.Incidencias on sla.IncidenciaId equals inc.Id
                    where sla.CumplidoRespuesta == false
                          && inc.EstadoId == estadoReportadoId
                          && sla.FechaLimiteRespuesta < ahora
                    select new { sla, inc }
                ).ToListAsync(stoppingToken);

                foreach (var item in pendientesRespuesta)
                {
                    db.Escalaciones.Add(new Escalacion
                    {
                        IncidenciaId = item.inc.Id,
                        Nivel = 2,
                        Motivo = "SLA de respuesta vencido",
                        Fecha = ahora
                    });

                    var admins = await db.Usuarios.Where(u => u.RolId == 3).Select(u => u.Id).ToListAsync(stoppingToken);
                    var refCod = $"INC-{item.inc.Id:000000}";
                    foreach (var admin in admins)
                    {
                        db.Notificaciones.Add(new Notificacion
                        {
                            UsuarioDestinoId = admin,
                            Tipo = "ESCALACION",
                            Referencia = refCod,
                            Mensaje = $"Escalación por SLA respuesta vencido: {refCod}"
                        });
                    }

                    item.sla.CumplidoRespuesta = true;
                }

                var estadoResueltoId = await db.EstadosIncidencia.Where(e => e.Codigo == "RESUELTO").Select(e => e.Id).SingleAsync(stoppingToken);
                var estadoCerradoId  = await db.EstadosIncidencia.Where(e => e.Codigo == "CERRADO").Select(e => e.Id).SingleAsync(stoppingToken);

                var pendientesResolucion = await (
                    from sla in db.SLA_Incidencias
                    join inc in db.Incidencias on sla.IncidenciaId equals inc.Id
                    where sla.CumplidoResolucion == false
                          && inc.EstadoId != estadoResueltoId
                          && inc.EstadoId != estadoCerradoId
                          && sla.FechaLimiteResolucion < ahora
                    select new { sla, inc }
                ).ToListAsync(stoppingToken);

                foreach (var item in pendientesResolucion)
                {
                    db.Escalaciones.Add(new Escalacion
                    {
                        IncidenciaId = item.inc.Id,
                        Nivel = 3,
                        Motivo = "SLA de resolución vencido",
                        Fecha = ahora
                    });

                    var admins = await db.Usuarios.Where(u => u.RolId == 3).Select(u => u.Id).ToListAsync(stoppingToken);
                    var refCod = $"INC-{item.inc.Id:000000}";
                    foreach (var admin in admins)
                    {
                        db.Notificaciones.Add(new Notificacion
                        {
                            UsuarioDestinoId = admin,
                            Tipo = "ESCALACION",
                            Referencia = refCod,
                            Mensaje = $"Escalación por SLA resolución vencido: {refCod}"
                        });
                    }

                    item.sla.CumplidoResolucion = true;
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en SLA Escalation cycle");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}