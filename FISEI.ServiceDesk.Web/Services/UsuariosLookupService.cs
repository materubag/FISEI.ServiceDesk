using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class UsuariosLookupService
{
    private readonly HttpClient _http;
    public UsuariosLookupService(HttpClient http) => _http = http;

    // Endpoint sugerido para resolver nombre por Id
    public async Task<UsuarioMiniDto?> ObtenerAsync(int id)
    {
        var resp = await _http.GetAsync($"/api/usuarios/{id}/mini");
        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<UsuarioMiniDto>();
    }
}

public class UsuarioMiniDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}