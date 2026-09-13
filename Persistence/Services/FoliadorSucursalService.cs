using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Services;

/// <summary>
/// Implementación del foliador de Spec 019. Usa un UPSERT atómico de PostgreSQL
/// (INSERT ... ON CONFLICT DO UPDATE ... RETURNING) para incrementar el folio del día
/// en una sola sentencia SQL, evitando condiciones de carrera entre terminales concurrentes
/// (POS/Edge) sin necesidad de bloqueos explícitos en el código de aplicación.
/// </summary>
public class FoliadorSucursalService : IFoliadorSucursalService
{
    private readonly ApplicationDbContext _context;

    public FoliadorSucursalService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> ObtenerSiguienteFolioAsync(int idSucursal, DateOnly? fecha = null, CancellationToken cancellationToken = default)
    {
        var dia = fecha ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var resultado = await _context.Database.SqlQuery<int>(
            $@"INSERT INTO ""FoliadorSucursal"" (""IdSucursal"", ""Fecha"", ""UltimoFolio"", ""IsActive"")
               VALUES ({idSucursal}, {dia}, 1, true)
               ON CONFLICT (""IdSucursal"", ""Fecha"")
               DO UPDATE SET ""UltimoFolio"" = ""FoliadorSucursal"".""UltimoFolio"" + 1
               RETURNING ""UltimoFolio"" AS ""Value""")
            .ToListAsync(cancellationToken);

        return resultado.Single();
    }
}
