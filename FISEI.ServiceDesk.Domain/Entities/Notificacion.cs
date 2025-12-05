namespace FISEI.ServiceDesk.Domain.Entities;

public class Notificacion
{
    public int Id { get; set; } 
    public int UsuarioDestinoId { get; set; }
    public string Tipo { get; set; } = default!;
    public string Referencia { get; set; } = default!;
    public string Mensaje { get; set; } = default!;
    public bool Leida { get; set; }
    public DateTime Fecha { get; set; }
}