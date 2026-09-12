using System.Reflection;

namespace MesaFacil.API.Modules.Endpoints;

public static class EndpointRegistration
{
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        // 1. Obtenemos todas las clases estáticas (Abstract y Sealed en IL) que terminen en "Endpoints"
        var whitelist = new[] { "AccesoRutaEndpoints", "RolAccesoRutaEndpoints" };
        var endpointClasses = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsAbstract && t.IsSealed && whitelist.Contains(t.Name));

        foreach (var type in endpointClasses)
        {
            // 2. Buscamos un método público y estático cuyo nombre sea "Map" + NombreDeLaClase
            // Ej: Si la clase es "CuentaEndpoints", busca "MapCuentaEndpoints"
            var method = type.GetMethod($"Map{type.Name}", BindingFlags.Static | BindingFlags.Public);

            if (method != null)
            {
                // 3. Invocamos el método pasando 'app' como parámetro
                method.Invoke(null, new object[] { app });
            }
        }

        return app;
    }
}