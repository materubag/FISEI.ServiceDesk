namespace FISEI.ServiceDesk.Domain.Entities;

public class EstadoIncidencia
{
    public int Id { get; set; }
    public string Codigo { get; set; } = default!; // REPORTADO, ASIGNADO, EN_PROCESO, RESUELTO, CERRADO
    public bool EsFinal { get; set; }
    public int Orden { get; set; }
}