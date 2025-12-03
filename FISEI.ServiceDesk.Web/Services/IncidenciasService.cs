using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class IncidenciasService
{
    private readonly HttpClient _http;
    public IncidenciasService(HttpClient http) => _http = http;

    public async Task<List<IncidenciaResumenDto>> ListarAsync(int usuarioId, string? texto, int? estadoId, int? prioridadId)
    {
        var url = $"/api/incidencias?usuarioId={usuarioId}";
        if (!string.IsNullOrWhiteSpace(texto)) url += $"&q={Uri.EscapeDataString(texto)}";
        if (estadoId.HasValue) url += $"&estadoId={estadoId}";
        if (prioridadId.HasValue) url += $"&prioridadId={prioridadId}";
        return await _http.GetFromJsonAsync<List<IncidenciaResumenDto>>(url) ?? new();
    }

    public async Task<DashboardVmDto> GetDashboardAsync(int usuarioId)
        => await _http.GetFromJsonAsync<DashboardVmDto>($"/api/incidencias/dashboard?usuarioId={usuarioId}") ?? new DashboardVmDto();

    public async Task<int> CrearAsync(NuevaIncidenciaDto dto)
    {
        var resp = await _http.PostAsJsonAsync("/api/incidencias", dto);
        resp.EnsureSuccessStatusCode();
        var creado = await resp.Content.ReadFromJsonAsync<IncidenciaDto>();
        if (creado is null) throw new InvalidOperationException("Respuesta de creación inválida");
        return creado.Id;
    }

    public async Task<IncidenciaDetalleDto?> ObtenerDetalleAsync(int id)
        => await _http.GetFromJsonAsync<IncidenciaDetalleDto>($"/api/incidencias/{id}");

    public async Task AgregarComentarioAsync(int id, ComentarioCrearDto dto)
    {
        var resp = await _http.PostAsJsonAsync($"/api/incidencias/{id}/comentarios", dto);
        resp.EnsureSuccessStatusCode();
    }
}

public class IncidenciaResumenDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string ServicioNombre { get; set; } = "";
    public string PrioridadNombre { get; set; } = "";
    public string EstadoNombre { get; set; } = "";
    public DateTime FechaCreacion { get; set; }
}

public class DashboardVmDto
{
    public int Activos { get; set; }
    public int EnProgreso { get; set; }
    public int ResueltosMes { get; set; }
    public int Criticos { get; set; }
    public List<IncidenciaResumenDto> Recientes { get; set; } = new();
}

public class NuevaIncidenciaDto
{
    public int CreadorId { get; set; }
    public int ServicioId { get; set; }
    public int PrioridadId { get; set; }
    public string Titulo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int? LaboratorioId { get; set; }
}

public class IncidenciaDetalleDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string ServicioNombre { get; set; } = "";
    public string PrioridadNombre { get; set; } = "";
    public string EstadoNombre { get; set; } = "";
    public List<ComentarioDto> Comentarios { get; set; } = new();
}

public class IncidenciaDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = "";
    public string Titulo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int EstadoId { get; set; }
    public int PrioridadId { get; set; }
    public int ServicioId { get; set; }
    public int CreadorId { get; set; }
    public int? TecnicoAsignadoId { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class ComentarioDto
{
    public string AutorNombre { get; set; } = "";
    public DateTime Fecha { get; set; }
    public bool EsInterno { get; set; }
    public string Texto { get; set; } = "";
}

public class ComentarioCrearDto
{
    public string Texto { get; set; } = "";
    public bool EsInterno { get; set; }
}
