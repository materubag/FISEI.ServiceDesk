using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class LaboratoriosService
{
    private readonly HttpClient _http;
    public LaboratoriosService(HttpClient http) => _http = http;

    // Ajusta la ruta si tu API expone otra (p. ej. /api/labs)
    public async Task<List<LaboratorioDto>> ListarAsync()
        => await _http.GetFromJsonAsync<List<LaboratorioDto>>("/api/laboratorios") ?? new();

    public async Task<LaboratorioDto?> ObtenerAsync(int id)
        => await _http.GetFromJsonAsync<LaboratorioDto>($"/api/laboratorios/{id}");
}

public class LaboratorioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}
