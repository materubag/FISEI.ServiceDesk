namespace FISEI.ServiceDesk.Application.DTOs.Auth;

public class LoginRequestDto
{
    public string Correo { get; set; } = default!;
    public string Password { get; set; } = default!;
}