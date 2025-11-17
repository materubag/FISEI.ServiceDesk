namespace FISEI.ServiceDesk.Domain.Entities;

public class Usuario
{
    public int Id { get; set; } // IDENTITY
    public string Nombre { get; set; } = default!;
    public string Correo { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string PasswordSalt { get; set; } = default!;
    public int RolId { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; }
}