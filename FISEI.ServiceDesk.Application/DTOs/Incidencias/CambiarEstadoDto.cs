using System;

namespace FISEI.ServiceDesk.Application.DTOs.Incidencias;

public class CambiarEstadoDto
{
    public int NuevoEstadoId { get; set; }
    public int ActorId { get; set; }
    public string? Comentario { get; set; }
}