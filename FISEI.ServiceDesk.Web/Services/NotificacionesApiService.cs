using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class NotificacionesApiService
{
    private readonly HttpClient _http;
    public NotificacionesApiService(HttpClient http) => _http = http;

    public async Task<List<NotificacionDto>> ListarAsync(int usuarioId)
        => await _http.GetFromJsonAsync<List<NotificacionDto>>($"/api/notificaciones?usuarioId={usuarioId}") ?? new();

    public async Task<int> ContarNoLeidasAsync(int usuarioId)
        => await _http.GetFromJsonAsync<int>($"/api/notificaciones/unread-count?usuarioId={usuarioId}");

    public async Task MarcarLeidaAsync(int notificacionId)
    {
        var resp = await _http.PatchAsync($"/api/notificaciones/{notificacionId}/leer", content: null);
        resp.EnsureSuccessStatusCode();
    }

    public async Task MarcarTodasLeidasAsync(int usuarioId)
    {
        var resp = await _http.PostAsync($"/api/notificaciones/leer-todas?usuarioId={usuarioId}", content: null);
        resp.EnsureSuccessStatusCode();
    }
}

public class NotificacionDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Mensaje { get; set; } = "";
    public DateTime Fecha { get; set; }
    public bool Leida { get; set; }
}
