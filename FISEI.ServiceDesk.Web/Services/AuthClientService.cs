using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class AuthClientService
{
    private readonly IHttpClientFactory _httpFactory;
    private string? _token;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_token);
    public int? CurrentUserId { get; private set; }
    public string? CurrentUserRole { get; private set; }
    public string? CurrentUserName { get; private set; }

    public AuthClientService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task<bool> LoginAsync(string correo, string password)
    {
        var http = _httpFactory.CreateClient("Auth"); // sin handler
        var payload = new { Correo = correo, Password = password };
        var resp = await http.PostAsJsonAsync("/api/auth/login", payload);
        if (!resp.IsSuccessStatusCode) return false;

        var dto = await resp.Content.ReadFromJsonAsync<LoginResponseDto>();
        if (dto is null || string.IsNullOrWhiteSpace(dto.Token)) return false;

        _token = dto.Token;
        CurrentUserId = dto.UserId;
        CurrentUserRole = dto.Rol;
        CurrentUserName = dto.Nombre;

        // Opcional: también setear en el cliente Api actual
        var api = _httpFactory.CreateClient("Api");
        api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        return true;
    }

    public void Logout()
    {
        _token = null;
        CurrentUserId = null;
        CurrentUserRole = null;
        CurrentUserName = null;
        // No limpiamos headers de instancias previas; el handler controla el bearer.
    }

    public string? GetToken() => _token;
}

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Nombre { get; set; } = default!;
    public string Rol { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTime ExpiraUtc { get; set; }
}