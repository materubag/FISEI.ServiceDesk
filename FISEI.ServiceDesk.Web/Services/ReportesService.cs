using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class ReportesService
{
    private readonly HttpClient _http;
    public ReportesService(HttpClient http) => _http = http;

    public async Task<ReportePersonalDto> ObtenerReportePersonalAsync(int usuarioId)
        => await _http.GetFromJsonAsync<ReportePersonalDto>($"/api/reportes/personales?usuarioId={usuarioId}")
           ?? new ReportePersonalDto();
}

public class ReportePersonalDto
{
    public int Abiertos { get; set; }
    public int EnProgreso { get; set; }
    public int Resueltos { get; set; }
    public int FueraSla { get; set; }
    public List<IncidenciaHistDto> Recientes { get; set; } = new();
}

public class IncidenciaHistDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string ServicioNombre { get; set; } = "";
    public string EstadoNombre { get; set; } = "";
    public DateTime FechaCreacion { get; set; }
    public double? TiempoResolucionHoras { get; set; }
}
