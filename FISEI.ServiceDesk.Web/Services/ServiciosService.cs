using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class ServiciosService
{
    private readonly HttpClient _http;
    public ServiciosService(HttpClient http) => _http = http;

    public async Task<List<ServicioDto>> ListarAsync()
        => await _http.GetFromJsonAsync<List<ServicioDto>>("/api/servicios") ?? new();
}

public class ServicioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
}
