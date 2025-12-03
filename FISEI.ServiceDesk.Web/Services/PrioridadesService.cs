using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class PrioridadesService
{
    private readonly HttpClient _http;
    public PrioridadesService(HttpClient http) => _http = http;

    public async Task<List<PrioridadDto>> ListarAsync()
        => await _http.GetFromJsonAsync<List<PrioridadDto>>("/api/prioridades") ?? new();

    public async Task<PrioridadDto?> ObtenerAsync(int id)
        => await _http.GetFromJsonAsync<PrioridadDto>($"/api/prioridades/{id}");
}

public class PrioridadDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}