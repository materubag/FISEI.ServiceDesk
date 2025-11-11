using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FISEI.ServiceDesk.Api.Hubs;

public class NotificacionesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) 
                         ?? user.FindFirstValue(ClaimTypes.Name) // fallback
                         ?? user.FindFirst("sub")?.Value;

            if (!string.IsNullOrEmpty(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"USER_{userId}");

            var role = user.FindFirstValue(ClaimTypes.Role);
            if (role == "Tecnico" || role == "Administrador")
                await Groups.AddToGroupAsync(Context.ConnectionId, "ROL_TECNICO");
        }
        await base.OnConnectedAsync();
    }
}