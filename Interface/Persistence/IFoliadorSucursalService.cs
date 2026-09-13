namespace Interface.Persistence;

/// <summary>
/// Spec 019: genera folios humanos correlativos (por sucursal/día) de forma atómica
/// y segura ante concurrencia, sin leer-incrementar-guardar en memoria.
/// </summary>
public interface IFoliadorSucursalService
{
    Task<int> ObtenerSiguienteFolioAsync(int idSucursal, DateOnly? fecha = null, CancellationToken cancellationToken = default);
}
