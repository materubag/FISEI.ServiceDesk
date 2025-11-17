using System;

namespace FISEI.ServiceDesk.Application.DTOs.Incidencias;

public class IncidenciaDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = default!;
    public string Titulo { get; set; } = default!;
    public string Descripcion { get; set; } = default!;
    public int EstadoId { get; set; }
    public int PrioridadId { get; set; }
    public int ServicioId { get; set; }
    public int CreadorId { get; set; }
    public int? TecnicoAsignadoId { get; set; }
    public DateTime FechaCreacion { get; set; }
}