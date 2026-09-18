using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs;

public class MesasHub : Hub
{
    public async Task JoinSucursalGroup(int idSucursal)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"sucursal-{idSucursal}");
    }

    public async Task LeaveSucursalGroup(int idSucursal)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"sucursal-{idSucursal}");
    }
}
