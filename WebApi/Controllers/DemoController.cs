using Common;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class DemoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DemoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("SeedRestauranteCompleto")]
    public async Task<ActionResult<Response<object>>> SeedRestauranteCompleto([FromQuery] bool resetOrders = true)
    {
        try
        {
            // 1. Empresa y Sucursal
            var empresa = await _context.Empresas.FirstOrDefaultAsync();
            if (empresa == null)
            {
                empresa = new Empresa
                {
                    Nombre = "Bistró & Brasa La Central",
                    Rfc = "BBC240911001"
                };
                _context.Empresas.Add(empresa);
                await _context.SaveChangesAsync();
            }

            var sucursal = await _context.Sucursales.FirstOrDefaultAsync(s => s.IdEmpresa == empresa.Id);
            if (sucursal == null)
            {
                sucursal = new Sucursal
                {
                    IdEmpresa = empresa.Id,
                    Nombre = "Sucursal Centro Histórico",
                    Direccion = "Calle Madero 120, Centro",
                    ZonaHoraria = "America/Mexico_City"
                };
                _context.Sucursales.Add(sucursal);
                await _context.SaveChangesAsync();
            }

            // 2. Estaciones de Cocina
            var estaciones = await _context.EstacionesCocina.Where(e => e.IdSucursal == sucursal.Id).ToListAsync();
            EstacionCocina estacionParrilla = estaciones.FirstOrDefault(e => e.Nombre != null && e.Nombre.Contains("Parrilla"))!;
            EstacionCocina estacionCocina = estaciones.FirstOrDefault(e => e.Nombre != null && e.Nombre.Contains("Cocina"))!;
            EstacionCocina estacionBarra = estaciones.FirstOrDefault(e => e.Nombre != null && (e.Nombre.Contains("Barra") || e.Nombre.Contains("Bebidas")))!;

            if (estacionParrilla == null)
            {
                estacionParrilla = new EstacionCocina { IdSucursal = sucursal.Id, Nombre = "Parrilla & Brasa", MinutosAmbar = 7, MinutosRojo = 12 };
                _context.EstacionesCocina.Add(estacionParrilla);
            }
            if (estacionCocina == null)
            {
                estacionCocina = new EstacionCocina { IdSucursal = sucursal.Id, Nombre = "Cocina Caliente & Fría", MinutosAmbar = 8, MinutosRojo = 15 };
                _context.EstacionesCocina.Add(estacionCocina);
            }
            if (estacionBarra == null)
            {
                estacionBarra = new EstacionCocina { IdSucursal = sucursal.Id, Nombre = "Barra & Coctelería", MinutosAmbar = 4, MinutosRojo = 8 };
                _context.EstacionesCocina.Add(estacionBarra);
            }
            await _context.SaveChangesAsync();

            // 2.1 Catálogo tipado de Estaciones de Cocina (CatEstacionesCocina requerido por FK en Producto)
            var catEstaciones = await _context.CatEstacionesCocina.ToListAsync();
            var catParrilla = catEstaciones.FirstOrDefault(e => e.Descripcion != null && e.Descripcion.Contains("Parrilla"));
            var catCocina = catEstaciones.FirstOrDefault(e => e.Descripcion != null && e.Descripcion.Contains("Cocina"));
            var catBarra = catEstaciones.FirstOrDefault(e => e.Descripcion != null && (e.Descripcion.Contains("Barra") || e.Descripcion.Contains("Bebidas")));

            if (catParrilla == null)
            {
                catParrilla = new CatEstacionesCocina { Descripcion = "Parrilla & Brasa", IsActive = true, CreatedBy = "demo-seed" };
                _context.CatEstacionesCocina.Add(catParrilla);
            }
            if (catCocina == null)
            {
                catCocina = new CatEstacionesCocina { Descripcion = "Cocina Caliente & Fría", IsActive = true, CreatedBy = "demo-seed" };
                _context.CatEstacionesCocina.Add(catCocina);
            }
            if (catBarra == null)
            {
                catBarra = new CatEstacionesCocina { Descripcion = "Barra & Coctelería", IsActive = true, CreatedBy = "demo-seed" };
                _context.CatEstacionesCocina.Add(catBarra);
            }
            await _context.SaveChangesAsync();

            // 3. Áreas
            var areasExistentes = await _context.Areas.Where(a => a.IdSucursal == sucursal.Id).ToListAsync();
            Area areaSalon = areasExistentes.FirstOrDefault(a => a.Nombre != null && a.Nombre.Contains("Salón"))!;
            Area areaTerraza = areasExistentes.FirstOrDefault(a => a.Nombre != null && a.Nombre.Contains("Terraza"))!;
            Area areaBarra = areasExistentes.FirstOrDefault(a => a.Nombre != null && a.Nombre.Contains("Barra"))!;
            Area areaVip = areasExistentes.FirstOrDefault(a => a.Nombre != null && a.Nombre.Contains("VIP"))!;

            if (areaSalon == null)
            {
                areaSalon = new Area { IdSucursal = sucursal.Id, Nombre = "Salón Principal", Orden = 1 };
                _context.Areas.Add(areaSalon);
            }
            if (areaTerraza == null)
            {
                areaTerraza = new Area { IdSucursal = sucursal.Id, Nombre = "Terraza Jardín", Orden = 2 };
                _context.Areas.Add(areaTerraza);
            }
            if (areaBarra == null)
            {
                areaBarra = new Area { IdSucursal = sucursal.Id, Nombre = "Barra & Lounge", Orden = 3 };
                _context.Areas.Add(areaBarra);
            }
            if (areaVip == null)
            {
                areaVip = new Area { IdSucursal = sucursal.Id, Nombre = "Privado VIP", Orden = 4 };
                _context.Areas.Add(areaVip);
            }
            await _context.SaveChangesAsync();

            // 4. Catálogo de Estados Mesa
            var estMesaDisp = await _context.CatEstadosMesa.FirstOrDefaultAsync(e => e.Descripcion.ToLower().Contains("disponible"));
            var estMesaOcup = await _context.CatEstadosMesa.FirstOrDefaultAsync(e => e.Descripcion.ToLower().Contains("ocupada"));
            var estMesaSucia = await _context.CatEstadosMesa.FirstOrDefaultAsync(e => e.Descripcion.ToLower().Contains("sucia"));
            var estMesaRes = await _context.CatEstadosMesa.FirstOrDefaultAsync(e => e.Descripcion.ToLower().Contains("reservada"));

            int idDisp = estMesaDisp?.Id ?? 1;
            int idOcup = estMesaOcup?.Id ?? 2;
            int idRes = estMesaRes?.Id ?? 3;
            int idSucia = estMesaSucia?.Id ?? 4;

            // 5. Mesas
            var mesasExistentes = await _context.Mesas.ToListAsync();
            var mesasDef = new List<(string codigo, int asientos, int idArea, int estadoInicial)>
            {
                ("M1", 4, areaSalon.Id, idDisp),
                ("M2", 4, areaSalon.Id, idOcup),
                ("M3", 6, areaSalon.Id, idDisp),
                ("M4", 4, areaSalon.Id, idOcup),
                ("M5", 2, areaSalon.Id, idDisp),
                ("M6", 6, areaSalon.Id, idSucia),
                ("T1", 4, areaTerraza.Id, idDisp),
                ("T2", 4, areaTerraza.Id, idOcup),
                ("T3", 2, areaTerraza.Id, idDisp),
                ("T4", 4, areaTerraza.Id, idDisp),
                ("B1", 2, areaBarra.Id, idDisp),
                ("B2", 2, areaBarra.Id, idDisp),
                ("B3", 2, areaBarra.Id, idDisp),
                ("VIP1", 10, areaVip.Id, idRes),
            };

            var mesasCreadas = new List<Mesa>();
            foreach (var def in mesasDef)
            {
                var mesa = mesasExistentes.FirstOrDefault(m => m.Codigo == def.codigo);
                if (mesa == null)
                {
                    mesa = new Mesa
                    {
                        IdSucursal = sucursal.Id,
                        Codigo = def.codigo,
                        Asientos = def.asientos,
                        IdArea = def.idArea,
                        IdEstadoMesa = def.estadoInicial
                    };
                    _context.Mesas.Add(mesa);
                }
                else
                {
                    mesa.IdSucursal = sucursal.Id;
                    mesa.IdArea = def.idArea;
                    mesa.IdEstadoMesa = def.estadoInicial;
                    mesa.Asientos = def.asientos;
                }
                mesasCreadas.Add(mesa);
            }
            await _context.SaveChangesAsync();

            // 6. Menú y Categorías
            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.IdSucursal == sucursal.Id);
            if (menu == null)
            {
                menu = new Menu { IdSucursal = sucursal.Id, Nombre = "Menú Principal" };
                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();
            }

            var categoriasExistentes = await _context.CategoriaMenus.Where(c => c.IdMenu == menu.Id).ToListAsync();
            var catDefs = new[] { "Cortes & Parrilla", "Hamburguesas Gourmet", "Entradas & Tapas", "Coctelería de Autor", "Vinos & Cervezas", "Postres Artesanales" };
            var catMap = new Dictionary<string, CategoriaMenu>();

            for (int i = 0; i < catDefs.Length; i++)
            {
                var nombreCat = catDefs[i];
                var c = categoriasExistentes.FirstOrDefault(cat => cat.Nombre == nombreCat);
                if (c == null)
                {
                    c = new CategoriaMenu { IdMenu = menu.Id, Nombre = nombreCat, Orden = i + 1 };
                    _context.CategoriaMenus.Add(c);
                }
                catMap[nombreCat] = c;
            }
            await _context.SaveChangesAsync();

            // Catálogos auxiliares
            var catImpuestos = await _context.CatImpuestos.ToListAsync();
            int idImpuestoIva = catImpuestos.FirstOrDefault(i => i.Descripcion.ToLower().Contains("iva"))?.Id ?? 1;

            var catMonedas = await _context.CatMonedas.ToListAsync();
            int idMonedaMxn = catMonedas.FirstOrDefault(m => m.Descripcion != null && (m.Descripcion.ToUpper().Contains("MXN") || m.Descripcion.ToLower().Contains("peso")))?.Id ?? 1;

            // 7. Productos con Variantes y Precios
            var prodsExistentes = await _context.Productos.Where(p => p.IdMenu == menu.Id).ToListAsync();
            var prodsList = new List<(string nombre, string cat, decimal precio, int idEstacion, string desc)>
            {
                ("Ribeye Choice 400g", "Cortes & Parrilla", 480m, catParrilla.Id, "Corte marmoleado asado al carbón con sal de mar y romero"),
                ("Picaña al Carbón 350g", "Cortes & Parrilla", 390m, catParrilla.Id, "Servida con chiles toreados y chimichurri rústico"),
                ("Vacío Argentino 300g", "Cortes & Parrilla", 360m, catParrilla.Id, "Corte suave y jugoso a las brasas"),
                ("Hamburguesa Brasa Ahumada", "Hamburguesas Gourmet", 220m, catParrilla.Id, "Carne Angus 200g, cheddar añejo, tocino ahumado y cebolla caramelizada"),
                ("Hamburguesa Trufa & Portobello", "Hamburguesas Gourmet", 250m, catParrilla.Id, "Carne Angus, portobello braseado, queso suizo y mayonesa de trufa"),
                ("Carpaccio de Res Trufado", "Entradas & Tapas", 185m, catCocina.Id, "Láminas finas de lomo de res con arúgula, alcaparras y lascas de parmesano"),
                ("Tuétanos Asados con Esquites", "Entradas & Tapas", 165m, catCocina.Id, "Dos canoas de tuétano a la brasa con esquites tiernos y epazote"),
                ("Tabla de Quesos & Jamón Serrano", "Entradas & Tapas", 240m, catCocina.Id, "Selección de quesos madurados, nueces garrapiñadas y pan campesino"),
                ("Smoked Mezcalita Frutos Rojos", "Coctelería de Autor", 160m, catBarra.Id, "Mezcal espadín artesanal, infusión de frutos rojos y sal de gusano"),
                ("Gin & Tonic Botánico", "Coctelería de Autor", 150m, catBarra.Id, "Ginebra premium, tónica artesanal, pepino fresco y bayas de enebro"),
                ("Carajillo Shakeado", "Coctelería de Autor", 140m, catBarra.Id, "Licor 43 y shot de espresso recién extraído batido al punto"),
                ("Cerveza Artesanal IPA 355ml", "Vinos & Cervezas", 95m, catBarra.Id, "Notas cítricas, lúpulo intenso y amargor equilibrado"),
                ("Cerveza Ultra 355ml", "Vinos & Cervezas", 65m, catBarra.Id, "Cerveza clara ligera y refrescante"),
                ("Copa Ensamble Tinto", "Vinos & Cervezas", 130m, catBarra.Id, "Valle de Guadalupe (Cabernet Sauvignon & Merlot)"),
                ("Volcán de Dulce de Leche", "Postres Artesanales", 125m, catCocina.Id, "Centro líquido tibio con helado de vainilla de Papantla"),
                ("Cheesecake Frutos del Bosque", "Postres Artesanales", 115m, catCocina.Id, "Estilo New York horneado con coulis de frambuesa y zarzamora")
            };

            var productosMap = new Dictionary<string, (Producto prod, VarianteProducto var, decimal precio)>();

            foreach (var item in prodsList)
            {
                var prod = prodsExistentes.FirstOrDefault(p => p.Nombre == item.nombre);
                if (prod == null)
                {
                    prod = new Producto
                    {
                        IdMenu = menu.Id,
                        IdCategoria = catMap[item.cat].Id,
                        Nombre = item.nombre,
                        Descripcion = item.desc,
                        IdEstacionCocina = item.idEstacion,
                        Activo = true
                    };
                    _context.Productos.Add(prod);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    prod.IdEstacionCocina = item.idEstacion;
                    prod.Descripcion = item.desc;
                    prod.Activo = true;
                    await _context.SaveChangesAsync();
                }

                var variante = await _context.VarianteProductos.FirstOrDefaultAsync(v => v.IdProducto == prod.Id);
                if (variante == null)
                {
                    variante = new VarianteProducto
                    {
                        IdProducto = prod.Id,
                        Nombre = "Regular",
                        Codigo = $"VAR-{prod.Id}",
                        EsDefault = true
                    };
                    _context.VarianteProductos.Add(variante);
                    await _context.SaveChangesAsync();
                }

                var precio = await _context.Precios.FirstOrDefaultAsync(p => p.IdVariante == variante.Id);
                if (precio == null)
                {
                    precio = new Precio
                    {
                        IdVariante = variante.Id,
                        Monto = item.precio,
                        Moneda = "MXN",
                        IdImpuesto = idImpuestoIva,
                        IdMoneda = idMonedaMxn
                    };
                    _context.Precios.Add(precio);
                    await _context.SaveChangesAsync();
                }

                productosMap[item.nombre] = (prod, variante, item.precio);
            }

            // 8. Usuario para asignación de Turno y Pedidos
            var usuario = await _context.Usuarios.FirstOrDefaultAsync();
            if (usuario == null)
            {
                usuario = new Usuario
                {
                    IdEmpresa = empresa.Id,
                    NombreCompleto = "Carlos Mendoza",
                    Correo = "carlos.mendoza@bistrolacentral.mx"
                };
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
            }

            // 9. Turno Activo
            var turno = await _context.Turnos.FirstOrDefaultAsync(t => t.IdSucursal == sucursal.Id && t.Cierre == null);
            var ahora = DateTime.UtcNow;
            if (turno == null)
            {
                turno = new Turno
                {
                    IdUsuario = usuario.Id,
                    IdSucursal = sucursal.Id,
                    Apertura = ahora.AddHours(-5),
                    Cierre = null,
                    CajaInicial = 2000.00m
                };
                _context.Turnos.Add(turno);
                await _context.SaveChangesAsync();

                _context.MovimientosCaja.Add(new MovimientoCaja
                {
                    IdTurno = turno.Id,
                    Tipo = "Ingreso",
                    Monto = 500.00m,
                    Nota = "Fondo de cambio adicional en billetes de $50 y monedas"
                });
                _context.MovimientosCaja.Add(new MovimientoCaja
                {
                    IdTurno = turno.Id,
                    Tipo = "Egreso",
                    Monto = 200.00m,
                    Nota = "Compra urgente de 2 bolsas de hielo frappé"
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                // Asegurar que el turno activo inicie antes de las ventas simuladas
                turno.Apertura = ahora.AddHours(-5);
                if (turno.CajaInicial <= 0) turno.CajaInicial = 2000.00m;
                await _context.SaveChangesAsync();

                var movs = await _context.MovimientosCaja.Where(m => m.IdTurno == turno.Id).ToListAsync();
                if (!movs.Any())
                {
                    _context.MovimientosCaja.Add(new MovimientoCaja
                    {
                        IdTurno = turno.Id,
                        Tipo = "Ingreso",
                        Monto = 500.00m,
                        Nota = "Fondo de cambio adicional en billetes de $50 y monedas"
                    });
                    _context.MovimientosCaja.Add(new MovimientoCaja
                    {
                        IdTurno = turno.Id,
                        Tipo = "Egreso",
                        Monto = 200.00m,
                        Nota = "Compra urgente de 2 bolsas de hielo frappé"
                    });
                    await _context.SaveChangesAsync();
                }
            }

            // Catálogos adicionales
            var catTiposPedido = await _context.CatTiposPedido.ToListAsync();
            var tipoComedor = catTiposPedido.FirstOrDefault(t => t.Descripcion.ToLower().Contains("comedor"))?.Id ?? 1;

            var catEstPedidos = await _context.CatEstadosPedido.ToListAsync();
            int estReg = catEstPedidos.FirstOrDefault(e => e.Descripcion.ToLower().Contains("registrado"))?.Id ?? 1;
            int estPrep = catEstPedidos.FirstOrDefault(e => e.Descripcion.ToLower().Contains("preparaci"))?.Id ?? 2;
            int estListo = catEstPedidos.FirstOrDefault(e => e.Descripcion.ToLower().Contains("listo"))?.Id ?? 3;
            int estEntregado = catEstPedidos.FirstOrDefault(e => e.Descripcion.ToLower().Contains("entregado"))?.Id ?? 4;
            int estCerrado = catEstPedidos.FirstOrDefault(e => e.Descripcion.ToLower().Contains("cerrado"))?.Id ?? 5;

            var catMetodos = await _context.CatMetodosDePago.ToListAsync();
            int idMetEfectivo = catMetodos.FirstOrDefault(m => m.Descripcion.ToLower().Contains("efectivo"))?.Id ?? 1;
            int idMetTarjeta = catMetodos.FirstOrDefault(m => m.Descripcion.ToLower().Contains("tarjeta"))?.Id ?? 2;
            int idMetTransf = catMetodos.FirstOrDefault(m => m.Descripcion.ToLower().Contains("transfer"))?.Id ?? (catMetodos.Count > 2 ? catMetodos[2].Id : 1);

            var catEstCuenta = await _context.CatEstadosCuenta.ToListAsync();
            int idEstCuentaPagada = catEstCuenta.FirstOrDefault(c => c.Descripcion.ToLower().Contains("pagad") || c.Descripcion.ToLower().Contains("cerrad"))?.Id ?? 2;
            int idEstCuentaAbierta = catEstCuenta.FirstOrDefault(c => c.Descripcion.ToLower().Contains("abiert") || c.Descripcion.ToLower().Contains("pendient"))?.Id ?? 1;

            var catEstTicket = await _context.CatEstadosTicketCocina.ToListAsync();
            int idTicketPend = catEstTicket.FirstOrDefault(t => t.Descripcion.ToLower().Contains("pend"))?.Id ?? 1;
            int idTicketPrep = catEstTicket.FirstOrDefault(t => t.Descripcion.ToLower().Contains("prep"))?.Id ?? 2;

            var catEstItemKds = await _context.CatEstadosItemKDS.ToListAsync();
            int idItemKdsPend = catEstItemKds.FirstOrDefault(i => i.Descripcion.ToLower().Contains("pend"))?.Id ?? 1;

            var catEstPedDet = await _context.CatEstadosPedidoDetalle.ToListAsync();
            int idDetReg = catEstPedDet.FirstOrDefault(d => d.Descripcion.ToLower().Contains("reg") || d.Descripcion.ToLower().Contains("pend"))?.Id ?? 1;
            int idDetListo = catEstPedDet.FirstOrDefault(d => d.Descripcion.ToLower().Contains("list") || d.Descripcion.ToLower().Contains("entreg"))?.Id ?? 2;

            // 10. Limpieza selectiva si resetOrders = true
            if (resetOrders)
            {
                var pedidosAnteriores = await _context.Pedidos
                    .Include(p => p.Detalles)
                    .Include(p => p.Cuentas).ThenInclude(c => c.Pagos)
                    .Include(p => p.TicketsCocina).ThenInclude(t => t.Detalles)
                    .ToListAsync();

                foreach (var p in pedidosAnteriores)
                {
                    foreach (var c in p.Cuentas)
                    {
                        _context.Pagos.RemoveRange(c.Pagos);
                    }
                    _context.Cuentas.RemoveRange(p.Cuentas);
                    foreach (var t in p.TicketsCocina)
                    {
                        _context.TicketDetalles.RemoveRange(t.Detalles);
                    }
                    _context.TicketsCocina.RemoveRange(p.TicketsCocina);
                    _context.PedidoDetalles.RemoveRange(p.Detalles);
                }
                _context.Pedidos.RemoveRange(pedidosAnteriores);
                await _context.SaveChangesAsync();
            }

            // 11. Generar 14 Pedidos Cobrados Hoy
            var random = new Random(42);
            var listaPlatillos = prodsList.Select(p => p.nombre).ToList();
            var metodosArray = new[] { idMetEfectivo, idMetTarjeta, idMetTarjeta, idMetTransf };
            decimal totalVentasSimuladas = 0;

            for (int i = 1; i <= 14; i++)
            {
                var horaApertura = turno.Apertura.AddMinutes((i - 1) * 16 + 10);
                var horaCierre = horaApertura.AddMinutes(random.Next(12, 22));
                if (horaCierre >= ahora) horaCierre = ahora.AddMinutes(-5);
                var personas = random.Next(2, 5);

                var pedido = new Pedido
                {
                    IdEmpresa = empresa.Id,
                    IdSucursal = sucursal.Id,
                    IdMesa = mesasCreadas[random.Next(0, mesasCreadas.Count)].Id,
                    Personas = personas,
                    AbiertoPor = usuario.Id,
                    CerradoPor = usuario.Id,
                    AbiertoEn = horaApertura,
                    CerradoEn = horaCierre,
                    IdTipoPedido = tipoComedor,
                    IdEstadoPedido = estCerrado,
                    CanalOrigen = "POS"
                };
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                decimal pedidoTotal = 0;
                int numPlatillos = random.Next(2, 5);
                for (int d = 0; d < numPlatillos; d++)
                {
                    var platilloNombre = listaPlatillos[random.Next(0, listaPlatillos.Count)];
                    var (pInfo, vInfo, precio) = productosMap[platilloNombre];
                    int cant = random.Next(1, 3);
                    decimal sub = precio * cant;
                    pedidoTotal += sub;

                    var det = new PedidoDetalle
                    {
                        IdPedido = pedido.Id,
                        IdProducto = pInfo.Id,
                        IdVariante = vInfo.Id,
                        ProductoNombre = pInfo.Nombre,
                        VarianteNombre = vInfo.Nombre,
                        Cantidad = cant,
                        PrecioUnitario = precio,
                        IdImpuesto = idImpuestoIva,
                        TasaImpuesto = 16m,
                        MontoImpuesto = sub * 0.16m,
                        IdEstadoPedidoDetalle = idDetListo,
                        Cancelado = false
                    };
                    _context.PedidoDetalles.Add(det);
                }
                await _context.SaveChangesAsync();

                var cuenta = new Cuenta
                {
                    IdPedido = pedido.Id,
                    Subtotal = pedidoTotal / 1.16m,
                    ImpuestoTotal = pedidoTotal - (pedidoTotal / 1.16m),
                    Total = pedidoTotal,
                    IdEstadoCuenta = idEstCuentaPagada
                };
                _context.Cuentas.Add(cuenta);
                await _context.SaveChangesAsync();

                var metodoElegido = metodosArray[random.Next(0, metodosArray.Length)];
                var pago = new Pago
                {
                    IdCuenta = cuenta.Id,
                    Monto = pedidoTotal,
                    Moneda = "MXN",
                    Propina = Math.Round(pedidoTotal * 0.10m, 2),
                    PagadoEn = horaCierre,
                    IdMetodoDePago = metodoElegido,
                    RecibidoPor = usuario.Id,
                    IsActive = true,
                    CreatedAt = horaCierre,
                    CreatedBy = "demo-seed"
                };
                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                totalVentasSimuladas += pedidoTotal;
            }

            // 12. Generar 3 Pedidos Vivos en Curso
            var mesaM2 = mesasCreadas.FirstOrDefault(m => m.Codigo == "M2");
            var mesaM4 = mesasCreadas.FirstOrDefault(m => m.Codigo == "M4");
            var mesaT2 = mesasCreadas.FirstOrDefault(m => m.Codigo == "T2");

            // Mesa M2: Comiendo (Entregado)
            if (mesaM2 != null)
            {
                mesaM2.IdEstadoMesa = idOcup;
                var pedM2 = new Pedido
                {
                    IdEmpresa = empresa.Id,
                    IdSucursal = sucursal.Id,
                    IdMesa = mesaM2.Id,
                    Personas = 4,
                    AbiertoPor = usuario.Id,
                    AbiertoEn = ahora.AddMinutes(-40),
                    IdTipoPedido = tipoComedor,
                    IdEstadoPedido = estEntregado,
                    CanalOrigen = "POS"
                };
                _context.Pedidos.Add(pedM2);
                await _context.SaveChangesAsync();

                var pRibeye = productosMap["Ribeye Choice 400g"];
                var pCarpaccio = productosMap["Carpaccio de Res Trufado"];
                var pMezcal = productosMap["Smoked Mezcalita Frutos Rojos"];

                _context.PedidoDetalles.Add(new PedidoDetalle { IdPedido = pedM2.Id, IdProducto = pRibeye.prod.Id, IdVariante = pRibeye.var.Id, ProductoNombre = pRibeye.prod.Nombre, VarianteNombre = pRibeye.var.Nombre, Cantidad = 2, PrecioUnitario = pRibeye.precio, IdImpuesto = idImpuestoIva, TasaImpuesto = 16, MontoImpuesto = (pRibeye.precio * 2) * 0.16m, IdEstadoPedidoDetalle = idDetListo });
                _context.PedidoDetalles.Add(new PedidoDetalle { IdPedido = pedM2.Id, IdProducto = pCarpaccio.prod.Id, IdVariante = pCarpaccio.var.Id, ProductoNombre = pCarpaccio.prod.Nombre, VarianteNombre = pCarpaccio.var.Nombre, Cantidad = 1, PrecioUnitario = pCarpaccio.precio, IdImpuesto = idImpuestoIva, TasaImpuesto = 16, MontoImpuesto = pCarpaccio.precio * 0.16m, IdEstadoPedidoDetalle = idDetListo });
                _context.PedidoDetalles.Add(new PedidoDetalle { IdPedido = pedM2.Id, IdProducto = pMezcal.prod.Id, IdVariante = pMezcal.var.Id, ProductoNombre = pMezcal.prod.Nombre, VarianteNombre = pMezcal.var.Nombre, Cantidad = 2, PrecioUnitario = pMezcal.precio, IdImpuesto = idImpuestoIva, TasaImpuesto = 16, MontoImpuesto = (pMezcal.precio * 2) * 0.16m, IdEstadoPedidoDetalle = idDetListo });
                await _context.SaveChangesAsync();
            }

            // Mesa M4: Por Cobrar (Pre-cuenta solicitada)
            if (mesaM4 != null)
            {
                mesaM4.IdEstadoMesa = idOcup;
                var pedM4 = new Pedido
                {
                    IdEmpresa = empresa.Id,
                    IdSucursal = sucursal.Id,
                    IdMesa = mesaM4.Id,
                    Personas = 2,
                    AbiertoPor = usuario.Id,
                    AbiertoEn = ahora.AddMinutes(-30),
                    IdTipoPedido = tipoComedor,
                    IdEstadoPedido = estEntregado,
                    CanalOrigen = "POS"
                };
                _context.Pedidos.Add(pedM4);
                await _context.SaveChangesAsync();

                var pBurger = productosMap["Hamburguesa Brasa Ahumada"];
                var pVolcan = productosMap["Volcán de Dulce de Leche"];
                var pCarajillo = productosMap["Carajillo Shakeado"];

                _context.PedidoDetalles.Add(new PedidoDetalle { IdPedido = pedM4.Id, IdProducto = pBurger.prod.Id, IdVariante = pBurger.var.Id, ProductoNombre = pBurger.prod.Nombre, VarianteNombre = pBurger.var.Nombre, Cantidad = 2, PrecioUnitario = pBurger.precio, IdImpuesto = idImpuestoIva, TasaImpuesto = 16, MontoImpuesto = (pBurger.precio * 2) * 0.16m, IdEstadoPedidoDetalle = idDetListo });
                _context.PedidoDetalles.Add(new PedidoDetalle { IdPedido = pedM4.Id, IdProducto = pVolcan.prod.Id, IdVariante = pVolcan.var.Id, ProductoNombre = pVolcan.prod.Nombre, VarianteNombre = pVolcan.var.Nombre, Cantidad = 1, PrecioUnitario = pVolcan.precio, IdImpuesto = idImpuestoIva, TasaImpuesto = 16, MontoImpuesto = pVolcan.precio * 0.16m, IdEstadoPedidoDetalle = idDetListo });
                _context.PedidoDetalles.Add(new PedidoDetalle { IdPedido = pedM4.Id, IdProducto = pCarajillo.prod.Id, IdVariante = pCarajillo.var.Id, ProductoNombre = pCarajillo.prod.Nombre, VarianteNombre = pCarajillo.var.Nombre, Cantidad = 2, PrecioUnitario = pCarajillo.precio, IdImpuesto = idImpuestoIva, TasaImpuesto = 16, MontoImpuesto = (pCarajillo.precio * 2) * 0.16m, IdEstadoPedidoDetalle = idDetListo });
                await _context.SaveChangesAsync();

                decimal totalM4 = (pBurger.precio * 2) + pVolcan.precio + (pCarajillo.precio * 2);
                _context.Cuentas.Add(new Cuenta
                {
                    IdPedido = pedM4.Id,
                    Subtotal = totalM4 / 1.16m,
                    ImpuestoTotal = totalM4 - (totalM4 / 1.16m),
                    Total = totalM4,
                    IdEstadoCuenta = idEstCuentaAbierta
                });
                await _context.SaveChangesAsync();
            }

            // Mesa T2: En Cocina con Tickets KDS
            if (mesaT2 != null)
            {
                mesaT2.IdEstadoMesa = idOcup;
                var pedT2 = new Pedido
                {
                    IdEmpresa = empresa.Id,
                    IdSucursal = sucursal.Id,
                    IdMesa = mesaT2.Id,
                    Personas = 3,
                    AbiertoPor = usuario.Id,
                    AbiertoEn = ahora.AddMinutes(-12),
                    IdTipoPedido = tipoComedor,
                    IdEstadoPedido = estPrep,
                    CanalOrigen = "POS"
                };
                _context.Pedidos.Add(pedT2);
                await _context.SaveChangesAsync();

                var pVacio = productosMap["Vacío Argentino 300g"];
                var pTuetanos = productosMap["Tuétanos Asados con Esquites"];
                var pIpa = productosMap["Cerveza Artesanal IPA 355ml"];

                var detVacio = new PedidoDetalle { IdPedido = pedT2.Id, IdProducto = pVacio.prod.Id, IdVariante = pVacio.var.Id, ProductoNombre = pVacio.prod.Nombre, VarianteNombre = pVacio.var.Nombre, Cantidad = 2, PrecioUnitario = pVacio.precio, IdImpuesto = idImpuestoIva, TasaImpuesto = 16, MontoImpuesto = (pVacio.precio * 2) * 0.16m, IdEstadoPedidoDetalle = idDetReg, Notas = "Término 3/4 bien sellado" };
                var detTuetano = new PedidoDetalle { IdPedido = pedT2.Id, IdProducto = pTuetanos.prod.Id, IdVariante = pTuetanos.var.Id, ProductoNombre = pTuetanos.prod.Nombre, VarianteNombre = pTuetanos.var.Nombre, Cantidad = 1, PrecioUnitario = pTuetanos.precio, IdImpuesto = idImpuestoIva, TasaImpuesto = 16, MontoImpuesto = pTuetanos.precio * 0.16m, IdEstadoPedidoDetalle = idDetReg };
                var detIpa = new PedidoDetalle { IdPedido = pedT2.Id, IdProducto = pIpa.prod.Id, IdVariante = pIpa.var.Id, ProductoNombre = pIpa.prod.Nombre, VarianteNombre = pIpa.var.Nombre, Cantidad = 3, PrecioUnitario = pIpa.precio, IdImpuesto = idImpuestoIva, TasaImpuesto = 16, MontoImpuesto = (pIpa.precio * 3) * 0.16m, IdEstadoPedidoDetalle = idDetReg, Notas = "Bien frías" };

                _context.PedidoDetalles.AddRange(detVacio, detTuetano, detIpa);
                await _context.SaveChangesAsync();

                var ticketParrilla = new TicketCocina
                {
                    IdEstacion = estacionParrilla.Id,
                    IdPedido = pedT2.Id,
                    IdEstadoTicketCocina = idTicketPrep
                };
                _context.TicketsCocina.Add(ticketParrilla);
                await _context.SaveChangesAsync();

                _context.TicketDetalles.Add(new TicketDetalle { IdTicket = ticketParrilla.Id, IdDetalle = detVacio.Id, IdEstadoItemKDS = idItemKdsPend });

                var ticketBarra = new TicketCocina
                {
                    IdEstacion = estacionBarra.Id,
                    IdPedido = pedT2.Id,
                    IdEstadoTicketCocina = idTicketPend
                };
                _context.TicketsCocina.Add(ticketBarra);
                await _context.SaveChangesAsync();

                _context.TicketDetalles.Add(new TicketDetalle { IdTicket = ticketBarra.Id, IdDetalle = detIpa.Id, IdEstadoItemKDS = idItemKdsPend });

                await _context.SaveChangesAsync();
            }

            // 13. Spec 021: Suscripción de ejemplo en Tier3 (Multi-Sucursal) para la empresa demo.
            // En demostraciones comerciales todo debe verse desbloqueado; se le asigna el tier
            // más alto sin importar que FeatureGating:Enabled esté en false por defecto.
            var planTier3 = await _context.CatPlanesSuscripcion.FirstOrDefaultAsync(p => p.Codigo == Domain.Entities.CodigosPlanSuscripcion.Tier3_Multi);
            if (planTier3 != null)
            {
                var suscripcionDemo = await _context.EmpresasSuscripcion.FirstOrDefaultAsync(s => s.IdEmpresa == empresa.Id);
                if (suscripcionDemo == null)
                {
                    suscripcionDemo = new Domain.Entities.EmpresaSuscripcion
                    {
                        IdEmpresa = empresa.Id,
                        IdPlan = planTier3.Id,
                        EsPagoAnual = false,
                        FechaInicio = DateTime.UtcNow.AddMonths(-1),
                        FechaFinVigencia = DateTime.UtcNow.AddMonths(1),
                        EstadoSuscripcion = Domain.Entities.EstadoSuscripcionValores.Activa,
                        KdsAddonsContratados = 0,
                        ComanderosAddons = 0,
                        EnPeriodoGracia = false,
                        IsActive = true
                    };
                    _context.EmpresasSuscripcion.Add(suscripcionDemo);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new Response<object>
            {
                isSuccess = true,
                Message = "Escenario demo cargado exitosamente para 'Bistró & Brasa La Central'.",
                Data = new
                {
                    Restaurante = empresa.Nombre,
                    Sucursal = sucursal.Nombre,
                    TurnoId = turno.Id,
                    MesasCreadas = mesasCreadas.Count,
                    PedidosCobradosHoy = 14,
                    VentasAcumuladas = totalVentasSimuladas,
                    TicketsKdsVivos = 2,
                    MesasOcupadas = 3
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<object>
            {
                isSuccess = false,
                Message = $"Error al poblar datos demo: {ex.Message} -> {ex.InnerException?.Message}"
            });
        }
    }
}
