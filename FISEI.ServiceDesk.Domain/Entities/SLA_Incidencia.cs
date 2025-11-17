namespace FISEI.ServiceDesk.Domain.Entities;

public class SLA_Incidencia
{
    public int Id { get; set; } // IDENTITY
    public int IncidenciaId { get; set; }
    public DateTime FechaLimiteRespuesta { get; set; }
    public DateTime FechaLimiteResolucion { get; set; }
    public bool CumplidoRespuesta { get; set; }
    public bool CumplidoResolucion { get; set; }
    public DateTime CreadoUtc { get; set; }
}