using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs;

public class KdsHub : Hub
{
    public async Task JoinStationGroup(int estacionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"estacion-{estacionId}");
    }

    public async Task LeaveStationGroup(int estacionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"estacion-{estacionId}");
    }

    public async Task JoinExpoGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "expo");
    }

    public async Task LeaveExpoGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "expo");
    }
}
