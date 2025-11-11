using System.Net.Http.Json;

namespace FISEI.ServiceDesk.Web.Services;

public class AuthClientService
{
    private readonly HttpClient _http;
    private string? _token;

    public Guid? CurrentUserId { get; private set; }
    public string? CurrentRol { get; private set; }
    public bool IsAuthenticated => _token != null;

    public AuthClientService(HttpClient http)
    {
        _http = http;
    }

    public record LoginRequest(string correo, string password);
    public record LoginResponse(Guid userId, string nombre, string rol, string token, DateTime expiraUtc);

    public async Task<bool> LoginAsync(string correo, string password)
    {
        var resp = await _http.PostAsJsonAsync("api/auth/login", new LoginRequest(correo, password));
        if (!resp.IsSuccessStatusCode) return false;

        var data = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        if (data is null) return false;

        _token = data.token;
        CurrentUserId = data.userId;
        CurrentRol = data.rol;

        // Nota: ya no seteamos Authorization en DefaultRequestHeaders aquí;
        // el AuthHeaderHandler leerá el token en cada request.
        return true;
    }

    public void Logout()
    {
        _token = null;
        CurrentUserId = null;
        CurrentRol = null;
    }

    public string? GetToken() => _token;
}