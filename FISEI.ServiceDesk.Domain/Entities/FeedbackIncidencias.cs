using System;

namespace FISEI.ServiceDesk.Domain.Entities;

public class FeedbackIncidencia
{
    public long Id { get; set; }
    public long IncidenciaId { get; set; }
    public Guid UsuarioId { get; set; } // estudiante
    public byte Puntuacion { get; set; } // 1..5
    public string? Comentario { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}