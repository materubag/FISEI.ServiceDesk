using System;

namespace FISEI.ServiceDesk.Application.DTOs.Incidencias;

public class CambiarEstadoDto
{
    public int NuevoEstadoId { get; set; }
    public Guid ActorId { get; set; }
    public string? Comentario { get; set; }
}