namespace FISEI.ServiceDesk.Domain.Entities;

public class FeedbackIncidencia
{
    public int Id { get; set; } // IDENTITY
    public int IncidenciaId { get; set; }
    public int UsuarioId { get; set; }
    public byte Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime Fecha { get; set; }
}