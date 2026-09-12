using System.Text.Json;
using Common;
using Domain.Entities;
using Interface.UseCases;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.Inventario;

public class DescuentoInventarioService : IDescuentoInventarioService
{
    private readonly ApplicationDbContext _context;
    private readonly IAppLogger<DescuentoInventarioService> _logger;

    public DescuentoInventarioService(ApplicationDbContext context, IAppLogger<DescuentoInventarioService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> DescontarPorCuentaPagadaAsync(int idCuenta)
    {
        try
        {
            var cuenta = await _context.Cuentas
                .Include(c => c.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.Modificadores)
                .Include(c => c.Pedido)
                    .ThenInclude(p => p.Sucursal)
                .FirstOrDefaultAsync(c => c.Id == idCuenta);

            if (cuenta == null || cuenta.Pedido == null)
            {
                _logger.LogWarning($"No se encontró la cuenta #{idCuenta} o su pedido asociado para descuento de inventario.");
                return false;
            }

            return await DescontarPorPedidoInternalAsync(cuenta.Pedido, idCuenta);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al procesar descuento de inventario para cuenta #{idCuenta}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DescontarPorPedidoAsync(int idPedido)
    {
        try
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Modificadores)
                .Include(p => p.Sucursal)
                .FirstOrDefaultAsync(p => p.Id == idPedido);

            if (pedido == null)
            {
                _logger.LogWarning($"No se encontró el pedido #{idPedido} para descuento de inventario.");
                return false;
            }

            return await DescontarPorPedidoInternalAsync(pedido, null);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al procesar descuento de inventario para pedido #{idPedido}: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> DescontarPorPedidoInternalAsync(Pedido pedido, int? idCuenta)
    {
        // 1. Determinar almacén de cocina o principal de la sucursal
        var almacen = await _context.Almacenes
            .FirstOrDefaultAsync(a => a.IdSucursal == pedido.IdSucursal && a.TipoAlmacen == "Cocina" && a.IsActive)
            ?? await _context.Almacenes
            .FirstOrDefaultAsync(a => a.IdSucursal == pedido.IdSucursal && a.EsPrincipal && a.IsActive)
            ?? await _context.Almacenes
            .FirstOrDefaultAsync(a => a.IdSucursal == pedido.IdSucursal && a.IsActive);

        if (almacen == null)
        {
            _logger.LogWarning($"La sucursal #{pedido.IdSucursal} no tiene almacén activo configurado. Omitiendo descuento de inventario.");
            return false;
        }

        // Cargar factores de conversión explícitos
        var factoresDb = await _context.FactoresConversion
            .Include(f => f.UnidadOrigen)
            .Include(f => f.UnidadDestino)
            .Where(f => f.IsActive)
            .ToListAsync();

        // 2. Acumular insumos requeridos en su unidad base
        var insumosRequeridos = new Dictionary<int, decimal>(); // IdInsumo -> CantidadBase
        var platillosDescontados = new List<string>();

        var detallesActivos = pedido.Detalles.Where(d => !d.Cancelado).ToList();
        if (!detallesActivos.Any())
        {
            return true;
        }

        foreach (var detalle in detallesActivos)
        {
            platillosDescontados.Add($"{detalle.Cantidad}x {detalle.ProductoNombre}");

            // A) Buscar receta para la variante específica o para el producto padre
            var receta = await _context.Recetas
                .Include(r => r.UnidadMedidaRendimiento)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Insumo)
                        .ThenInclude(i => i.UnidadMedidaBase)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.UnidadMedida)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.SubReceta)
                        .ThenInclude(s => s.UnidadMedidaRendimiento)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.SubReceta)
                        .ThenInclude(s => s.Detalles)
                            .ThenInclude(sd => sd.Insumo)
                                .ThenInclude(si => si.UnidadMedidaBase)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.SubReceta)
                        .ThenInclude(s => s.Detalles)
                            .ThenInclude(sd => sd.UnidadMedida)
                .FirstOrDefaultAsync(r => r.IdVariante == detalle.IdVariante && r.IsActive)
                ?? await _context.Recetas
                .Include(r => r.UnidadMedidaRendimiento)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Insumo)
                        .ThenInclude(i => i.UnidadMedidaBase)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.UnidadMedida)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.SubReceta)
                        .ThenInclude(s => s.UnidadMedidaRendimiento)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.SubReceta)
                        .ThenInclude(s => s.Detalles)
                            .ThenInclude(sd => sd.Insumo)
                                .ThenInclude(si => si.UnidadMedidaBase)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.SubReceta)
                        .ThenInclude(s => s.Detalles)
                            .ThenInclude(sd => sd.UnidadMedida)
                .FirstOrDefaultAsync(r => r.IdProducto == detalle.IdProducto && !r.EsSubReceta && r.IsActive);

            if (receta != null)
            {
                // Multiplicador por cantidad de platillos y rendimiento
                decimal factorPorcion = (receta.Rendimiento > 0)
                    ? (detalle.Cantidad / receta.Rendimiento)
                    : detalle.Cantidad;

                ExplosionarIngredientes(receta.Detalles, factorPorcion, insumosRequeridos, factoresDb);
            }

            // B) Modificadores con escandallo
            if (detalle.Modificadores != null && detalle.Modificadores.Any())
            {
                foreach (var mod in detalle.Modificadores)
                {
                    var recetaMod = await _context.Recetas
                        .Include(r => r.UnidadMedidaRendimiento)
                        .Include(r => r.Detalles)
                            .ThenInclude(d => d.Insumo)
                                .ThenInclude(i => i.UnidadMedidaBase)
                        .Include(r => r.Detalles)
                            .ThenInclude(d => d.UnidadMedida)
                        .FirstOrDefaultAsync(r => r.IdOpcionModificador == mod.IdOpcion && r.IsActive);

                    if (recetaMod != null)
                    {
                        decimal factorMod = (recetaMod.Rendimiento > 0)
                            ? (detalle.Cantidad / recetaMod.Rendimiento)
                            : detalle.Cantidad;

                        ExplosionarIngredientes(recetaMod.Detalles, factorMod, insumosRequeridos, factoresDb);
                    }
                }
            }
        }

        if (!insumosRequeridos.Any())
        {
            // Ningún platillo o modificador tenía receta configurada; no se requiere salida de inventario
            return true;
        }

        // 3. Descontar cada insumo de InventarioExistencia y registrar KardexMovimiento (No Bloqueante)
        var ahora = DateTime.UtcNow;
        var usuarioId = pedido.CerradoPor ?? pedido.AbiertoPor;
        var advertenciasStock = new List<object>();

        foreach (var kvp in insumosRequeridos)
        {
            int idInsumo = kvp.Key;
            decimal cantidadConsumo = kvp.Value;

            if (cantidadConsumo <= 0) continue;

            var insumo = await _context.Insumos.FindAsync(idInsumo);
            if (insumo == null) continue;

            var existencia = await _context.InventarioExistencias
                .FirstOrDefaultAsync(e => e.IdAlmacen == almacen.Id && e.IdInsumo == idInsumo);

            if (existencia == null)
            {
                existencia = new InventarioExistencia
                {
                    IdAlmacen = almacen.Id,
                    IdInsumo = idInsumo,
                    StockActual = 0,
                    FechaUltimoMovimiento = ahora,
                    IsActive = true
                };
                _context.InventarioExistencias.Add(existencia);
            }

            decimal saldoAnterior = existencia.StockActual;
            decimal nuevoSaldo = saldoAnterior - cantidadConsumo; // Tolera stock negativo
            decimal costoUnitario = insumo.CostoPromedio;
            decimal costoTotal = Math.Round(cantidadConsumo * costoUnitario, 4);

            existencia.StockActual = nuevoSaldo;
            existencia.FechaUltimoMovimiento = ahora;

            var kardex = new KardexMovimiento
            {
                IdAlmacen = almacen.Id,
                IdInsumo = idInsumo,
                TipoMovimiento = "SalidaVenta",
                Submotivo = "VentaReceta",
                Cantidad = cantidadConsumo,
                CostoUnitario = costoUnitario,
                CostoTotal = costoTotal,
                SaldoAnterior = saldoAnterior,
                SaldoNuevo = nuevoSaldo,
                CostoPromedioResultante = costoUnitario,
                DocumentoReferencia = $"Pedido #{pedido.Id}" + (idCuenta.HasValue ? $" - Cuenta #{idCuenta.Value}" : ""),
                Observaciones = $"Descuento automático de escandallo al liquidar comanda ({almacen.Nombre})",
                IdUsuario = usuarioId,
                FechaHora = ahora,
                IsActive = true
            };

            _context.KardexMovimientos.Add(kardex);

            // Si el stock cae en cero o negativo, generar auditoría de advertencia
            if (nuevoSaldo <= 0)
            {
                advertenciasStock.Add(new
                {
                    IdInsumo = idInsumo,
                    Codigo = insumo.Codigo,
                    Nombre = insumo.Nombre,
                    SaldoAnterior = saldoAnterior,
                    Consumo = cantidadConsumo,
                    SaldoResultante = nuevoSaldo,
                    Almacen = almacen.Nombre
                });
            }
        }

        // Registrar evento de auditoría si hubo consumo sin stock previo
        if (advertenciasStock.Any())
        {
            var eventoAdvertencia = new EventoPedido
            {
                IdPedido = pedido.Id,
                TipoEvento = "AdvertenciaStockNegativo",
                Payload = JsonSerializer.Serialize(new
                {
                    IdPedido = pedido.Id,
                    IdCuenta = idCuenta,
                    AlmacenId = almacen.Id,
                    AlmacenNombre = almacen.Nombre,
                    Mensaje = "Se registraron consumos de insumos con stock insuficiente o resultante negativo en almacén.",
                    Insumos = advertenciasStock,
                    FechaHora = ahora
                }),
                IsActive = true,
                CreatedAt = ahora,
                CreatedBy = "SistemaInventario"
            };
            _context.EventosPedido.Add(eventoAdvertencia);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private void ExplosionarIngredientes(
        IEnumerable<RecetaDetalle> detalles,
        decimal multiplicador,
        Dictionary<int, decimal> acumulador,
        List<FactorConversion> factoresDb)
    {
        foreach (var detalle in detalles.Where(d => d.IsActive))
        {
            decimal cantidadConsumo = detalle.Cantidad * multiplicador;

            // Merma esperada
            if (detalle.PorcentajeMermaEsperada > 0)
            {
                cantidadConsumo *= (1m + (detalle.PorcentajeMermaEsperada / 100m));
            }

            if (detalle.IdInsumo.HasValue && detalle.Insumo != null)
            {
                string umOrigen = detalle.UnidadMedida?.Codigo ?? "PZA";
                string umDestino = detalle.Insumo.UnidadMedidaBase?.Codigo ?? umOrigen;

                // Buscar factor si existe en BD
                decimal? factorBd = factoresDb
                    .FirstOrDefault(f => f.IdUnidadOrigen == detalle.IdUnidadMedida && f.IdUnidadDestino == detalle.Insumo.IdUnidadMedidaBase)?
                    .Factor;

                decimal cantidadBase = ConversionUnidadesHelper.ConvertirCantidad(cantidadConsumo, umOrigen, umDestino, factorBd);

                int idInsumo = detalle.IdInsumo.Value;
                if (!acumulador.ContainsKey(idInsumo))
                    acumulador[idInsumo] = 0m;

                acumulador[idInsumo] += cantidadBase;
            }
            else if (detalle.IdSubReceta.HasValue && detalle.SubReceta != null)
            {
                // Sub-receta anidada: calcular factor respecto al rendimiento de la sub-receta
                decimal rendimientoSub = (detalle.SubReceta.Rendimiento > 0)
                    ? detalle.SubReceta.Rendimiento
                    : 1m;

                decimal subMultiplicador = cantidadConsumo / rendimientoSub;

                if (detalle.SubReceta.Detalles != null && detalle.SubReceta.Detalles.Any())
                {
                    ExplosionarIngredientes(detalle.SubReceta.Detalles, subMultiplicador, acumulador, factoresDb);
                }
            }
        }
    }
}
