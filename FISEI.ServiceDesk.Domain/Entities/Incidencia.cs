using System;

namespace FISEI.ServiceDesk.Domain.Entities;

public class Incidencia
{
    public long Id { get; set; }
    public Guid CreadorId { get; set; }
    public Guid? TecnicoAsignadoId { get; set; }
    public int ServicioId { get; set; }
    public int PrioridadId { get; set; }
    public int EstadoId { get; set; }
    public string Titulo { get; set; } = default!;
    public string Descripcion { get; set; } = default!;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaUltimoCambio { get; set; } = DateTime.UtcNow;
    public DateTime? FechaResolucion { get; set; }
    public bool Cerrada { get; set; }
    public bool Activo { get; set; } = true;

    public void CambiarEstado(int nuevoEstado)
    {
        EstadoId = nuevoEstado;
        FechaUltimoCambio = DateTime.UtcNow;
    }
}