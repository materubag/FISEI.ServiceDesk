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

    public async Task<List<IncidenciaResumenDto>> ListarAsync(int usuarioId, string? texto, int? estadoId, int? prioridadId)
    {
        // Usa endpoint existente: /api/incidencias/mias?usuarioId=...
        var baseList = await _http.GetFromJsonAsync<List<IncidenciaResumenDto>>($"/api/incidencias/mias?usuarioId={usuarioId}")
                       ?? new List<IncidenciaResumenDto>();

        // Filtros en cliente para compatibilidad
        if (!string.IsNullOrWhiteSpace(texto))
        {
            var t = texto.Trim();
            baseList = baseList.FindAll(i =>
                (i.Titulo?.Contains(t, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (i.Descripcion?.Contains(t, StringComparison.OrdinalIgnoreCase) ?? false));
        }
        if (estadoId.HasValue) baseList = baseList.FindAll(i => i.EstadoId == estadoId.Value);
        if (prioridadId.HasValue) baseList = baseList.FindAll(i => i.PrioridadId == prioridadId.Value);

        return baseList;
    }

    public async Task<List<IncidenciaResumenDto>> ListarTodasAsync(string? texto, int? estadoId, int? prioridadId, int page = 1, int pageSize = 200)
    {
        var qs = new List<string>();
        if (!string.IsNullOrWhiteSpace(texto)) qs.Add($"search={Uri.EscapeDataString(texto)}");
        if (estadoId.HasValue) qs.Add($"estadoId={estadoId.Value}");
        if (prioridadId.HasValue) qs.Add($"prioridadId={prioridadId.Value}");
        if (page > 0) qs.Add($"page={page}");
        if (pageSize > 0) qs.Add($"pageSize={pageSize}");
        var query = qs.Count > 0 ? ("?" + string.Join("&", qs)) : string.Empty;

        var list = await _http.GetFromJsonAsync<List<IncidenciaResumenDto>>($"/api/incidencias{query}")
                   ?? new List<IncidenciaResumenDto>();
        return list;
    }

    public async Task<DashboardVmDto> GetDashboardAsync(int usuarioId)
    {
        // Construye un dashboard básico desde "mias"
        var list = await _http.GetFromJsonAsync<List<IncidenciaResumenDto>>($"/api/incidencias/mias?usuarioId={usuarioId}")
                   ?? new List<IncidenciaResumenDto>();

        var vm = new DashboardVmDto();
        vm.Activos = list.Count;
        vm.Recientes = list.OrderByDescending(i => i.FechaCreacion).Take(5).ToList();

        // Estimaciones simples basadas en nombres si están presentes
        vm.EnProgreso = list.Count(i => (i.EstadoNombre?.Contains("PROGRES", StringComparison.OrdinalIgnoreCase) ?? false)
                                     || (i.EstadoNombre?.Contains("ABIER", StringComparison.OrdinalIgnoreCase) ?? false));
        vm.ResueltosMes = list.Count(i => (i.EstadoNombre?.Contains("RESUELT", StringComparison.OrdinalIgnoreCase) ?? false)
                                       && i.FechaCreacion >= DateTime.UtcNow.AddDays(-30));
        vm.Criticos = list.Count(i => i.PrioridadNombre?.Contains("RÍTIC", StringComparison.OrdinalIgnoreCase) == true
                                   || i.PrioridadNombre?.Contains("CRITIC", StringComparison.OrdinalIgnoreCase) == true);

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
    {
        var detalle = await _http.GetFromJsonAsync<IncidenciaDetalleDto>($"/api/incidencias/{id}");
        if (detalle is null) return null;

        try
        {
            // GET comentarios desde controlador ComentariosIncidencia
            var apiComentarios = await _http.GetFromJsonAsync<List<ApiComentario>>($"/api/incidencias/{id}/ComentariosIncidencia")
                                 ?? new List<ApiComentario>();

            var cacheNombres = new Dictionary<int, string>();
            var list = new List<ComentarioDto>();
            foreach (var c in apiComentarios)
            {
                if (!cacheNombres.TryGetValue(c.UsuarioId, out var nombre))
                {
                    var u = await _http.GetFromJsonAsync<ApiUsuario>($"/api/usuarios/{c.UsuarioId}");
                    nombre = u?.Nombre ?? $"Usuario #{c.UsuarioId}";
                    cacheNombres[c.UsuarioId] = nombre;
                }
                list.Add(new ComentarioDto
                {
                    AutorNombre = nombre,
                    Fecha = c.Fecha,
                    EsInterno = c.EsInterno,
                    Texto = c.Texto
                });
            }
            detalle.Comentarios = list;
        }
        catch
        {
            // Ignorar errores de comentarios para no romper el detalle
        }

        return detalle;
    }

    public async Task AgregarComentarioAsync(int id, ComentarioCrearDto dto)
    {
        var resp = await _http.PostAsJsonAsync($"/api/incidencias/{id}/comentarios", dto);
        resp.EnsureSuccessStatusCode();
    }

    public async Task CambiarEstadoAsync(int id, int nuevoEstadoId, int actorId, string comentario)
    {
        var payload = new CambiarEstadoDto
        {
            NuevoEstadoId = nuevoEstadoId,
            ActorId = actorId,
            Comentario = comentario
        };

        using var req = new HttpRequestMessage(HttpMethod.Patch, $"/api/incidencias/{id}/estado")
        {
            Content = JsonContent.Create(payload)
        };
        var resp = await _http.SendAsync(req);
        resp.EnsureSuccessStatusCode();
    }
}

public class IncidenciaResumenDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }

    // IDs reales del backend
    public int ServicioId { get; set; }
    public int PrioridadId { get; set; }
    public int EstadoId { get; set; }

    // Nombres opcionales (si el backend los envía)
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
    public string Texto { get; set; } = "";
    public bool EsInterno { get; set; }
}

public class CambiarEstadoDto
{
    public int NuevoEstadoId { get; set; }
    public int ActorId { get; set; }
    public string Comentario { get; set; } = "";
}

// Tipos auxiliares de consumo de API
internal class ApiComentario
{
    public int UsuarioId { get; set; }
    public string Texto { get; set; } = "";
    public bool EsInterno { get; set; }
    public DateTime Fecha { get; set; }
}

internal class ApiUsuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}
