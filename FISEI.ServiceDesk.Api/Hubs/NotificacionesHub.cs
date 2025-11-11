using Microsoft.AspNetCore.SignalR;

namespace FISEI.ServiceDesk.Api.Hubs;

public class NotificacionesHub : Hub
{
    public override Task OnConnectedAsync()
    {
        // Aquí puedes agregar al usuario a grupos por rol/Id si usas auth
        return base.OnConnectedAsync();
    }
}