using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class CategoriasService
{
    private readonly HttpClient _http;
    public CategoriasService(HttpClient http) => _http = http;

    // Ajusta la ruta si tu API expone /api/kb/categorias
    public async Task<List<CategoriaDto>> ListarAsync()
        => await _http.GetFromJsonAsync<List<CategoriaDto>>("/api/categorias") ?? new();
}

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}
