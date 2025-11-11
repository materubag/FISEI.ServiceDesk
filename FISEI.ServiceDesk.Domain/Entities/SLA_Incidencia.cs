using System;

namespace FISEI.ServiceDesk.Domain.Entities;

public class SLA_Incidencia
{
    public long Id { get; set; }
    public long IncidenciaId { get; set; }
    public DateTime FechaLimiteRespuesta { get; set; }
    public DateTime FechaLimiteResolucion { get; set; }
    public bool CumplidoRespuesta { get; set; }
    public bool CumplidoResolucion { get; set; }
    public DateTime CreadoUtc { get; set; }
}