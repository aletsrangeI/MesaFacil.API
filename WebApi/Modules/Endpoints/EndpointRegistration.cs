using Microsoft.AspNetCore.Routing;

namespace MesaFacil.API.Modules.Endpoints;

public static class EndpointRegistration
{
    public static IEndpointRouteBuilder MapMesaFacilEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCatalogoEndpoints();
        app.MapAreaEndpoints();
        app.MapCategoriaMenuEndpoints();
        app.MapClienteEndpoints();
        app.MapAuthEndpoints();
        app.MapCuentaEndpoints();

        app.MapDescuentoAplicadoEndpoints();
        app.MapDetalleCuentaEndpoints();
        return app;
    }
}
