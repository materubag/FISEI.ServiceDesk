using Microsoft.AspNetCore.SignalR.Client;

namespace FISEI.ServiceDesk.Web.Services;

public class NotificacionesService : IAsyncDisposable
{
    private HubConnection? _connection;

    public event Action<string>? OnMensaje;

    public async Task IniciarAsync(string hubUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string>("Notificacion", m => OnMensaje?.Invoke(m));

        await _connection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}