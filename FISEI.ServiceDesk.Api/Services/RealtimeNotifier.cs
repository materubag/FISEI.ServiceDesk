using FISEI.ServiceDesk.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FISEI.ServiceDesk.Api.Services;

public class RealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<NotificacionesHub> _hub;
    public RealtimeNotifier(IHubContext<NotificacionesHub> hub) => _hub = hub;

    public Task NotifyAllAsync(string message)
        => _hub.Clients.All.SendAsync("Notificacion", message);

    public Task NotifyUserAsync(string userId, string message)
        => _hub.Clients.Group($"USER_{userId}").SendAsync("Notificacion", message);

    public Task NotifyUserAsync(int userId, string message)
        => _hub.Clients.Group($"USER_{userId}").SendAsync("Notificacion", message);

    public Task NotifyTecnicosAsync(string message)
        => _hub.Clients.Group("ROL_TECNICO").SendAsync("Notificacion", message);
}
public interface IRealtimeNotifier
{
    Task NotifyAllAsync(string message);
    Task NotifyUserAsync(string userId, string message);
    Task NotifyUserAsync(int userId, string message);
    Task NotifyTecnicosAsync(string message);
}

