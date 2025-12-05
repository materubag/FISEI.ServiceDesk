using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class UsuariosService
{
    private readonly HttpClient _http;
    public UsuariosService(HttpClient http) => _http = http;

    public async Task<UsuarioPerfilDto?> ObtenerPerfilAsync(int usuarioId)
        => await _http.GetFromJsonAsync<UsuarioPerfilDto>($"/api/usuarios/{usuarioId}");

    public async Task ActualizarPerfilAsync(UsuarioPerfilDto dto)
    {
        var resp = await _http.PutAsJsonAsync($"/api/usuarios/{dto.Id}", dto);
        resp.EnsureSuccessStatusCode();
    }

    public async Task CambiarPasswordAsync(CambiarPasswordDto dto)
    {
        var resp = await _http.PostAsJsonAsync($"/api/usuarios/{dto.UsuarioId}/password", dto);
        resp.EnsureSuccessStatusCode();
    }
}

public class UsuarioPerfilDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public string RolNombre { get; set; } = "";
}

public class CambiarPasswordDto
{
    public int UsuarioId { get; set; }
    public string PasswordActual { get; set; } = "";
    public string PasswordNueva { get; set; } = "";
    public string PasswordNuevaConfirmacion { get; set; } = "";
}
