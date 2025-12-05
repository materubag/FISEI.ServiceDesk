using System;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class LaboratoriosService
{
    private readonly HttpClient _http;
    public LaboratoriosService(HttpClient http) => _http = http;

    private const string BasePath = "/api/Laboratorios"; // consistente con el controlador

    public async Task<List<LaboratorioDto>> ListarAsync(bool? activo = null)
    {
        var url = activo.HasValue ? $"{BasePath}?activo={(activo.Value ? "true" : "false")}" : BasePath;
        return await _http.GetFromJsonAsync<List<LaboratorioDto>>(url) ?? new();
    }

    public async Task<LaboratorioDto?> ObtenerAsync(int id)
        => await _http.GetFromJsonAsync<LaboratorioDto>($"{BasePath}/{id}");

    public async Task<int> CrearAsync(LaboratorioEditDto dto)
    {
        var resp = await _http.PostAsJsonAsync(BasePath, dto);
        resp.EnsureSuccessStatusCode();
        var creado = await resp.Content.ReadFromJsonAsync<LaboratorioDto>();
        if (creado is null) throw new InvalidOperationException("Respuesta vacía al crear laboratorio");
        return creado.Id;
    }

    public async Task ActualizarAsync(int id, LaboratorioEditDto dto)
    {
        var resp = await _http.PutAsJsonAsync($"{BasePath}/{id}", dto);
        resp.EnsureSuccessStatusCode();
    }

    public async Task EliminarAsync(int id)
    {
        var resp = await _http.DeleteAsync($"{BasePath}/{id}");
        resp.EnsureSuccessStatusCode();
    }
}

public class LaboratorioDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string? Edificio { get; set; }
    public string? Ubicacion { get; set; }
    public bool Activo { get; set; }
}

public class LaboratorioEditDto
{
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string? Edificio { get; set; }
    public string? Ubicacion { get; set; }
    public bool Activo { get; set; } = true;
}
