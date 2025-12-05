using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class EstadosService
{
    private readonly HttpClient _http;
    public EstadosService(HttpClient http) => _http = http;

    // Ajusta esta ruta al path REAL de tu API:
    // Ejemplo 1 (kebab-case): "/api/estados-incidencia"
    // Ejemplo 2 (PascalCase): "/api/EstadosIncidencia"
    private const string BasePath = "/api/EstadosIncidencia";

    public async Task<List<EstadoDto>> ListarAsync()
    {
        var resp = await _http.GetAsync(BasePath);
        if (resp.StatusCode == HttpStatusCode.NoContent) return new();
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<List<EstadoDto>>() ?? new();
    }

    public async Task<EstadoDto?> ObtenerAsync(int id)
        => await _http.GetFromJsonAsync<EstadoDto>($"{BasePath}/{id}");
}

public class EstadoDto
{
    public int Id { get; set; }
    // Algunos controladores exponen 'Codigo' en lugar de 'Nombre'
    public string? Nombre { get; set; }
    public string? Codigo { get; set; }
    public bool EsFinal { get; set; }
}