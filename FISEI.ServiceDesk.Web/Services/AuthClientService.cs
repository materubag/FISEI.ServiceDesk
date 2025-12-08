using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FISEI.ServiceDesk.Web.Services;

public class AuthClientService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ProtectedLocalStorage _storage;
    private string? _token;
    public string? LastError { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_token);
    public int? CurrentUserId { get; private set; }
    public string? CurrentUserRole { get; private set; }
    public string? CurrentUserName { get; private set; }

    public AuthClientService(IHttpClientFactory httpFactory, ProtectedLocalStorage storage)
    {
        _httpFactory = httpFactory;
        _storage = storage;
    }

    public async Task EnsureInitializedAsync()
    {
        try
        {
            var res = await _storage.GetAsync<string>("auth.token");
            if (res.Success && !string.IsNullOrWhiteSpace(res.Value))
            {
                _token = res.Value;
                var api = _httpFactory.CreateClient("Api");
                api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }
        }
        catch { /* ignore */ }
    }

    public async Task<bool> LoginAsync(string correo, string password)
    {
        var http = _httpFactory.CreateClient("Auth"); // sin handler
        // El API espera propiedades en minúscula: { "correo": "", "password": "" }
        var payload = new { correo = correo, password = password };
        var resp = await http.PostAsJsonAsync("/api/auth/login", payload);
        if (!resp.IsSuccessStatusCode)
        {
            try { LastError = await resp.Content.ReadAsStringAsync(); }
            catch { LastError = null; }
            return false;
        }

        var dto = await resp.Content.ReadFromJsonAsync<LoginResponseDto>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (dto is null)
        {
            LastError = "Respuesta vacía o JSON inválido.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(dto.Token))
        {
            LastError = "Login sin token en la respuesta.";
            return false;
        }

        _token = dto.Token;
        CurrentUserId = dto.UserId;
        CurrentUserRole = dto.Rol;
        CurrentUserName = dto.Nombre;
        try { await _storage.SetAsync("auth.token", _token); } catch { }

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
        LastError = null;
        try { _storage.DeleteAsync("auth.token"); } catch { }
        // No limpiamos headers de instancias previas; el handler controla el bearer.
    }

    public string? GetToken() => _token;
}

public class LoginResponseDto
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = default!;

    [JsonPropertyName("rol")]
    public string Rol { get; set; } = default!;

    [JsonPropertyName("token")]
    public string Token { get; set; } = default!;

    [JsonPropertyName("expiraUtc")]
    public DateTime ExpiraUtc { get; set; }
}

// helper class removed to avoid namespace issues; we read content inline with try/catch