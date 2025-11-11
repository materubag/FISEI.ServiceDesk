namespace FISEI.ServiceDesk.Domain.Entities;

public class Servicio
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = default!;
    public bool Activo { get; set; } = true;
}