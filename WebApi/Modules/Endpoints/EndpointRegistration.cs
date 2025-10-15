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
        app.MapEmpresaEndpoints();
        app.MapEstacionCocinaEndpoints();
        app.MapEventoPedidoEndpoints();
        app.MapGrupoModificadorEndpoints();
        app.MapMenuEndpoints();
        app.MapMesaEndpoints();
        app.MapMovimientoCajaEndpoints();
        app.MapOpcionModificadorEndpoints();
        app.MapPagoEndpoints();
        app.MapPedidoEndpoints();
        app.MapPedidoAsientoEndpoints();
        app.MapPedidoDetalleEndpoints();
        app.MapPedidoModificadorEndpoints();
        app.MapPrecioEndpoints();
        app.MapProductoEndpoints();
        app.MapRolEndpoints();
        app.MapSucursalEndpoints();
        app.MapTicketCocinaEndpoints();
        app.MapTicketDetalleEndpoints();
        app.MapTurnoEndpoints();
        app.MapUsuarioEndpoints();
        app.MapUsuarioRolEndpoints();
        app.MapVarianteProductoEndpoints();
        app.MapFormFieldEndpoints();
        app.MapAccesoRutaEndpoints();
        app.MapRolAccesoRutaEndpoints();
        return app;
    }
}
