namespace FISEI.ServiceDesk.Domain.Entities;

public class Incidencia
{
    public int Id { get; set; }
    public string Titulo { get; set; } = default!;
    public string Descripcion { get; set; } = default!;
    public int EstadoId { get; set; }
    public int PrioridadId { get; set; }
    public int ServicioId { get; set; }
    public int CreadorId { get; set; }
    public int? TecnicoAsignadoId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaUltimoCambio { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public bool Cerrada { get; set; }
    public bool Activo { get; set; } = true;

    // Método de dominio (simple). Puedes extender validaciones de transición aquí.
    public void CambiarEstado(int nuevoEstadoId)
    {
        if (EstadoId == nuevoEstadoId) return;
        EstadoId = nuevoEstadoId;
        FechaUltimoCambio = DateTime.UtcNow;
    }
}