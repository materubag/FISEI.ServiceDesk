using System;

namespace FISEI.ServiceDesk.Application.DTOs.Incidencias;

public class CreateIncidenciaDto
{
    public Guid CreadorId { get; set; }
    public string Titulo { get; set; } = default!;
    public string Descripcion { get; set; } = default!;
    public int PrioridadId { get; set; }
    public int ServicioId { get; set; }
}