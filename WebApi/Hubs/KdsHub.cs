using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs;

public class KdsHub : Hub
{
    // Hub vacio, los eventos se emitiran desde el Controlador o Aplicacion usando IHubContext<KdsHub>
}
