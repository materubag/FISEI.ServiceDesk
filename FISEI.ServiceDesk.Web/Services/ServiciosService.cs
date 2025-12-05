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

    public async Task<ServicioDto?> ObtenerAsync(int id)
        => await _http.GetFromJsonAsync<ServicioDto>($"/api/servicios/{id}");

    public async Task<ServicioDto?> CrearAsync(ServicioCreateDto payload)
    {
        var resp = await _http.PostAsJsonAsync("/api/servicios", payload);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<ServicioDto>();
    }

    public async Task<bool> ActualizarAsync(int id, ServicioUpdateDto payload)
    {
        var resp = await _http.PutAsJsonAsync($"/api/servicios/{id}", payload);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var resp = await _http.DeleteAsync($"/api/servicios/{id}");
        return resp.IsSuccessStatusCode;
    }
}

public class ServicioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public int CategoriaId { get; set; }
    public bool Activo { get; set; }
}

public class ServicioCreateDto
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = "";
    public bool Activo { get; set; } = true;
}

public class ServicioUpdateDto
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = "";
    public bool Activo { get; set; } = true;
}