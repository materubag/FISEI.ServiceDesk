using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class KnowledgeBaseService
{
    private readonly HttpClient _http;
    public KnowledgeBaseService(HttpClient http) => _http = http;

    // Detalle
    public async Task<KbArticuloDto?> ObtenerAsync(int id)
    {
        var resp = await _http.GetAsync($"/api/kb/{id}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<KbArticuloDto>();
    }

    public async Task VotarAsync(int id, int score)
    {
        var resp = await _http.PostAsJsonAsync($"/api/kb/{id}/votar", new { Puntuacion = score });
        resp.EnsureSuccessStatusCode();
    }

    public async Task EnviarFeedbackAsync(int id, KbFeedbackCrearDto dto)
    {
        var resp = await _http.PostAsJsonAsync($"/api/kb/{id}/feedback", dto);
        resp.EnsureSuccessStatusCode();
    }

    public async Task ActualizarAsync(int id, KbArticuloDto dto)
    {
        var updateDto = new { dto.Titulo, dto.Contenido, dto.Referencias, dto.Etiquetas, dto.ServicioId, dto.LaboratorioId };
        var resp = await _http.PutAsJsonAsync($"/api/kb/{id}", updateDto);
        resp.EnsureSuccessStatusCode();
    }

    // Crear artículo (vinculado a incidencia)
    public async Task<int> CrearDesdeIncidenciaAsync(int incidenciaId, KbCrearDto dto)
    {
        var resp = await _http.PostAsJsonAsync($"/api/incidencias/{incidenciaId}/kb", dto);
        if (resp.StatusCode == HttpStatusCode.Conflict)
        {
            throw new HttpRequestException("Ya existe un artículo con este título.", null, HttpStatusCode.Conflict);
        }
        resp.EnsureSuccessStatusCode();
        var creado = await resp.Content.ReadFromJsonAsync<KbArticuloDto>();
        return creado?.Id ?? 0;
    }

    // Lista/búsqueda (robusto a objeto paginado o array simple)
    public async Task<PagedResult<KbArticuloResumenDto>> BuscarAsync(
        string? query, int? servicioId, int? laboratorioId, int? autorId, int page, int pageSize)
    {
        var url = $"/api/kb?query={Uri.EscapeDataString(query ?? "")}&page={page}&pageSize={pageSize}";
        if (servicioId.HasValue) url += $"&servicioId={servicioId.Value}";
        if (laboratorioId.HasValue) url += $"&laboratorioId={laboratorioId.Value}";
        if (autorId.HasValue) url += $"&autorId={autorId.Value}";

        var resp = await _http.GetAsync(url);
        if (resp.StatusCode == HttpStatusCode.NoContent)
            return new PagedResult<KbArticuloResumenDto> { Items = new(), Total = 0, Page = page, PageSize = pageSize };

        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync();

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        try
        {
            var paged = JsonSerializer.Deserialize<PagedResult<KbArticuloResumenDto>>(json, opts);
            if (paged?.Items != null) return paged;
        }
        catch { /* intentar como array */ }

        var arr = JsonSerializer.Deserialize<List<KbArticuloResumenDto>>(json, opts) ?? new();
        return new PagedResult<KbArticuloResumenDto> { Items = arr, Total = arr.Count, Page = page, PageSize = pageSize };
    }
}

// DTOs lista/paginación
public class KbArticuloResumenDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string? Extracto { get; set; }
    public string? Etiquetas { get; set; }
    public string? Referencias { get; set; }

    public int? ServicioId { get; set; }
    public int? LaboratorioId { get; set; }
    public int AutorId { get; set; }
    public int? IncidenciaOrigenId { get; set; }

    public DateTime FechaCreacion { get; set; }
    public DateTime UltimaActualizacion { get; set; }
    public bool Activo { get; set; }

    // Nombres opcionales si la API los trae
    public string? ServicioNombre { get; set; }
    public string? LaboratorioNombre { get; set; }
    public string? AutorNombre { get; set; }

    // Métricas (si aplica)
    public int Votos { get; set; }
    public double PromedioVotos { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int? Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

// DTO detalle
public class KbArticuloDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Contenido { get; set; } = "";        // texto/markdown
    public string? ContenidoHtml { get; set; }         // si la API ya lo renderiza
    public string? Referencias { get; set; }
    public string? Etiquetas { get; set; }
    public int? ServicioId { get; set; }
    public int? LaboratorioId { get; set; }
    public int AutorId { get; set; }
    public int? IncidenciaOrigenId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime UltimaActualizacion { get; set; }
    public bool Activo { get; set; }

    public string? ServicioNombre { get; set; }
    public string? LaboratorioNombre { get; set; }
    public string? AutorNombre { get; set; }

    public int Votos { get; set; }
    public double PromedioVotos { get; set; }
}

public class KbCrearDto
{
    public string Titulo { get; set; } = "";
    public string Contenido { get; set; } = "";
    public string? Referencias { get; set; }
    public string? Etiquetas { get; set; }
    public int? ServicioId { get; set; }
    public int? LaboratorioId { get; set; }
    public int AutorId { get; set; }
    public int? IncidenciaOrigenId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime UltimaActualizacion { get; set; }
}

public class KbFeedbackCrearDto
{
    public string Texto { get; set; } = "";
}