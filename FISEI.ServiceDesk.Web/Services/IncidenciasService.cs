using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class IncidenciasService
{
    private readonly HttpClient _http;
    public IncidenciasService(HttpClient http) => _http = http;

    // Lista "mías" (creadas por el usuario)
    public async Task<List<IncidenciaResumenDto>> ListarMiasAsync(int usuarioId)
        => await _http.GetFromJsonAsync<List<IncidenciaResumenDto>>($"/api/incidencias/mias?usuarioId={usuarioId}") ?? new();

    // Lista general con filtros del catálogo (usa IncidenciasExtraController GET /api/incidencias)
    public async Task<List<IncidenciaResumenDto>> ListarCatalogoAsync(string? texto, int? estadoId, int? prioridadId, int? servicioId, int page = 1, int pageSize = 50)
    {
        var url = $"/api/incidencias?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(texto)) url += $"&search={Uri.EscapeDataString(texto)}";
        if (estadoId.HasValue) url += $"&estadoId={estadoId.Value}";
        if (prioridadId.HasValue) url += $"&prioridadId={prioridadId.Value}";
        if (servicioId.HasValue) url += $"&servicioId={servicioId.Value}";

        // El endpoint devuelve un objeto anónimo; mapear a DTO de resumen
        var rawList = await _http.GetFromJsonAsync<List<IncidenciaExtraListItem>>(url) ?? new();
        return rawList.Select(i => new IncidenciaResumenDto
        {
            Id = i.Id,
            Titulo = i.Titulo,
            Descripcion = i.Descripcion,
            EstadoId = i.EstadoId,
            PrioridadId = i.PrioridadId,
            ServicioId = i.ServicioId,
            FechaCreacion = i.FechaCreacion
        }).ToList();
    }

    // Compat de tu UI original: decide si listar "mías" o catálogo
    public async Task<List<IncidenciaResumenDto>> ListarAsync(int usuarioId, string? texto, int? estadoId, int? prioridadId)
    {
        // Si hay filtros, usa catálogo; si no, usa "mías"
        var usarCatalogo = (estadoId.HasValue || prioridadId.HasValue || !string.IsNullOrWhiteSpace(texto));
        if (usarCatalogo)
            return await ListarCatalogoAsync(texto, estadoId, prioridadId, servicioId: null);

        return await ListarMiasAsync(usuarioId);
    }

    public async Task<DashboardVmDto> GetDashboardAsync(int usuarioId)
    {
        var list = await ListarMiasAsync(usuarioId);
        var vm = new DashboardVmDto
        {
            Activos = list.Count,
            Recientes = list.OrderByDescending(i => i.FechaCreacion).Take(5).ToList(),
            EnProgreso = list.Count(i => (i.EstadoNombre?.Contains("PROGRES", StringComparison.OrdinalIgnoreCase) ?? false)
                                      || (i.EstadoNombre?.Contains("ABIER", StringComparison.OrdinalIgnoreCase) ?? false)),
            ResueltosMes = list.Count(i => (i.EstadoNombre?.Contains("RESUELT", StringComparison.OrdinalIgnoreCase) ?? false)
                                        && i.FechaCreacion >= DateTime.UtcNow.AddDays(-30)),
            Criticos = list.Count(i => i.PrioridadNombre?.Contains("CRITIC", StringComparison.OrdinalIgnoreCase) == true
                                    || i.PrioridadNombre?.Contains("RÍTIC", StringComparison.OrdinalIgnoreCase) == true)
        };
        return vm;
    }

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

    // Comentarios: ruta unificada /api/incidencias/{id}/comentarios
    public async Task<List<ComentarioDto>> ListarComentariosAsync(int incidenciaId)
        => await _http.GetFromJsonAsync<List<ComentarioDto>>($"/api/incidencias/{incidenciaId}/comentarios") ?? new();

    public async Task AgregarComentarioAsync(int incidenciaId, ComentarioCrearDto dto)
    {
        var resp = await _http.PostAsJsonAsync($"/api/incidencias/{incidenciaId}/comentarios", dto);
        resp.EnsureSuccessStatusCode(); // espera 201 Created
    }
}

// Modelos auxiliares para mapear la respuesta del catálogo (IncidenciasExtraController GET /api/incidencias)
public class IncidenciaExtraListItem
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string? Descripcion { get; set; }
    public int EstadoId { get; set; }
    public int PrioridadId { get; set; }
    public int ServicioId { get; set; }
    public int CreadorId { get; set; }
    public int? TecnicoAsignadoId { get; set; }
    public DateTime FechaCreacion { get; set; }
}

// DTOs usados por la UI (mantengo tus definiciones)
public class IncidenciaResumenDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int ServicioId { get; set; }
    public int PrioridadId { get; set; }
    public int EstadoId { get; set; }
    public string? ServicioNombre { get; set; }
    public string? PrioridadNombre { get; set; }
    public string? EstadoNombre { get; set; }
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
    public int ServicioId { get; set; }
    public int PrioridadId { get; set; }
    public int EstadoId { get; set; }
    public string? ServicioNombre { get; set; }
    public string? PrioridadNombre { get; set; }
    public string? EstadoNombre { get; set; }
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
    public int UsuarioId { get; set; } // asegúrate de enviar el UsuarioId
    public string Texto { get; set; } = "";
    public bool EsInterno { get; set; }
}