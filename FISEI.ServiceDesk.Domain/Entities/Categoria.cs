namespace FISEI.ServiceDesk.Domain.Entities;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public bool Activo { get; set; } = true;
}