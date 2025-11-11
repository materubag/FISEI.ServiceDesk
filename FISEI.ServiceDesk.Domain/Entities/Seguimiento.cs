using System;

namespace FISEI.ServiceDesk.Domain.Entities;

public class Seguimiento
{
    public long Id { get; set; }
    public long IncidenciaId { get; set; }
    public Guid UsuarioId { get; set; }
    public int? EstadoAnteriorId { get; set; }
    public int? EstadoNuevoId { get; set; }
    public string? Comentario { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}