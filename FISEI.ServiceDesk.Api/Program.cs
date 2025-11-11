using FISEI.ServiceDesk.Api.Hubs;
using FISEI.ServiceDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection string (appsettings o variable de entorno SD_CONNECTION_STRING)
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? Environment.GetEnvironmentVariable("SD_CONNECTION_STRING")
         ?? "Server=(localdb)\\MSSQLLocalDB;Database=ServiceDeskDB;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<ServiceDeskDbContext>(opt =>
    opt.UseSqlServer(cs));

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS de desarrollo (ajusta origen según el puerto de tu Blazor Web)
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("DevCors", p =>
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         .SetIsOriginAllowed(_ => true) // Solo DEV
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevCors");

app.MapControllers();
app.MapHub<NotificacionesHub>("/hubs/notificaciones");

app.Run();