namespace FISEI.ServiceDesk.Domain.Entities;

public class Seguimiento
{
    public int Id { get; set; } // IDENTITY
    public int IncidenciaId { get; set; }
    public int? EstadoAnteriorId { get; set; }
    public int EstadoNuevoId { get; set; }
    public int UsuarioId { get; set; }
    public string? Comentario { get; set; }
    public DateTime Fecha { get; set; }
}