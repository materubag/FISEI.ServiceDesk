namespace FISEI.ServiceDesk.Domain.Entities;

public class Laboratorio
{
    public int Id { get; set; }
    public string Codigo { get; set; } = default!;     // ej: LAB-A1
    public string Nombre { get; set; } = default!;     // ej: Laboratorio de Redes
    public string? Edificio { get; set; }              // ej: Edif. A
    public string? Ubicacion { get; set; }             // ej: Piso 2
    public bool Activo { get; set; } = true;
}