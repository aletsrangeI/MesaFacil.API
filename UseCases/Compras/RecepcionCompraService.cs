using Common;
using Domain.Entities;
using DTO.Compras;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.Compras;

public interface IRecepcionCompraService
{
    Task<Response<CompraFacturaDTO>> RegistrarCompraAsync(RegistrarCompraDTO dto, int? idUsuario);
    Task<Response<CompraFacturaDTO>> AplicarCompraAsync(int idCompra, int? idUsuario);
    Task<Response<bool>> CancelarCompraAsync(int idCompra, int? idUsuario, string? motivo);
    Task<Response<CompraFacturaDTO>> ObtenerPorIdAsync(int idCompra);
    Task<Response<List<CompraItemResumenDTO>>> ObtenerListadoAsync(CompraFiltroDTO filtro);
    Task<Response<ResumenKpisComprasDTO>> ObtenerKpisAsync(int? idSucursal);
}

public class RecepcionCompraService : IRecepcionCompraService
{
    private readonly ApplicationDbContext _context;

    public RecepcionCompraService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Response<CompraFacturaDTO>> RegistrarCompraAsync(RegistrarCompraDTO dto, int? idUsuario)
    {
        var response = new Response<CompraFacturaDTO>();

        // 1. Validaciones básicas
        if (dto.Detalles == null || dto.Detalles.Count == 0)
        {
            response.isSuccess = false;
            response.Message = "La factura debe incluir al menos una partida de insumo.";
            return response;
        }

        // 2. Validación de UUID duplicado (Prevención de duplicidad SAT)
        if (!string.IsNullOrWhiteSpace(dto.UUID))
        {
            var uuidNormalizado = dto.UUID.Trim().ToUpperInvariant();
            var facturaDuplicada = await _context.ComprasFactura
                .AnyAsync(c => c.UUID == uuidNormalizado && c.Estado != "Cancelada");

            if (facturaDuplicada)
            {
                response.isSuccess = false;
                response.Message = $"La factura con UUID fiscal '{uuidNormalizado}' ya fue registrada previamente en el sistema.";
                return response;
            }
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 3. Validar Almacén y Sucursal para resolver IdEmpresa e IdSucursal válidos
            var almacen = await _context.Almacenes
                .Include(a => a.Sucursal)
                .FirstOrDefaultAsync(a => a.Id == dto.IdAlmacen);

            if (almacen == null || !almacen.IsActive)
            {
                response.isSuccess = false;
                response.Message = "El almacén de destino no existe o está inactivo.";
                return response;
            }

            int idSucursalEfectivo = dto.IdSucursal > 0 ? dto.IdSucursal : almacen.IdSucursal;
            int idEmpresaEfectivo = dto.IdEmpresa;

            // Si dto.IdEmpresa no fue especificado o se envió 1 por defecto, intentar obtenerlo de la sucursal
            if (almacen.Sucursal != null && almacen.Sucursal.IdEmpresa > 0)
            {
                idEmpresaEfectivo = almacen.Sucursal.IdEmpresa;
            }
            else
            {
                var sucursalDb = await _context.Sucursales.FindAsync(idSucursalEfectivo);
                if (sucursalDb != null && sucursalDb.IdEmpresa > 0)
                {
                    idEmpresaEfectivo = sucursalDb.IdEmpresa;
                }
                else
                {
                    // Fallback a la primera empresa activa existente en la base de datos
                    var primerEmpresa = await _context.Empresas.OrderBy(e => e.Id).FirstOrDefaultAsync(e => e.IsActive);
                    if (primerEmpresa != null)
                    {
                        idEmpresaEfectivo = primerEmpresa.Id;
                    }
                }
            }

            // 4. Resolución o alta al vuelo del proveedor
            int idProveedor = dto.IdProveedor;
            if (idProveedor <= 0 && dto.ProveedorNuevo != null)
            {
                if (!RfcHelper.EsRfcValido(dto.ProveedorNuevo.RFC, out var rfcError))
                {
                    response.isSuccess = false;
                    response.Message = $"Error en RFC de proveedor: {rfcError}";
                    return response;
                }

                var rfcNorm = dto.ProveedorNuevo.RFC.Trim().ToUpperInvariant();
                var provExistente = await _context.Proveedores.FirstOrDefaultAsync(p => p.RFC == rfcNorm);
                if (provExistente != null)
                {
                    idProveedor = provExistente.Id;
                }
                else
                {
                    var nuevoProv = new Proveedor
                    {
                        IdEmpresa = idEmpresaEfectivo,
                        RFC = rfcNorm,
                        RazonSocial = dto.ProveedorNuevo.RazonSocial.Trim(),
                        NombreComercial = dto.ProveedorNuevo.NombreComercial?.Trim(),
                        Email = dto.ProveedorNuevo.Email?.Trim(),
                        Telefono = dto.ProveedorNuevo.Telefono?.Trim(),
                        Contacto = dto.ProveedorNuevo.Contacto?.Trim(),
                        Direccion = dto.ProveedorNuevo.Direccion?.Trim(),
                        RegimenFiscal = dto.ProveedorNuevo.RegimenFiscal?.Trim(),
                        DiasCredito = dto.ProveedorNuevo.DiasCredito >= 0 ? dto.ProveedorNuevo.DiasCredito : 0,
                        Banco = dto.ProveedorNuevo.Banco?.Trim(),
                        CuentaBancaria = dto.ProveedorNuevo.CuentaBancaria?.Trim(),
                        IsActive = true
                    };
                    _context.Proveedores.Add(nuevoProv);
                    await _context.SaveChangesAsync();
                    idProveedor = nuevoProv.Id;
                }
            }

            var proveedor = await _context.Proveedores.FindAsync(idProveedor);
            if (proveedor == null || !proveedor.IsActive)
            {
                response.isSuccess = false;
                response.Message = "El proveedor especificado no existe o está inactivo.";
                return response;
            }

            // 5. Crear Cabecera CompraFactura
            DateTime? fechaVencimiento = dto.FechaVencimiento;
            if (dto.EsCredito && !fechaVencimiento.HasValue)
            {
                int dias = dto.DiasCredito > 0 ? dto.DiasCredito : proveedor.DiasCredito;
                fechaVencimiento = dto.FechaEmision.AddDays(dias);
            }

            var compra = new CompraFactura
            {
                IdEmpresa = idEmpresaEfectivo,
                IdSucursal = idSucursalEfectivo,
                IdAlmacen = dto.IdAlmacen,
                IdProveedor = idProveedor,
                UUID = !string.IsNullOrWhiteSpace(dto.UUID) ? dto.UUID.Trim().ToUpperInvariant() : null,
                Serie = dto.Serie?.Trim(),
                Folio = !string.IsNullOrWhiteSpace(dto.Folio) ? dto.Folio.Trim() : (dto.UUID?.Length >= 8 ? dto.UUID[..8] : "S/F"),
                FechaEmision = dto.FechaEmision,
                FechaRecepcion = DateTime.UtcNow,
                EsCredito = dto.EsCredito,
                DiasCredito = dto.DiasCredito > 0 ? dto.DiasCredito : (dto.EsCredito ? proveedor.DiasCredito : 0),
                FechaVencimiento = fechaVencimiento,
                Subtotal = dto.Subtotal,
                TotalDescuento = dto.TotalDescuento,
                TotalIVA = dto.TotalIVA,
                TotalIEPS = dto.TotalIEPS,
                Total = dto.Total,
                Estado = dto.AplicarDirecto ? "Aplicada" : "Borrador",
                Observaciones = dto.Observaciones?.Trim(),
                IdUsuario = idUsuario,
                IsActive = true
            };

            _context.ComprasFactura.Add(compra);
            await _context.SaveChangesAsync();

            // 6. Crear Partidas
            foreach (var item in dto.Detalles)
            {
                if (item.Cantidad <= 0)
                {
                    response.isSuccess = false;
                    response.Message = "La cantidad de cada partida debe ser mayor a cero.";
                    return response;
                }

                var insumo = await _context.Insumos
                    .Include(i => i.UnidadMedidaBase)
                    .FirstOrDefaultAsync(i => i.Id == item.IdInsumo);

                if (insumo == null || !insumo.IsActive)
                {
                    response.isSuccess = false;
                    response.Message = $"El insumo con ID {item.IdInsumo} no existe o está inactivo.";
                    return response;
                }

                decimal factor = item.FactorConversion > 0 ? item.FactorConversion : 1.0m;
                decimal cantidadInsumo = item.Cantidad * factor;

                var detalle = new CompraFacturaDetalle
                {
                    IdCompraFactura = compra.Id,
                    IdInsumo = item.IdInsumo,
                    ClaveProdServ = item.ClaveProdServ?.Trim(),
                    DescripcionOriginal = item.DescripcionOriginal?.Trim(),
                    UnidadSAT = item.UnidadSAT?.Trim(),
                    Cantidad = item.Cantidad,
                    IdUnidadMedida = item.IdUnidadMedida ?? insumo.IdUnidadMedidaBase,
                    FactorConversion = factor,
                    CantidadInsumo = cantidadInsumo,
                    CostoUnitario = item.CostoUnitario,
                    Importe = item.Importe > 0 ? item.Importe : (item.Cantidad * item.CostoUnitario),
                    Descuento = item.Descuento,
                    TasaIVA = item.TasaIVA,
                    ImporteIVA = item.ImporteIVA,
                    TasaIEPS = item.TasaIEPS,
                    ImporteIEPS = item.ImporteIEPS,
                    ImporteTotal = item.ImporteTotal > 0 ? item.ImporteTotal : (item.Importe - item.Descuento + item.ImporteIVA + item.ImporteIEPS),
                    IsActive = true
                };

                _context.CompraFacturaDetalles.Add(detalle);
            }

            await _context.SaveChangesAsync();

            // 7. Si es compra a crédito, generar automáticamente la CuentaPorPagar (Spec 017)
            if (dto.EsCredito)
            {
                var cxp = new CuentaPorPagar
                {
                    IdEmpresa = idEmpresaEfectivo,
                    IdSucursal = idSucursalEfectivo,
                    IdProveedor = idProveedor,
                    IdCompraFactura = compra.Id,
                    MontoTotal = dto.Total,
                    SaldoInsoluto = dto.Total,
                    FechaEmision = dto.FechaEmision,
                    FechaVencimiento = fechaVencimiento ?? dto.FechaEmision.AddDays(dto.DiasCredito > 0 ? dto.DiasCredito : proveedor.DiasCredito),
                    Estado = "Pendiente",
                    Observaciones = $"Generada automáticamente de Factura {compra.Serie}{compra.Folio}",
                    IsActive = true
                };
                _context.CuentasPorPagar.Add(cxp);
                await _context.SaveChangesAsync();
            }

            // 8. Si se solicitó "AplicarDirecto", impactar inventario, kárdex y recálculo de costos
            if (dto.AplicarDirecto)
            {
                await AplicarImpactoInventarioAsync(compra.Id, idUsuario, dto.GuardarMapeos);
            }

            await transaction.CommitAsync();

            var resultDto = await ConstruirCompraFacturaDtoAsync(compra.Id);
            response.Data = resultDto;
            response.isSuccess = true;
            response.Message = dto.AplicarDirecto
                ? "Compra registrada e ingresada al inventario exitosamente."
                : "Compra guardada en borrador exitosamente.";
            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.isSuccess = false;
            response.Message = $"Error al registrar la compra: {ex.Message}";
            return response;
        }
    }

    public async Task<Response<CompraFacturaDTO>> AplicarCompraAsync(int idCompra, int? idUsuario)
    {
        var response = new Response<CompraFacturaDTO>();

        var compra = await _context.ComprasFactura.FindAsync(idCompra);
        if (compra == null)
        {
            response.isSuccess = false;
            response.Message = "La factura de compra no existe.";
            return response;
        }

        if (compra.Estado == "Aplicada")
        {
            response.isSuccess = false;
            response.Message = "La factura de compra ya se encuentra aplicada en el inventario.";
            return response;
        }

        if (compra.Estado == "Cancelada")
        {
            response.isSuccess = false;
            response.Message = "No se puede aplicar una factura que ha sido cancelada.";
            return response;
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await AplicarImpactoInventarioAsync(idCompra, idUsuario, guardarMapeos: true);
            compra.Estado = "Aplicada";
            compra.FechaRecepcion = DateTime.UtcNow;

            // Spec 017: Asegurar pasivo en Cuentas por Pagar si la compra es a crédito
            if (compra.EsCredito)
            {
                var cxpExistente = await _context.CuentasPorPagar
                    .FirstOrDefaultAsync(c => c.IdCompraFactura == idCompra);

                if (cxpExistente == null)
                {
                    var prov = await _context.Proveedores.FindAsync(compra.IdProveedor);
                    var cxp = new CuentaPorPagar
                    {
                        IdEmpresa = compra.IdEmpresa,
                        IdSucursal = compra.IdSucursal,
                        IdProveedor = compra.IdProveedor,
                        IdCompraFactura = compra.Id,
                        MontoTotal = compra.Total,
                        SaldoInsoluto = compra.Total,
                        FechaEmision = compra.FechaEmision,
                        FechaVencimiento = compra.FechaVencimiento ?? compra.FechaEmision.AddDays(compra.DiasCredito > 0 ? compra.DiasCredito : (prov?.DiasCredito ?? 0)),
                        Estado = "Pendiente",
                        Observaciones = $"Generada al aplicar Factura {compra.Serie}{compra.Folio}",
                        IsActive = true
                    };
                    _context.CuentasPorPagar.Add(cxp);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Data = await ConstruirCompraFacturaDtoAsync(idCompra);
            response.isSuccess = true;
            response.Message = "Factura aplicada correctamente. Kárdex y costos actualizados.";
            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.isSuccess = false;
            response.Message = $"Error al aplicar la compra: {ex.Message}";
            return response;
        }
    }

    private async Task AplicarImpactoInventarioAsync(int idCompra, int? idUsuario, bool guardarMapeos)
    {
        var compra = await _context.ComprasFactura
            .Include(c => c.Proveedor)
            .Include(c => c.Detalles)
            .FirstOrDefaultAsync(c => c.Id == idCompra);

        if (compra == null) return;

        foreach (var detalle in compra.Detalles)
        {
            var insumo = await _context.Insumos.FindAsync(detalle.IdInsumo);
            if (insumo == null) continue;

            // Cantidad en unidad base del insumo
            decimal cantidadInventario = detalle.CantidadInsumo > 0
                ? detalle.CantidadInsumo
                : (detalle.Cantidad * (detalle.FactorConversion > 0 ? detalle.FactorConversion : 1.0m));

            // Costo unitario neto por unidad base
            decimal costoNetoPartida = detalle.Importe - detalle.Descuento;
            decimal costoUnitarioBase = cantidadInventario > 0
                ? Math.Round(costoNetoPartida / cantidadInventario, 4)
                : detalle.CostoUnitario;

            // Obtener o crear existencia en almacén
            var existencia = await _context.InventarioExistencias
                .FirstOrDefaultAsync(e => e.IdAlmacen == compra.IdAlmacen && e.IdInsumo == detalle.IdInsumo);

            if (existencia == null)
            {
                existencia = new InventarioExistencia
                {
                    IdAlmacen = compra.IdAlmacen,
                    IdInsumo = detalle.IdInsumo,
                    StockActual = 0,
                    FechaUltimoMovimiento = DateTime.UtcNow,
                    IsActive = true
                };
                _context.InventarioExistencias.Add(existencia);
            }

            decimal saldoAnterior = existencia.StockActual;
            decimal nuevoSaldo = saldoAnterior + cantidadInventario;

            // FÓRMULA OFICIAL SPEC 016: Recálculo de Costo Promedio Ponderado (CPP)
            // CPP_nuevo = ((StockActual * CostoPromedio_actual) + (CantidadEntrada * CostoCompraUnitario)) / (StockActual + CantidadEntrada)
            decimal nuevoCostoPromedio;
            if (saldoAnterior <= 0)
            {
                nuevoCostoPromedio = costoUnitarioBase;
            }
            else
            {
                decimal valorInventarioActual = saldoAnterior * insumo.CostoPromedio;
                decimal valorEntradaCompra = cantidadInventario * costoUnitarioBase;
                nuevoCostoPromedio = Math.Round((valorInventarioActual + valorEntradaCompra) / nuevoSaldo, 4);
            }

            // Actualizar Insumo
            insumo.CostoPromedio = nuevoCostoPromedio;
            insumo.UltimoCosto = costoUnitarioBase;

            // Actualizar Existencia
            existencia.StockActual = nuevoSaldo;
            existencia.FechaUltimoMovimiento = DateTime.UtcNow;

            // Registrar movimiento en Kárdex
            string docRef = $"Factura {compra.Serie}{compra.Folio}";
            if (!string.IsNullOrWhiteSpace(compra.UUID))
            {
                docRef += $" ({compra.UUID[..Math.Min(8, compra.UUID.Length)]})";
            }

            var kardex = new KardexMovimiento
            {
                IdAlmacen = compra.IdAlmacen,
                IdInsumo = detalle.IdInsumo,
                TipoMovimiento = "EntradaCompra",
                Submotivo = $"Compra Factura {compra.Serie}{compra.Folio}",
                Cantidad = cantidadInventario,
                CostoUnitario = costoUnitarioBase,
                CostoTotal = cantidadInventario * costoUnitarioBase,
                SaldoAnterior = saldoAnterior,
                SaldoNuevo = nuevoSaldo,
                CostoPromedioResultante = nuevoCostoPromedio,
                DocumentoReferencia = docRef,
                Observaciones = $"Recepción de compra. Proveedor: {compra.Proveedor?.RazonSocial ?? "N/A"}",
                IdUsuario = idUsuario,
                FechaHora = DateTime.UtcNow,
                IsActive = true
            };

            _context.KardexMovimientos.Add(kardex);

            // Guardar o actualizar memoria de mapeo inteligente SAT ➔ Insumo
            if (guardarMapeos && !string.IsNullOrWhiteSpace(detalle.ClaveProdServ) && !string.IsNullOrWhiteSpace(detalle.DescripcionOriginal))
            {
                var mapeoExistente = await _context.MapeosInsumoProveedor
                    .FirstOrDefaultAsync(m => m.IdProveedor == compra.IdProveedor &&
                                             m.ClaveProdServ == detalle.ClaveProdServ &&
                                             m.DescripcionSAT == detalle.DescripcionOriginal);

                if (mapeoExistente != null)
                {
                    mapeoExistente.IdInsumo = detalle.IdInsumo;
                    mapeoExistente.FactorConversion = detalle.FactorConversion > 0 ? detalle.FactorConversion : 1.0m;
                    mapeoExistente.UnidadSAT = detalle.UnidadSAT;
                    mapeoExistente.FechaUltimaCompra = DateTime.UtcNow;
                }
                else
                {
                    var nuevoMapeo = new MapeoInsumoProveedor
                    {
                        IdProveedor = compra.IdProveedor,
                        ClaveProdServ = detalle.ClaveProdServ,
                        DescripcionSAT = detalle.DescripcionOriginal,
                        UnidadSAT = detalle.UnidadSAT,
                        IdInsumo = detalle.IdInsumo,
                        FactorConversion = detalle.FactorConversion > 0 ? detalle.FactorConversion : 1.0m,
                        FechaRegistro = DateTime.UtcNow,
                        FechaUltimaCompra = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.MapeosInsumoProveedor.Add(nuevoMapeo);
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Response<bool>> CancelarCompraAsync(int idCompra, int? idUsuario, string? motivo)
    {
        var response = new Response<bool>();

        var compra = await _context.ComprasFactura
            .Include(c => c.Detalles)
            .FirstOrDefaultAsync(c => c.Id == idCompra);

        if (compra == null)
        {
            response.isSuccess = false;
            response.Message = "La factura de compra especificada no existe.";
            return response;
        }

        if (compra.Estado == "Cancelada")
        {
            response.isSuccess = false;
            response.Message = "La factura ya se encuentra cancelada.";
            return response;
        }

        // Spec 017: Validar si la factura tiene una CuentaPorPagar asociada
        var cxpAsociada = await _context.CuentasPorPagar
            .Include(c => c.Pagos)
            .FirstOrDefaultAsync(c => c.IdCompraFactura == idCompra);

        if (cxpAsociada != null && cxpAsociada.Estado != "Cancelada")
        {
            if (cxpAsociada.Pagos.Any(p => p.IsActive))
            {
                response.isSuccess = false;
                response.Message = "No se puede cancelar la factura de compra porque ya tiene abonos o pagos registrados en Cuentas por Pagar. Debe revertir los pagos primero.";
                return response;
            }
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Si la compra estaba aplicada, reversar el stock ingresado
            if (compra.Estado == "Aplicada")
            {
                foreach (var detalle in compra.Detalles)
                {
                    var insumo = await _context.Insumos.FindAsync(detalle.IdInsumo);
                    var existencia = await _context.InventarioExistencias
                        .FirstOrDefaultAsync(e => e.IdAlmacen == compra.IdAlmacen && e.IdInsumo == detalle.IdInsumo);

                    if (existencia != null)
                    {
                        decimal saldoAnterior = existencia.StockActual;
                        decimal nuevoSaldo = saldoAnterior - detalle.CantidadInsumo;

                        existencia.StockActual = nuevoSaldo;
                        existencia.FechaUltimoMovimiento = DateTime.UtcNow;

                        var kardex = new KardexMovimiento
                        {
                            IdAlmacen = compra.IdAlmacen,
                            IdInsumo = detalle.IdInsumo,
                            TipoMovimiento = "CancelacionCompra",
                            Submotivo = $"Cancelación Compra Factura {compra.Serie}{compra.Folio}",
                            Cantidad = detalle.CantidadInsumo,
                            CostoUnitario = insumo?.CostoPromedio ?? detalle.CostoUnitario,
                            CostoTotal = detalle.CantidadInsumo * (insumo?.CostoPromedio ?? detalle.CostoUnitario),
                            SaldoAnterior = saldoAnterior,
                            SaldoNuevo = nuevoSaldo,
                            CostoPromedioResultante = insumo?.CostoPromedio ?? detalle.CostoUnitario,
                            DocumentoReferencia = $"Cancelación {compra.Serie}{compra.Folio}",
                            Observaciones = $"Cancelación de factura. Motivo: {motivo ?? "Sin motivo especificado"}",
                            IdUsuario = idUsuario,
                            FechaHora = DateTime.UtcNow,
                            IsActive = true
                        };
                        _context.KardexMovimientos.Add(kardex);
                    }
                }
            }

            // Cancelar pasivo en Cuentas por Pagar si existía
            if (cxpAsociada != null && cxpAsociada.Estado != "Cancelada")
            {
                cxpAsociada.Estado = "Cancelada";
                cxpAsociada.Observaciones = string.IsNullOrWhiteSpace(cxpAsociada.Observaciones)
                    ? $"[Cancelada por anulación de factura: {motivo}]"
                    : $"{cxpAsociada.Observaciones} [Cancelada por anulación de factura: {motivo}]";
            }

            compra.Estado = "Cancelada";
            compra.Observaciones = string.IsNullOrWhiteSpace(compra.Observaciones)
                ? $"[Cancelada: {motivo}]"
                : $"{compra.Observaciones} [Cancelada: {motivo}]";

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Data = true;
            response.isSuccess = true;
            response.Message = "Factura cancelada e inventario ajustado correctamente.";
            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.isSuccess = false;
            response.Message = $"Error al cancelar la factura: {ex.Message}";
            return response;
        }
    }

    public async Task<Response<CompraFacturaDTO>> ObtenerPorIdAsync(int idCompra)
    {
        var response = new Response<CompraFacturaDTO>();
        var dto = await ConstruirCompraFacturaDtoAsync(idCompra);
        if (dto == null)
        {
            response.isSuccess = false;
            response.Message = "Factura de compra no encontrada.";
            return response;
        }

        response.Data = dto;
        response.isSuccess = true;
        return response;
    }

    public async Task<Response<List<CompraItemResumenDTO>>> ObtenerListadoAsync(CompraFiltroDTO filtro)
    {
        var response = new Response<List<CompraItemResumenDTO>>();

        var query = _context.ComprasFactura
            .Include(c => c.Sucursal)
            .Include(c => c.Almacen)
            .Include(c => c.Proveedor)
            .Include(c => c.Detalles)
            .AsNoTracking()
            .AsQueryable();

        if (filtro.IdSucursal.HasValue && filtro.IdSucursal.Value > 0)
            query = query.Where(c => c.IdSucursal == filtro.IdSucursal.Value);

        if (filtro.IdAlmacen.HasValue && filtro.IdAlmacen.Value > 0)
            query = query.Where(c => c.IdAlmacen == filtro.IdAlmacen.Value);

        if (filtro.IdProveedor.HasValue && filtro.IdProveedor.Value > 0)
            query = query.Where(c => c.IdProveedor == filtro.IdProveedor.Value);

        if (!string.IsNullOrWhiteSpace(filtro.Estado) && filtro.Estado != "Todos")
            query = query.Where(c => c.Estado == filtro.Estado);

        if (filtro.FechaInicio.HasValue)
            query = query.Where(c => c.FechaEmision >= filtro.FechaInicio.Value.Date);

        if (filtro.FechaFin.HasValue)
            query = query.Where(c => c.FechaEmision <= filtro.FechaFin.Value.Date.AddDays(1).AddTicks(-1));

        if (!string.IsNullOrWhiteSpace(filtro.Buscar))
        {
            var term = filtro.Buscar.Trim().ToLowerInvariant();
            query = query.Where(c =>
                c.Folio.ToLower().Contains(term) ||
                (c.Serie != null && c.Serie.ToLower().Contains(term)) ||
                (c.UUID != null && c.UUID.ToLower().Contains(term)) ||
                c.Proveedor!.RazonSocial.ToLower().Contains(term) ||
                c.Proveedor.RFC.ToLower().Contains(term));
        }

        var list = await query
            .OrderByDescending(c => c.FechaEmision)
            .ThenByDescending(c => c.Id)
            .Select(c => new CompraItemResumenDTO
            {
                Id = c.Id,
                UUID = c.UUID,
                Serie = c.Serie,
                Folio = c.Folio,
                FechaEmision = c.FechaEmision,
                FechaRecepcion = c.FechaRecepcion,
                SucursalNombre = c.Sucursal != null ? c.Sucursal.Nombre : "Sin sucursal",
                AlmacenNombre = c.Almacen != null ? c.Almacen.Nombre : "Sin almacén",
                ProveedorRFC = c.Proveedor != null ? c.Proveedor.RFC : string.Empty,
                ProveedorRazonSocial = c.Proveedor != null ? c.Proveedor.RazonSocial : string.Empty,
                EsCredito = c.EsCredito,
                DiasCredito = c.DiasCredito,
                FechaVencimiento = c.FechaVencimiento,
                Total = c.Total,
                Estado = c.Estado,
                CantidadPartidas = c.Detalles.Count
            })
            .ToListAsync();

        response.Data = list;
        response.isSuccess = true;
        return response;
    }

    public async Task<Response<ResumenKpisComprasDTO>> ObtenerKpisAsync(int? idSucursal)
    {
        var response = new Response<ResumenKpisComprasDTO>();

        var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var query = _context.ComprasFactura.AsNoTracking().AsQueryable();

        if (idSucursal.HasValue && idSucursal.Value > 0)
            query = query.Where(c => c.IdSucursal == idSucursal.Value);

        var comprasMes = await query
            .Where(c => c.FechaEmision >= inicioMes && c.Estado != "Cancelada")
            .ToListAsync();

        var kpis = new ResumenKpisComprasDTO
        {
            TotalComprasMes = comprasMes.Sum(c => c.Total),
            TotalFacturasMes = comprasMes.Count,
            FacturasAplicadas = comprasMes.Count(c => c.Estado == "Aplicada"),
            FacturasBorrador = comprasMes.Count(c => c.Estado == "Borrador"),
            TotalCreditoPendiente = comprasMes.Where(c => c.EsCredito).Sum(c => c.Total),
            FacturasPendientesPago = comprasMes.Count(c => c.EsCredito)
        };

        response.Data = kpis;
        response.isSuccess = true;
        return response;
    }

    private async Task<CompraFacturaDTO?> ConstruirCompraFacturaDtoAsync(int idCompra)
    {
        return await _context.ComprasFactura
            .Include(c => c.Sucursal)
            .Include(c => c.Almacen)
            .Include(c => c.Proveedor)
            .Include(c => c.Usuario)
            .Include(c => c.Detalles)
                .ThenInclude(d => d.Insumo)
            .Include(c => c.Detalles)
                .ThenInclude(d => d.UnidadMedida)
            .AsNoTracking()
            .Where(c => c.Id == idCompra)
            .Select(c => new CompraFacturaDTO
            {
                Id = c.Id,
                IdEmpresa = c.IdEmpresa,
                IdSucursal = c.IdSucursal,
                SucursalNombre = c.Sucursal != null ? c.Sucursal.Nombre : "Sin sucursal",
                IdAlmacen = c.IdAlmacen,
                AlmacenNombre = c.Almacen != null ? c.Almacen.Nombre : "Sin almacén",
                IdProveedor = c.IdProveedor,
                ProveedorRFC = c.Proveedor != null ? c.Proveedor.RFC : string.Empty,
                ProveedorRazonSocial = c.Proveedor != null ? c.Proveedor.RazonSocial : string.Empty,
                UUID = c.UUID,
                Serie = c.Serie,
                Folio = c.Folio,
                FechaEmision = c.FechaEmision,
                FechaRecepcion = c.FechaRecepcion,
                EsCredito = c.EsCredito,
                DiasCredito = c.DiasCredito,
                FechaVencimiento = c.FechaVencimiento,
                Subtotal = c.Subtotal,
                TotalDescuento = c.TotalDescuento,
                TotalIVA = c.TotalIVA,
                TotalIEPS = c.TotalIEPS,
                Total = c.Total,
                Estado = c.Estado,
                RutaArchivoXML = c.RutaArchivoXML,
                RutaArchivoPDF = c.RutaArchivoPDF,
                Observaciones = c.Observaciones,
                IdUsuario = c.IdUsuario,
                UsuarioNombre = c.Usuario != null ? c.Usuario.NombreCompleto : null,
                Detalles = c.Detalles.Select(d => new CompraFacturaDetalleDTO
                {
                    Id = d.Id,
                    IdCompraFactura = d.IdCompraFactura,
                    IdInsumo = d.IdInsumo,
                    InsumoCodigo = d.Insumo != null ? d.Insumo.Codigo : null,
                    InsumoNombre = d.Insumo != null ? d.Insumo.Nombre : string.Empty,
                    ClaveProdServ = d.ClaveProdServ,
                    DescripcionOriginal = d.DescripcionOriginal,
                    UnidadSAT = d.UnidadSAT,
                    Cantidad = d.Cantidad,
                    IdUnidadMedida = d.IdUnidadMedida,
                    UnidadMedidaNombre = d.UnidadMedida != null ? d.UnidadMedida.Nombre : "PZA",
                    FactorConversion = d.FactorConversion,
                    CantidadInsumo = d.CantidadInsumo,
                    CostoUnitario = d.CostoUnitario,
                    Importe = d.Importe,
                    Descuento = d.Descuento,
                    TasaIVA = d.TasaIVA,
                    ImporteIVA = d.ImporteIVA,
                    TasaIEPS = d.TasaIEPS,
                    ImporteIEPS = d.ImporteIEPS,
                    ImporteTotal = d.ImporteTotal
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}
