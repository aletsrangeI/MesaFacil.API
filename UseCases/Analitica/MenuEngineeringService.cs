using DTO.Analitica;
using Interface.UseCases;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.Analitica;

public class MenuEngineeringService : IMenuEngineeringService
{
    private readonly ApplicationDbContext _context;

    public MenuEngineeringService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuEngineeringReportDTO> ObtenerReporteIngenieriaMenuAsync(
        int? idSucursal,
        DateTime fechaInicio,
        DateTime fechaFin,
        int? idCategoria = null)
    {
        // 1. Filtrar los detalles de pedidos pagados y válidos en el rango de fechas
        var query = _context.PedidoDetalles
            .Include(d => d.Pedido)
            .Include(d => d.Producto)
                .ThenInclude(p => p.Categoria)
            .Where(d => d.Pedido.AbiertoEn >= fechaInicio && d.Pedido.AbiertoEn <= fechaFin)
            .Where(d => !d.Cancelado && d.IdEstadoPedidoDetalle != 5 && d.Pedido.IdEstadoPedido != 6);

        if (idSucursal.HasValue && idSucursal.Value > 0)
        {
            query = query.Where(d => d.Pedido.IdSucursal == idSucursal.Value);
        }

        if (idCategoria.HasValue && idCategoria.Value > 0)
        {
            query = query.Where(d => d.Producto.IdCategoria == idCategoria.Value);
        }

        var rawDetalles = await query
            .Select(d => new
            {
                d.IdProducto,
                NombreProducto = !string.IsNullOrEmpty(d.ProductoNombre)
                    ? d.ProductoNombre
                    : (d.Producto != null ? d.Producto.Nombre : $"Producto #{d.IdProducto}"),
                Categoria = (d.Producto != null && d.Producto.Categoria != null) ? d.Producto.Categoria.Nombre : "General",
                d.Cantidad,
                d.PrecioUnitario
            })
            .ToListAsync();

        // Si no hay ventas en el período, retornamos reporte vacío consistente
        if (!rawDetalles.Any())
        {
            return new MenuEngineeringReportDTO();
        }

        // 2. Agrupar por producto vendido
        var agrupados = rawDetalles
            .GroupBy(d => d.IdProducto)
            .Select(g => new
            {
                IdProducto = g.Key,
                Nombre = g.First().NombreProducto,
                Categoria = g.First().Categoria,
                UnidadesVendidas = (int)Math.Round(g.Sum(x => x.Cantidad)),
                IngresoTotal = g.Sum(x => x.Cantidad * x.PrecioUnitario)
            })
            .Where(x => x.UnidadesVendidas > 0)
            .ToList();

        var productIds = agrupados.Select(x => x.IdProducto).Distinct().ToList();

        // 3. Consultar recetas activas asociadas a estos productos
        var recetas = await _context.Recetas
            .Include(r => r.Detalles.Where(d => d.IsActive))
            .Where(r => r.IsActive && !r.EsSubReceta && r.IdProducto.HasValue && productIds.Contains(r.IdProducto.Value))
            .ToListAsync();

        var pendientesDeCosteo = new List<ItemSinCosteoDTO>();
        var itemsCosteados = new List<ItemIngenieriaMenuDTO>();

        foreach (var prod in agrupados)
        {
            var receta = recetas.FirstOrDefault(r => r.IdProducto == prod.IdProducto);
            decimal costoUnitario = 0m;

            if (receta != null)
            {
                decimal costoTotalLote = receta.Detalles.Where(d => d.IsActive).Sum(d => d.CostoCalculado);
                costoUnitario = receta.CostoEstimadoUnitario > 0
                    ? receta.CostoEstimadoUnitario
                    : (receta.Rendimiento > 0 ? Math.Round(costoTotalLote / receta.Rendimiento, 4) : costoTotalLote);
            }

            decimal pvpPromedio = prod.UnidadesVendidas > 0
                ? Math.Round(prod.IngresoTotal / prod.UnidadesVendidas, 2)
                : 0m;

            if (costoUnitario <= 0)
            {
                pendientesDeCosteo.Add(new ItemSinCosteoDTO
                {
                    IdProducto = prod.IdProducto,
                    NombreProducto = prod.Nombre,
                    Categoria = prod.Categoria,
                    UnidadesVendidas = prod.UnidadesVendidas,
                    IngresoTotal = Math.Round(prod.IngresoTotal, 2),
                    PrecioVentaPromedio = pvpPromedio
                });
            }
            else
            {
                decimal costoRecetaRedondeado = Math.Round(costoUnitario, 2);
                decimal margenUnitario = Math.Round(pvpPromedio - costoRecetaRedondeado, 2);
                decimal foodCostPct = pvpPromedio > 0
                    ? Math.Round((costoRecetaRedondeado / pvpPromedio) * 100m, 2)
                    : 0m;
                decimal utilidadTotal = Math.Round(margenUnitario * prod.UnidadesVendidas, 2);

                itemsCosteados.Add(new ItemIngenieriaMenuDTO
                {
                    IdProducto = prod.IdProducto,
                    NombreProducto = prod.Nombre,
                    Categoria = prod.Categoria,
                    PrecioVentaPromedio = pvpPromedio,
                    CostoReceta = costoRecetaRedondeado,
                    MargenContribucion = margenUnitario,
                    FoodCostPct = foodCostPct,
                    UnidadesVendidas = prod.UnidadesVendidas,
                    IngresoTotal = Math.Round(prod.IngresoTotal, 2),
                    UtilidadTotal = utilidadTotal
                });
            }
        }

        int nCosteados = itemsCosteados.Count;
        int totalUnidadesCosteadas = itemsCosteados.Sum(x => x.UnidadesVendidas);
        decimal totalVentasCosteadas = itemsCosteados.Sum(x => x.IngresoTotal);
        decimal totalUtilidadBruta = itemsCosteados.Sum(x => x.UtilidadTotal);

        // 4. Algoritmo Kasavana & Smith (1982)
        // Promedio Ponderado del Margen de Contribución del Menú
        decimal margenPromedio = totalUnidadesCosteadas > 0
            ? Math.Round(totalUtilidadBruta / totalUnidadesCosteadas, 2)
            : 0m;

        // Umbral de popularidad estándar: 70% de la cuota media por ítem
        decimal umbralPopularidadUnidades = nCosteados > 0
            ? Math.Round((0.70m * totalUnidadesCosteadas) / nCosteados, 2)
            : 0m;

        // Clasificar items
        foreach (var item in itemsCosteados)
        {
            item.PorcentajePopularidad = totalUnidadesCosteadas > 0
                ? Math.Round(((decimal)item.UnidadesVendidas / totalUnidadesCosteadas) * 100m, 2)
                : 0m;

            bool altoMargen = item.MargenContribucion >= margenPromedio;
            bool altaPopularidad = item.UnidadesVendidas >= umbralPopularidadUnidades;

            if (altoMargen && altaPopularidad)
            {
                item.Cuadrante = "Estrella";
                item.RecomendacionAccion = "Mantener receta y calidad estrictamente inalteradas. Posición visual privilegiada en la carta. Capacitar al personal para recomendación activa.";
            }
            else if (!altoMargen && altaPopularidad)
            {
                item.Cuadrante = "CaballoBatalla";
                item.RecomendacionAccion = "Subida quirúrgica de precio (+5% a +8%). Optimizar porción o renegociar costo de insumos clave con proveedores.";
            }
            else if (altoMargen && !altaPopularidad)
            {
                item.Cuadrante = "Puzzle";
                item.RecomendacionAccion = "Reposicionar en carta ('Sweet Spot'). Cambiar fotografía o nombre en el menú. Gamificar al personal con comisiones por venta.";
            }
            else // !altoMargen && !altaPopularidad
            {
                item.Cuadrante = "Perro";
                item.RecomendacionAccion = "Evaluar retiro del menú o sustitución en próxima renovación de carta. Evitar mermas de insumos exclusivos.";
            }
        }

        // 5. Consolidar KPIs generales
        decimal foodCostPromedioPonderado = totalVentasCosteadas > 0
            ? Math.Round((itemsCosteados.Sum(x => x.CostoReceta * x.UnidadesVendidas) / totalVentasCosteadas) * 100m, 2)
            : 0m;

        var platilloMasRentable = itemsCosteados.OrderByDescending(x => x.MargenContribucion).FirstOrDefault();
        var platilloMasVendido = itemsCosteados.OrderByDescending(x => x.UnidadesVendidas).FirstOrDefault();

        var kpis = new MenuEngineeringKpisDTO
        {
            FoodCostPromedioGeneral = foodCostPromedioPonderado,
            MargenPromedio = margenPromedio,
            PlatilloMasRentable = platilloMasRentable?.NombreProducto ?? "N/A",
            MargenPlatilloMasRentable = platilloMasRentable?.MargenContribucion ?? 0m,
            PlatilloMasVendido = platilloMasVendido?.NombreProducto ?? "N/A",
            UnidadesPlatilloMasVendido = platilloMasVendido?.UnidadesVendidas ?? 0,
            CantidadEstrellas = itemsCosteados.Count(x => x.Cuadrante == "Estrella"),
            CantidadCaballos = itemsCosteados.Count(x => x.Cuadrante == "CaballoBatalla"),
            CantidadPuzzles = itemsCosteados.Count(x => x.Cuadrante == "Puzzle"),
            CantidadPerros = itemsCosteados.Count(x => x.Cuadrante == "Perro"),
            CantidadSinCosteo = pendientesDeCosteo.Count
        };

        return new MenuEngineeringReportDTO
        {
            Kpis = kpis,
            MargenContribucionPromedio = margenPromedio,
            UmbralPopularidadUnidades = umbralPopularidadUnidades,
            TotalUnidadesVendidas = totalUnidadesCosteadas,
            TotalVentas = Math.Round(totalVentasCosteadas, 2),
            TotalUtilidadBruta = Math.Round(totalUtilidadBruta, 2),
            FoodCostPromedioPonderadoPct = foodCostPromedioPonderado,
            Items = itemsCosteados.OrderByDescending(x => x.UtilidadTotal).ToList(),
            PendientesDeCosteo = pendientesDeCosteo.OrderByDescending(x => x.UnidadesVendidas).ToList()
        };
    }
}
