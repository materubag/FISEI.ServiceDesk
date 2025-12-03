namespace FISEI.ServiceDesk.Domain.Entities;

public class ComentarioIncidencia
{
    public int Id { get; set; }
    public int IncidenciaId { get; set; }
    public int UsuarioId { get; set; }
    public string Texto { get; set; } = default!;
    public bool EsInterno { get; set; }   // <- AGREGAR
    public DateTime Fecha { get; set; }
}