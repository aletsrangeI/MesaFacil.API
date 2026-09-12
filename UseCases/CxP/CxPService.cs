using Common;
using Domain.Entities;
using DTO.CxP;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.CxP;

public class CxPService : ICxPService
{
    private readonly ApplicationDbContext _context;

    public CxPService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Response<List<CuentaPorPagarItemDTO>>> ObtenerListadoAsync(FiltroCxPDTO filtro)
    {
        var response = new Response<List<CuentaPorPagarItemDTO>>();

        var query = _context.CuentasPorPagar
            .Include(c => c.Sucursal)
            .Include(c => c.Proveedor)
            .Include(c => c.CompraFactura)
            .Include(c => c.Pagos)
            .AsNoTracking()
            .AsQueryable();

        if (filtro.IdSucursal.HasValue && filtro.IdSucursal.Value > 0)
            query = query.Where(c => c.IdSucursal == filtro.IdSucursal.Value);

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
                (c.CompraFactura != null && c.CompraFactura.Folio.ToLower().Contains(term)) ||
                (c.CompraFactura != null && c.CompraFactura.Serie != null && c.CompraFactura.Serie.ToLower().Contains(term)) ||
                (c.CompraFactura != null && c.CompraFactura.UUID != null && c.CompraFactura.UUID.ToLower().Contains(term)) ||
                c.Proveedor!.RazonSocial.ToLower().Contains(term) ||
                c.Proveedor.RFC.ToLower().Contains(term) ||
                (c.Observaciones != null && c.Observaciones.ToLower().Contains(term)));
        }

        var listDb = await query
            .OrderByDescending(c => c.FechaVencimiento)
            .ThenByDescending(c => c.Id)
            .ToListAsync();

        var hoy = DateTime.UtcNow.Date;

        var items = listDb.Select(c =>
        {
            var diasParaVencer = (int)(c.FechaVencimiento.Date - hoy).TotalDays;
            string semaforo;

            if (c.Estado == "Pagada" || c.Estado == "Cancelada" || c.SaldoInsoluto <= 0)
            {
                semaforo = "AlCorriente";
            }
            else if (diasParaVencer < 0)
            {
                semaforo = "Vencida";
            }
            else if (diasParaVencer <= 7)
            {
                semaforo = "PorVencer";
            }
            else
            {
                semaforo = "AlCorriente";
            }

            return new CuentaPorPagarItemDTO
            {
                Id = c.Id,
                IdEmpresa = c.IdEmpresa,
                IdSucursal = c.IdSucursal,
                SucursalNombre = c.Sucursal?.Nombre ?? "Sin sucursal",
                IdProveedor = c.IdProveedor,
                ProveedorRFC = c.Proveedor?.RFC ?? string.Empty,
                ProveedorRazonSocial = c.Proveedor?.RazonSocial ?? string.Empty,
                ProveedorNombreComercial = c.Proveedor?.NombreComercial,
                IdCompraFactura = c.IdCompraFactura,
                FacturaUUID = c.CompraFactura?.UUID,
                FacturaSerie = c.CompraFactura?.Serie,
                FacturaFolio = c.CompraFactura?.Folio,
                MontoTotal = c.MontoTotal,
                SaldoInsoluto = c.SaldoInsoluto,
                FechaEmision = c.FechaEmision,
                FechaVencimiento = c.FechaVencimiento,
                DiasCredito = c.CompraFactura?.DiasCredito ?? (c.Proveedor?.DiasCredito ?? 0),
                Estado = c.Estado,
                Semaforo = semaforo,
                DiasParaVencer = diasParaVencer,
                Observaciones = c.Observaciones,
                CantidadAbonos = c.Pagos.Count(p => p.IsActive)
            };
        }).ToList();

        if (!string.IsNullOrWhiteSpace(filtro.Semaforo) && filtro.Semaforo != "Todos")
        {
            items = items.Where(i => i.Semaforo == filtro.Semaforo).ToList();
        }

        response.Data = items;
        response.isSuccess = true;
        return response;
    }

    public async Task<Response<CuentaPorPagarDTO>> ObtenerPorIdAsync(int id)
    {
        var response = new Response<CuentaPorPagarDTO>();

        var c = await _context.CuentasPorPagar
            .Include(x => x.Sucursal)
            .Include(x => x.Proveedor)
            .Include(x => x.CompraFactura)
            .Include(x => x.Pagos)
                .ThenInclude(p => p.MetodoPago)
            .Include(x => x.Pagos)
                .ThenInclude(p => p.Usuario)
            .Include(x => x.Pagos)
                .ThenInclude(p => p.MovimientoCaja)
                    .ThenInclude(m => m!.Turno)
                        .ThenInclude(t => t.Usuario)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c == null)
        {
            response.isSuccess = false;
            response.Message = "La cuenta por pagar no fue encontrada.";
            return response;
        }

        var hoy = DateTime.UtcNow.Date;
        var diasParaVencer = (int)(c.FechaVencimiento.Date - hoy).TotalDays;
        string semaforo;

        if (c.Estado == "Pagada" || c.Estado == "Cancelada" || c.SaldoInsoluto <= 0)
        {
            semaforo = "AlCorriente";
        }
        else if (diasParaVencer < 0)
        {
            semaforo = "Vencida";
        }
        else if (diasParaVencer <= 7)
        {
            semaforo = "PorVencer";
        }
        else
        {
            semaforo = "AlCorriente";
        }

        var dto = new CuentaPorPagarDTO
        {
            Id = c.Id,
            IdEmpresa = c.IdEmpresa,
            IdSucursal = c.IdSucursal,
            SucursalNombre = c.Sucursal?.Nombre ?? "Sin sucursal",
            IdProveedor = c.IdProveedor,
            ProveedorRFC = c.Proveedor?.RFC ?? string.Empty,
            ProveedorRazonSocial = c.Proveedor?.RazonSocial ?? string.Empty,
            ProveedorNombreComercial = c.Proveedor?.NombreComercial,
            IdCompraFactura = c.IdCompraFactura,
            FacturaUUID = c.CompraFactura?.UUID,
            FacturaSerie = c.CompraFactura?.Serie,
            FacturaFolio = c.CompraFactura?.Folio,
            MontoTotal = c.MontoTotal,
            SaldoInsoluto = c.SaldoInsoluto,
            FechaEmision = c.FechaEmision,
            FechaVencimiento = c.FechaVencimiento,
            DiasCredito = c.CompraFactura?.DiasCredito ?? (c.Proveedor?.DiasCredito ?? 0),
            Estado = c.Estado,
            Semaforo = semaforo,
            DiasParaVencer = diasParaVencer,
            Observaciones = c.Observaciones,
            CantidadAbonos = c.Pagos.Count(p => p.IsActive),
            Pagos = c.Pagos
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.FechaPago)
                .ThenByDescending(p => p.Id)
                .Select(p => new PagoCuentaPorPagarDTO
                {
                    Id = p.Id,
                    IdCuentaPorPagar = p.IdCuentaPorPagar,
                    Monto = p.Monto,
                    FechaPago = p.FechaPago,
                    IdMetodoPago = p.IdMetodoPago,
                    MetodoPagoNombre = p.MetodoPago?.Descripcion ?? "Desconocido",
                    IdMovimientoCaja = p.IdMovimientoCaja,
                    TurnoUsuario = p.MovimientoCaja?.Turno?.Usuario?.NombreCompleto,
                    ReferenciaBancaria = p.ReferenciaBancaria,
                    ComprobanteUrl = p.ComprobanteUrl,
                    IdUsuario = p.IdUsuario,
                    UsuarioNombre = p.Usuario?.NombreCompleto,
                    Observaciones = p.Observaciones
                }).ToList()
        };

        response.Data = dto;
        response.isSuccess = true;
        return response;
    }

    public async Task<Response<ResumenKpisCxPDTO>> ObtenerKpisAsync(int? idSucursal)
    {
        var response = new Response<ResumenKpisCxPDTO>();

        var query = _context.CuentasPorPagar.AsNoTracking().AsQueryable();

        if (idSucursal.HasValue && idSucursal.Value > 0)
            query = query.Where(c => c.IdSucursal == idSucursal.Value);

        var cuentasActivas = await query
            .Where(c => c.Estado != "Cancelada")
            .ToListAsync();

        var hoy = DateTime.UtcNow.Date;
        var inicioSemana = hoy;
        var finSemana = hoy.AddDays(7);
        var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);

        var cuentasConSaldo = cuentasActivas.Where(c => c.SaldoInsoluto > 0).ToList();

        // Pagos del mes
        var queryPagos = _context.PagosCuentaPorPagar
            .Include(p => p.CuentaPorPagar)
            .Where(p => p.IsActive && p.FechaPago >= primerDiaMes);

        if (idSucursal.HasValue && idSucursal.Value > 0)
        {
            queryPagos = queryPagos.Where(p => p.CuentaPorPagar!.IdSucursal == idSucursal.Value);
        }

        var totalPagadoMes = await queryPagos.SumAsync(p => p.Monto);

        var kpis = new ResumenKpisCxPDTO
        {
            TotalPorPagar = cuentasConSaldo.Sum(c => c.SaldoInsoluto),
            TotalVencido = cuentasConSaldo.Where(c => c.FechaVencimiento.Date < hoy).Sum(c => c.SaldoInsoluto),
            TotalVenceEstaSemana = cuentasConSaldo.Where(c => c.FechaVencimiento.Date >= inicioSemana && c.FechaVencimiento.Date <= finSemana).Sum(c => c.SaldoInsoluto),
            TotalPagadoMes = totalPagadoMes,
            CantidadPendientes = cuentasConSaldo.Count,
            CantidadVencidas = cuentasConSaldo.Count(c => c.FechaVencimiento.Date < hoy)
        };

        response.Data = kpis;
        response.isSuccess = true;
        return response;
    }

    public async Task<Response<PagoCuentaPorPagarDTO>> RegistrarAbonoAsync(RegistrarPagoCxPDTO dto, int? idUsuario)
    {
        var response = new Response<PagoCuentaPorPagarDTO>();

        if (dto.Monto <= 0)
        {
            response.isSuccess = false;
            response.Message = "El monto a abonar debe ser mayor a $0.00 MXN.";
            return response;
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cuenta = await _context.CuentasPorPagar
                .Include(c => c.Proveedor)
                .Include(c => c.CompraFactura)
                .FirstOrDefaultAsync(c => c.Id == dto.IdCuentaPorPagar);

            if (cuenta == null)
            {
                response.isSuccess = false;
                response.Message = "La cuenta por pagar no existe.";
                return response;
            }

            if (cuenta.Estado == "Cancelada")
            {
                response.isSuccess = false;
                response.Message = "No se pueden registrar pagos a una cuenta por pagar cancelada.";
                return response;
            }

            if (cuenta.SaldoInsoluto <= 0 || cuenta.Estado == "Pagada")
            {
                response.isSuccess = false;
                response.Message = "La cuenta por pagar ya se encuentra completamente liquidada.";
                return response;
            }

            // Validar exceso de pago
            if (dto.Monto > cuenta.SaldoInsoluto)
            {
                response.isSuccess = false;
                response.Message = $"El monto a abonar (${dto.Monto:N2} MXN) excede el saldo insoluto pendiente (${cuenta.SaldoInsoluto:N2} MXN).";
                return response;
            }

            int? idMovimientoCaja = null;

            // Integración con Caja Chica si se solicitó pagar en efectivo desde el turno activo
            if (dto.PagarDesdeCajaChica)
            {
                Turno? turnoActivo = null;

                if (dto.IdTurno.HasValue && dto.IdTurno.Value > 0)
                {
                    turnoActivo = await _context.Turnos
                        .FirstOrDefaultAsync(t => t.Id == dto.IdTurno.Value && t.Cierre == null && t.IsActive);
                }
                else
                {
                    turnoActivo = await _context.Turnos
                        .FirstOrDefaultAsync(t => t.IdSucursal == cuenta.IdSucursal && t.Cierre == null && t.IsActive);
                }

                if (turnoActivo == null)
                {
                    response.isSuccess = false;
                    response.Message = "No se encontró un turno de caja abierto en la sucursal para debitar el dinero de caja chica. Abra un turno de caja o desactive la opción de caja chica.";
                    return response;
                }

                var provName = cuenta.Proveedor?.RazonSocial ?? "Proveedor";
                var folio = cuenta.CompraFactura != null ? $"{cuenta.CompraFactura.Serie}{cuenta.CompraFactura.Folio}" : $"CxP #{cuenta.Id}";

                var movCaja = new MovimientoCaja
                {
                    IdTurno = turnoActivo.Id,
                    Tipo = "Egreso",
                    Monto = dto.Monto,
                    Nota = $"Pago Proveedor: {provName} - Factura: {folio}",
                    IsActive = true
                };

                _context.MovimientosCaja.Add(movCaja);
                await _context.SaveChangesAsync();
                idMovimientoCaja = movCaja.Id;
            }

            // Actualizar Saldo Insoluto y Estado de la Cuenta
            cuenta.SaldoInsoluto -= dto.Monto;
            if (cuenta.SaldoInsoluto <= 0)
            {
                cuenta.SaldoInsoluto = 0;
                cuenta.Estado = "Pagada";
            }
            else
            {
                cuenta.Estado = "Abonada";
            }

            var pago = new PagoCuentaPorPagar
            {
                IdCuentaPorPagar = cuenta.Id,
                Monto = dto.Monto,
                FechaPago = dto.FechaPago ?? DateTime.UtcNow,
                IdMetodoPago = dto.IdMetodoPago > 0 ? dto.IdMetodoPago : 1, // Default 1 Efectivo
                IdMovimientoCaja = idMovimientoCaja,
                ReferenciaBancaria = dto.ReferenciaBancaria?.Trim(),
                IdUsuario = idUsuario,
                Observaciones = dto.Observaciones?.Trim(),
                IsActive = true
            };

            _context.PagosCuentaPorPagar.Add(pago);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var metodoDb = await _context.CatMetodosDePago.FindAsync(pago.IdMetodoPago);
            var usuarioDb = idUsuario.HasValue ? await _context.Usuarios.FindAsync(idUsuario.Value) : null;

            response.Data = new PagoCuentaPorPagarDTO
            {
                Id = pago.Id,
                IdCuentaPorPagar = pago.IdCuentaPorPagar,
                Monto = pago.Monto,
                FechaPago = pago.FechaPago,
                IdMetodoPago = pago.IdMetodoPago,
                MetodoPagoNombre = metodoDb?.Descripcion ?? "Efectivo",
                IdMovimientoCaja = pago.IdMovimientoCaja,
                ReferenciaBancaria = pago.ReferenciaBancaria,
                ComprobanteUrl = pago.ComprobanteUrl,
                IdUsuario = pago.IdUsuario,
                UsuarioNombre = usuarioDb?.NombreCompleto,
                Observaciones = pago.Observaciones
            };

            response.isSuccess = true;
            response.Message = cuenta.Estado == "Pagada"
                ? $"Pago de ${dto.Monto:N2} MXN registrado exitosamente. La cuenta ha quedado completamente liquidada."
                : $"Abono de ${dto.Monto:N2} MXN registrado exitosamente. Saldo restante: ${cuenta.SaldoInsoluto:N2} MXN.";

            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.isSuccess = false;
            response.Message = $"Error al registrar el abono: {ex.Message}";
            return response;
        }
    }

    public async Task<Response<bool>> CancelarCuentaPorPagarAsync(int id, int? idUsuario, string? motivo)
    {
        var response = new Response<bool>();

        var cuenta = await _context.CuentasPorPagar
            .Include(c => c.Pagos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cuenta == null)
        {
            response.isSuccess = false;
            response.Message = "La cuenta por pagar no existe.";
            return response;
        }

        if (cuenta.Estado == "Cancelada")
        {
            response.isSuccess = false;
            response.Message = "La cuenta por pagar ya se encuentra cancelada.";
            return response;
        }

        if (cuenta.Pagos.Any(p => p.IsActive))
        {
            response.isSuccess = false;
            response.Message = "No se puede cancelar una cuenta por pagar que ya tiene abonos o pagos registrados.";
            return response;
        }

        cuenta.Estado = "Cancelada";
        cuenta.Observaciones = string.IsNullOrWhiteSpace(cuenta.Observaciones)
            ? $"[Cancelada: {motivo}]"
            : $"{cuenta.Observaciones} [Cancelada: {motivo}]";

        await _context.SaveChangesAsync();

        response.Data = true;
        response.isSuccess = true;
        response.Message = "Cuenta por pagar cancelada correctamente.";
        return response;
    }

    public async Task<Response<ReporteAntiguedadSaldosDTO>> ObtenerReporteAntiguedadSaldosAsync(int? idSucursal)
    {
        var response = new Response<ReporteAntiguedadSaldosDTO>();

        var query = _context.CuentasPorPagar
            .Include(c => c.Proveedor)
            .Where(c => c.Estado != "Cancelada" && c.SaldoInsoluto > 0)
            .AsNoTracking()
            .AsQueryable();

        if (idSucursal.HasValue && idSucursal.Value > 0)
            query = query.Where(c => c.IdSucursal == idSucursal.Value);

        var cuentas = await query.ToListAsync();
        var hoy = DateTime.UtcNow.Date;

        var reporte = new ReporteAntiguedadSaldosDTO();

        // Agrupar por proveedor
        var agrupado = cuentas.GroupBy(c => new { c.IdProveedor, c.Proveedor!.RFC, c.Proveedor.RazonSocial });

        foreach (var grupo in agrupado)
        {
            var itemProv = new AntiguedadProveedorDTO
            {
                IdProveedor = grupo.Key.IdProveedor,
                RFC = grupo.Key.RFC,
                RazonSocial = grupo.Key.RazonSocial
            };

            foreach (var c in grupo)
            {
                int diasVencido = (int)(hoy - c.FechaVencimiento.Date).TotalDays;

                if (diasVencido <= 0)
                {
                    itemProv.AlCorriente += c.SaldoInsoluto;
                }
                else if (diasVencido <= 15)
                {
                    itemProv.De1A15 += c.SaldoInsoluto;
                }
                else if (diasVencido <= 30)
                {
                    itemProv.De16A30 += c.SaldoInsoluto;
                }
                else if (diasVencido <= 60)
                {
                    itemProv.De31A60 += c.SaldoInsoluto;
                }
                else
                {
                    itemProv.MasDe60 += c.SaldoInsoluto;
                }
            }

            reporte.Proveedores.Add(itemProv);

            reporte.Totales.AlCorriente += itemProv.AlCorriente;
            reporte.Totales.De1A15 += itemProv.De1A15;
            reporte.Totales.De16A30 += itemProv.De16A30;
            reporte.Totales.De31A60 += itemProv.De31A60;
            reporte.Totales.MasDe60 += itemProv.MasDe60;
        }

        reporte.Proveedores = reporte.Proveedores
            .OrderByDescending(p => p.Total)
            .ToList();

        response.Data = reporte;
        response.isSuccess = true;
        return response;
    }

    public async Task<Response<EstadoCuentaProveedorDTO>> ObtenerEstadoCuentaProveedorAsync(int idProveedor, int? idSucursal)
    {
        var response = new Response<EstadoCuentaProveedorDTO>();

        var proveedor = await _context.Proveedores.FindAsync(idProveedor);
        if (proveedor == null)
        {
            response.isSuccess = false;
            response.Message = "El proveedor especificado no existe.";
            return response;
        }

        var queryCuentas = _context.CuentasPorPagar
            .Include(c => c.CompraFactura)
            .Include(c => c.Pagos)
                .ThenInclude(p => p.MetodoPago)
            .Where(c => c.IdProveedor == idProveedor && c.Estado != "Cancelada")
            .AsNoTracking()
            .AsQueryable();

        if (idSucursal.HasValue && idSucursal.Value > 0)
            queryCuentas = queryCuentas.Where(c => c.IdSucursal == idSucursal.Value);

        var cuentas = await queryCuentas.ToListAsync();

        var estado = new EstadoCuentaProveedorDTO
        {
            IdProveedor = proveedor.Id,
            RFC = proveedor.RFC,
            RazonSocial = proveedor.RazonSocial,
            DiasCredito = proveedor.DiasCredito,
            SaldoTotalPendiente = cuentas.Sum(c => c.SaldoInsoluto),
            TotalCompradoCredito = cuentas.Sum(c => c.MontoTotal),
            TotalAbonado = cuentas.Sum(c => c.MontoTotal - c.SaldoInsoluto)
        };

        // Construir listado de movimientos cronológicos
        var movimientosSinSaldo = new List<MovimientoEstadoCuentaDTO>();

        foreach (var c in cuentas)
        {
            var folio = c.CompraFactura != null ? $"{c.CompraFactura.Serie}{c.CompraFactura.Folio}" : $"Factura #{c.Id}";
            movimientosSinSaldo.Add(new MovimientoEstadoCuentaDTO
            {
                Fecha = c.FechaEmision,
                Tipo = "Factura",
                Referencia = $"Factura a crédito {folio}",
                Cargo = c.MontoTotal,
                Abono = 0,
                Observaciones = c.Observaciones
            });

            foreach (var p in c.Pagos.Where(x => x.IsActive))
            {
                movimientosSinSaldo.Add(new MovimientoEstadoCuentaDTO
                {
                    Fecha = p.FechaPago,
                    Tipo = "Abono",
                    Referencia = $"Abono a {folio}" + (!string.IsNullOrWhiteSpace(p.ReferenciaBancaria) ? $" (Ref: {p.ReferenciaBancaria})" : ""),
                    Cargo = 0,
                    Abono = p.Monto,
                    MetodoPago = p.MetodoPago?.Descripcion ?? "Efectivo",
                    Observaciones = p.Observaciones
                });
            }
        }

        // Ordenar por fecha y calcular saldo acumulado progresivo
        decimal acumulado = 0;
        var ordenados = movimientosSinSaldo
            .OrderBy(m => m.Fecha)
            .ThenBy(m => m.Tipo == "Factura" ? 0 : 1)
            .ToList();

        foreach (var m in ordenados)
        {
            acumulado += (m.Cargo - m.Abono);
            m.SaldoAcumulado = acumulado;
            estado.Movimientos.Add(m);
        }

        response.Data = estado;
        response.isSuccess = true;
        return response;
    }

    public async Task<Response<List<TurnoActivoDTO>>> ObtenerTurnosActivosAsync(int idSucursal)
    {
        var response = new Response<List<TurnoActivoDTO>>();

        var query = _context.Turnos
            .Include(t => t.Sucursal)
            .Include(t => t.Usuario)
            .Where(t => t.Cierre == null && t.IsActive)
            .AsNoTracking()
            .AsQueryable();

        if (idSucursal > 0)
            query = query.Where(t => t.IdSucursal == idSucursal);

        var list = await query
            .OrderByDescending(t => t.Apertura)
            .Select(t => new TurnoActivoDTO
            {
                IdTurno = t.Id,
                IdSucursal = t.IdSucursal,
                SucursalNombre = t.Sucursal.Nombre,
                IdUsuario = t.IdUsuario,
                UsuarioNombre = t.Usuario.NombreCompleto,
                Apertura = t.Apertura,
                CajaInicial = t.CajaInicial
            })
            .ToListAsync();

        response.Data = list;
        response.isSuccess = true;
        return response;
    }
}
