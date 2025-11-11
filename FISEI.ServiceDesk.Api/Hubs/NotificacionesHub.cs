using Microsoft.AspNetCore.SignalR;

namespace FISEI.ServiceDesk.Api.Hubs;

public class NotificacionesHub : Hub
{
    public override Task OnConnectedAsync()
    {
        // TODO: Cuando agregues autenticación, agrega a grupos por usuario/rol
        return base.OnConnectedAsync();
    }
}