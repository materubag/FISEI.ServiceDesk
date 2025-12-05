using FISEI.ServiceDesk.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
    ?? (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
        ? "http://api:8080/"
        : "http://localhost:8070/");

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
// Registrar almacenamiento protegido directamente cuando la extensión no está disponible
builder.Services.AddScoped<ProtectedLocalStorage>();

builder.Services.AddScoped<AuthClientService>();
builder.Services.AddScoped<AuthHeaderHandler>();

// Cliente para AUTH (sin handler)
builder.Services.AddHttpClient("Auth", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Cliente para API (con bearer automático)
builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthHeaderHandler>();

// HttpClient “por defecto” usará el cliente Api
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

builder.Services.AddSingleton<NotificacionesService>();
builder.Services.AddScoped<UsuariosService>();
builder.Services.AddScoped<UsuariosLookupService>();
builder.Services.AddScoped<KnowledgeBaseService>();
builder.Services.AddScoped<ReportesService>();
builder.Services.AddScoped<NotificacionesApiService>();
builder.Services.AddScoped<ServiciosService>();
builder.Services.AddScoped<EstadosService>();
builder.Services.AddScoped<PrioridadesService>();
builder.Services.AddScoped<CategoriasService>();
builder.Services.AddScoped<LaboratoriosService>();
builder.Services.AddScoped<IncidenciasService>();
builder.Services.AddScoped<ServiciosService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();