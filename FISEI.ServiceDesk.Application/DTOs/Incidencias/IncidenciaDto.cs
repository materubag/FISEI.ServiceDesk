using System;

namespace FISEI.ServiceDesk.Application.DTOs.Incidencias;

public class IncidenciaDto
{
    public long Id { get; set; }
    public string Codigo { get; set; } = default!; // INC-000123 (formato presentacional)
    public string Titulo { get; set; } = default!;
    public string Descripcion { get; set; } = default!;
    public int EstadoId { get; set; }
    public int PrioridadId { get; set; }
    public int ServicioId { get; set; }
    public Guid CreadorId { get; set; }
    public Guid? TecnicoAsignadoId { get; set; }
    public DateTime FechaCreacion { get; set; }
}