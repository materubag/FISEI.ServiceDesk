using FISEI.ServiceDesk.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Configura aquí la URL base de tu API
var apiBaseUrl = "https://localhost:5031/";

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Servicio de autenticación (scope por circuito/usuario)
builder.Services.AddScoped<AuthClientService>();

// Handler que inyecta el token en cada request
builder.Services.AddScoped<AuthHeaderHandler>();

// HttpClient de la API con base address y el handler de auth
builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<AuthHeaderHandler>();

// Al inyectar HttpClient sin nombre, devolvemos el "Api"
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

// Servicio de notificaciones SignalR
builder.Services.AddSingleton<NotificacionesService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();