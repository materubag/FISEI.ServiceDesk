using Microsoft.AspNetCore.SignalR.Client;

namespace FISEI.ServiceDesk.Web.Services;

public class NotificacionesService : IAsyncDisposable
{
    private HubConnection? _hub;
    public event Action<string>? OnMensajeCrudo;

    public async Task ConectarAsync(string hubUrl, Func<Task<string?>> accessTokenProvider)
    {
        _hub = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = accessTokenProvider;
            })
            .WithAutomaticReconnect()
            .Build();

        _hub.On<string>("Notificacion", msg => OnMensajeCrudo?.Invoke(msg));
        await _hub.StartAsync();
    }

    public bool Conectado => _hub?.State == HubConnectionState.Connected;

    public async ValueTask DisposeAsync()
    {
        if (_hub != null) await _hub.DisposeAsync();
    }
}