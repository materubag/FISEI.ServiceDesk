namespace FISEI.ServiceDesk.Application.DTOs.Auth;

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Nombre { get; set; } = default!;
    public string Rol { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTime ExpiraUtc { get; set; }
}