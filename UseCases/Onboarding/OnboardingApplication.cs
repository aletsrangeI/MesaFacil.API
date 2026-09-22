using Common;
using Domain.Entities;
using DTO.Onboarding;
using Interface.Persistence;
using Interface.UseCases;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System.Text.RegularExpressions;
using Validator;

namespace UseCases.Onboarding;

public class OnboardingApplication : IOnboardingApplication
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly IAppLogger<OnboardingApplication> _logger;
    private readonly ProvisionarRestauranteValidator _validator;

    public OnboardingApplication(
        ApplicationDbContext context,
        IPasswordHasher hasher,
        IAppLogger<OnboardingApplication> logger,
        ProvisionarRestauranteValidator validator)
    {
        _context = context;
        _hasher = hasher;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Response<OnboardingEstadoDTO>> ObtenerEstadoAsync(int idEmpresa, CancellationToken ct = default)
    {
        var response = new Response<OnboardingEstadoDTO>();
        try
        {
            var sucursales = await _context.Sucursales
                .Where(s => s.IdEmpresa == idEmpresa && s.IsActive)
                .ToListAsync(ct);

            var sucursalIds = sucursales.Select(s => s.Id).ToList();

            int totalMesas = 0;
            int totalProductos = 0;
            bool tieneTurnoAbierto = false;

            if (sucursalIds.Count > 0)
            {
                totalMesas = await _context.Mesas
                    .CountAsync(m => sucursalIds.Contains(m.IdSucursal) && m.IsActive, ct);

                var menuIds = await _context.Menus
                    .Where(m => sucursalIds.Contains(m.IdSucursal) && m.IsActive)
                    .Select(m => m.Id)
                    .ToListAsync(ct);

                if (menuIds.Count > 0)
                {
                    totalProductos = await _context.Productos
                        .CountAsync(p => menuIds.Contains(p.IdMenu) && p.Activo && p.IsActive, ct);
                }

                tieneTurnoAbierto = await _context.Turnos
                    .AnyAsync(t => sucursalIds.Contains(t.IdSucursal) && t.Cierre == null && t.IsActive, ct);
            }

            // Criterio de completitud: tener sucursal, al menos 1 mesa y al menos 1 producto
            bool completado = sucursales.Count > 0 && totalMesas > 0 && totalProductos > 0;

            int pasoSugerido = 1;
            if (sucursales.Count == 0) pasoSugerido = 1;
            else if (totalMesas == 0) pasoSugerido = 2;
            else if (totalProductos == 0) pasoSugerido = 3;
            else if (!tieneTurnoAbierto) pasoSugerido = 5;

            response.Data = new OnboardingEstadoDTO
            {
                OnboardingCompletado = completado,
                TieneSucursales = sucursales.Count > 0,
                TotalMesas = totalMesas,
                TotalProductos = totalProductos,
                TieneTurnoAbierto = tieneTurnoAbierto,
                PasoSugerido = pasoSugerido
            };
            response.isSuccess = true;
            response.Message = "Estado consultado exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al consultar estado de onboarding para empresa {0}: {1}", idEmpresa, ex.Message);
            response.isSuccess = false;
            response.Message = "Error al consultar estado de onboarding";
        }

        return response;
    }

    public async Task<Response<ProvisionarRestauranteResponseDTO>> ProvisionarRestauranteAsync(
        ProvisionarRestauranteRequestDTO dto,
        int idUsuarioAdmin,
        int idEmpresa,
        CancellationToken ct = default)
    {
        var response = new Response<ProvisionarRestauranteResponseDTO>();

        var validationResult = await _validator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            response.isSuccess = false;
            response.Message = "Errores de validación en la solicitud de onboarding";
            response.Errors = validationResult.Errors;
            return response;
        }

        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            // 1. Empresa
            Empresa? empresa = null;
            if (idEmpresa > 0)
            {
                empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.Id == idEmpresa, ct);
            }

            if (empresa == null)
            {
                empresa = new Empresa
                {
                    Nombre = dto.DatosEmpresa.Nombre,
                    Rfc = string.IsNullOrWhiteSpace(dto.DatosEmpresa.Rfc) ? "XAXX010101000" : dto.DatosEmpresa.Rfc.Trim(),
                    IsActive = true,
                    CreatedBy = "onboarding"
                };
                _context.Empresas.Add(empresa);
                await _context.SaveChangesAsync(ct);
            }
            else
            {
                empresa.Nombre = dto.DatosEmpresa.Nombre;
                if (!string.IsNullOrWhiteSpace(dto.DatosEmpresa.Rfc))
                {
                    empresa.Rfc = dto.DatosEmpresa.Rfc.Trim();
                }
                empresa.IsActive = true;
                await _context.SaveChangesAsync(ct);
            }

            // 2. Sucursal Matriz
            var sucursal = await _context.Sucursales.FirstOrDefaultAsync(s => s.IdEmpresa == empresa.Id, ct);
            if (sucursal == null)
            {
                sucursal = new Sucursal
                {
                    IdEmpresa = empresa.Id,
                    Nombre = string.IsNullOrWhiteSpace(dto.DatosEmpresa.NombreSucursal) ? "Sucursal Matriz" : dto.DatosEmpresa.NombreSucursal,
                    Direccion = dto.DatosEmpresa.Direccion ?? "Dirección Principal",
                    ZonaHoraria = string.IsNullOrWhiteSpace(dto.DatosEmpresa.ZonaHoraria) ? "America/Mexico_City" : dto.DatosEmpresa.ZonaHoraria,
                    IsActive = true,
                    CreatedBy = "onboarding"
                };
                _context.Sucursales.Add(sucursal);
                await _context.SaveChangesAsync(ct);
            }
            else
            {
                sucursal.Nombre = dto.DatosEmpresa.NombreSucursal;
                if (!string.IsNullOrWhiteSpace(dto.DatosEmpresa.Direccion)) sucursal.Direccion = dto.DatosEmpresa.Direccion;
                if (!string.IsNullOrWhiteSpace(dto.DatosEmpresa.ZonaHoraria)) sucursal.ZonaHoraria = dto.DatosEmpresa.ZonaHoraria;
                sucursal.IsActive = true;
                await _context.SaveChangesAsync(ct);
            }

            // 3. Almacén General (Spec 014)
            var almacen = await _context.Almacenes.FirstOrDefaultAsync(a => a.IdSucursal == sucursal.Id, ct);
            if (almacen == null)
            {
                almacen = new Almacen
                {
                    IdSucursal = sucursal.Id,
                    Codigo = $"ALM-{sucursal.Id}-GEN",
                    Nombre = "Almacén General",
                    TipoAlmacen = "General",
                    EsPrincipal = true,
                    IsActive = true,
                    CreatedBy = "onboarding"
                };
                _context.Almacenes.Add(almacen);
                await _context.SaveChangesAsync(ct);
            }

            // 4. FoliadorSucursal (Spec 019)
            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
            var foliador = await _context.FoliadoresSucursal
                .FirstOrDefaultAsync(f => f.IdSucursal == sucursal.Id && f.Fecha == hoy, ct);
            if (foliador == null)
            {
                foliador = new FoliadorSucursal
                {
                    IdSucursal = sucursal.Id,
                    Fecha = hoy,
                    UltimoFolio = 0,
                    IsActive = true
                };
                _context.FoliadoresSucursal.Add(foliador);
                await _context.SaveChangesAsync(ct);
            }

            // 5. Estaciones de Cocina
            var estacionesNombres = dto.EstacionesCocina != null && dto.EstacionesCocina.Count > 0
                ? dto.EstacionesCocina
                : new List<EstacionCocinaOnboardingDTO>
                {
                    new() { Nombre = "Cocina Principal", MinutosAmbar = 7, MinutosRojo = 12 }
                };

            var catEstacionesMap = new Dictionary<string, CatEstacionesCocina>(StringComparer.OrdinalIgnoreCase);

            foreach (var estDto in estacionesNombres)
            {
                var catEst = await _context.CatEstacionesCocina.FirstOrDefaultAsync(c => c.Descripcion == estDto.Nombre, ct);
                if (catEst == null)
                {
                    catEst = new CatEstacionesCocina
                    {
                        Descripcion = estDto.Nombre,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    };
                    _context.CatEstacionesCocina.Add(catEst);
                    await _context.SaveChangesAsync(ct);
                }
                catEstacionesMap[estDto.Nombre] = catEst;

                var estOp = await _context.EstacionesCocina
                    .FirstOrDefaultAsync(e => e.IdSucursal == sucursal.Id && e.Nombre == estDto.Nombre, ct);
                if (estOp == null)
                {
                    estOp = new EstacionCocina
                    {
                        IdSucursal = sucursal.Id,
                        Nombre = estDto.Nombre,
                        MinutosAmbar = estDto.MinutosAmbar > 0 ? estDto.MinutosAmbar : 7,
                        MinutosRojo = estDto.MinutosRojo > 0 ? estDto.MinutosRojo : 12,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    };
                    _context.EstacionesCocina.Add(estOp);
                    await _context.SaveChangesAsync(ct);
                }
            }

            // 6. Áreas y Mesas
            var catEstadoDisp = await _context.CatEstadosMesa.FirstOrDefaultAsync(e => e.Descripcion.ToLower().Contains("disponible"), ct);
            int idEstadoDisponible = catEstadoDisp?.Id ?? 1;
            int mesasCreadas = 0;

            foreach (var areaDto in dto.AreasYMesas)
            {
                var area = await _context.Areas.FirstOrDefaultAsync(a => a.IdSucursal == sucursal.Id && a.Nombre == areaDto.NombreArea, ct);
                if (area == null)
                {
                    area = new Area
                    {
                        IdSucursal = sucursal.Id,
                        Nombre = areaDto.NombreArea,
                        Orden = areaDto.Orden,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    };
                    _context.Areas.Add(area);
                    await _context.SaveChangesAsync(ct);
                }

                int cantidad = areaDto.CantidadMesas > 0 ? areaDto.CantidadMesas : 4;
                string prefijo = string.IsNullOrWhiteSpace(areaDto.PrefijoMesa) ? "M" : areaDto.PrefijoMesa.Trim();
                int asientos = areaDto.AsientosPorMesa > 0 ? areaDto.AsientosPorMesa : 4;

                for (int i = 1; i <= cantidad; i++)
                {
                    string codigoMesa = $"{prefijo}{i}";
                    var mesaExistente = await _context.Mesas.FirstOrDefaultAsync(m => m.IdSucursal == sucursal.Id && m.Codigo == codigoMesa, ct);
                    if (mesaExistente == null)
                    {
                        var mesa = new Mesa
                        {
                            IdSucursal = sucursal.Id,
                            IdArea = area.Id,
                            Codigo = codigoMesa,
                            Asientos = asientos,
                            IdEstadoMesa = idEstadoDisponible,
                            IsActive = true,
                            CreatedBy = "onboarding"
                        };
                        _context.Mesas.Add(mesa);
                        mesasCreadas++;
                    }
                }
                await _context.SaveChangesAsync(ct);
            }

            // 7. Menú, Categorías y Precios
            var catImpuesto = await _context.CatImpuestos.FirstOrDefaultAsync(i => i.Descripcion.Contains("16") || i.Descripcion.ToLower().Contains("iva"), ct);
            int idImpuesto = catImpuesto?.Id ?? 1;

            var catMoneda = await _context.CatMonedas.FirstOrDefaultAsync(m => m.Descripcion.ToUpper().Contains("MXN") || m.Descripcion.ToLower().Contains("peso"), ct);
            int idMoneda = catMoneda?.Id ?? 1;

            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.IdSucursal == sucursal.Id, ct);
            if (menu == null)
            {
                menu = new Menu
                {
                    IdSucursal = sucursal.Id,
                    Nombre = string.IsNullOrWhiteSpace(dto.Menu.NombreMenu) ? "Menú Principal" : dto.Menu.NombreMenu,
                    IsActive = true,
                    CreatedBy = "onboarding"
                };
                _context.Menus.Add(menu);
                await _context.SaveChangesAsync(ct);
            }

            var catMap = new Dictionary<string, CategoriaMenu>(StringComparer.OrdinalIgnoreCase);
            var catsExistentes = await _context.CategoriaMenus.Where(c => c.IdMenu == menu.Id).ToListAsync(ct);
            foreach (var c in catsExistentes) catMap[c.Nombre ?? ""] = c;

            int ordenCat = 1;
            var categoriasNombres = dto.Menu.Categorias.Distinct().ToList();
            foreach (var prodDto in dto.Menu.Productos)
            {
                if (!string.IsNullOrWhiteSpace(prodDto.NombreCategoria) && !categoriasNombres.Contains(prodDto.NombreCategoria))
                {
                    categoriasNombres.Add(prodDto.NombreCategoria);
                }
            }
            if (categoriasNombres.Count == 0)
            {
                categoriasNombres.AddRange(new[] { "Entradas", "Platos Fuertes", "Bebidas", "Postres" });
            }

            foreach (var catNombre in categoriasNombres)
            {
                if (!catMap.ContainsKey(catNombre))
                {
                    var nuevaCat = new CategoriaMenu
                    {
                        IdMenu = menu.Id,
                        Nombre = catNombre,
                        Orden = ordenCat++,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    };
                    _context.CategoriaMenus.Add(nuevaCat);
                    await _context.SaveChangesAsync(ct);
                    catMap[catNombre] = nuevaCat;
                }
            }

            int prodsCreados = 0;
            var prodEntitiesMap = new Dictionary<string, Producto>(StringComparer.OrdinalIgnoreCase);

            foreach (var prodDto in dto.Menu.Productos)
            {
                int idCat = catMap.TryGetValue(prodDto.NombreCategoria, out var cVal)
                    ? cVal.Id
                    : catMap.Values.First().Id;

                int? idCatEstacion = null;
                if (!string.IsNullOrWhiteSpace(prodDto.EstacionCocina) && catEstacionesMap.TryGetValue(prodDto.EstacionCocina, out var cec))
                {
                    idCatEstacion = cec.Id;
                }

                var prod = await _context.Productos.FirstOrDefaultAsync(p => p.IdMenu == menu.Id && p.Nombre == prodDto.Nombre, ct);
                if (prod == null)
                {
                    prod = new Producto
                    {
                        IdMenu = menu.Id,
                        IdCategoria = idCat,
                        Nombre = prodDto.Nombre,
                        Descripcion = prodDto.Descripcion,
                        IdEstacionCocina = idCatEstacion,
                        Activo = true,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    };
                    _context.Productos.Add(prod);
                    await _context.SaveChangesAsync(ct);
                    prodsCreados++;
                }
                prodEntitiesMap[prod.Nombre!] = prod;

                var variante = await _context.VarianteProductos.FirstOrDefaultAsync(v => v.IdProducto == prod.Id, ct);
                if (variante == null)
                {
                    variante = new VarianteProducto
                    {
                        IdProducto = prod.Id,
                        Nombre = "Estándar",
                        Codigo = $"VAR-{prod.Id}",
                        EsDefault = true,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    };
                    _context.VarianteProductos.Add(variante);
                    await _context.SaveChangesAsync(ct);
                }

                var precio = await _context.Precios.FirstOrDefaultAsync(p => p.IdVariante == variante.Id, ct);
                if (precio == null)
                {
                    precio = new Precio
                    {
                        IdVariante = variante.Id,
                        Monto = prodDto.Precio,
                        Moneda = dto.DatosEmpresa.Moneda ?? "MXN",
                        IdImpuesto = idImpuesto,
                        IdMoneda = idMoneda,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    };
                    _context.Precios.Add(precio);
                    await _context.SaveChangesAsync(ct);
                }
                else
                {
                    precio.Monto = prodDto.Precio;
                    precio.IsActive = true;
                    await _context.SaveChangesAsync(ct);
                }
            }

            // 8. Modificadores
            if (dto.Menu.GruposModificador != null && dto.Menu.GruposModificador.Count > 0)
            {
                foreach (var grupoDto in dto.Menu.GruposModificador)
                {
                    if (!prodEntitiesMap.TryGetValue(grupoDto.NombreProducto, out var targetProd))
                        continue;

                    var grupo = await _context.GruposModificador
                        .FirstOrDefaultAsync(g => g.IdProducto == targetProd.Id && g.Nombre == grupoDto.NombreGrupo, ct);

                    if (grupo == null)
                    {
                        grupo = new GrupoModificador
                        {
                            IdProducto = targetProd.Id,
                            Nombre = grupoDto.NombreGrupo,
                            Obligatorio = grupoDto.Obligatorio,
                            MinSeleccion = grupoDto.MinSeleccion,
                            MaxSeleccion = grupoDto.MaxSeleccion > 0 ? grupoDto.MaxSeleccion : 1,
                            IsActive = true,
                            CreatedBy = "onboarding"
                        };
                        _context.GruposModificador.Add(grupo);
                        await _context.SaveChangesAsync(ct);
                    }

                    if (grupoDto.Opciones != null)
                    {
                        foreach (var opDto in grupoDto.Opciones)
                        {
                            var opc = await _context.OpcionesModificador
                                .FirstOrDefaultAsync(o => o.IdGrupo == grupo.Id && o.Nombre == opDto.Nombre, ct);

                            if (opc == null)
                            {
                                opc = new OpcionModificador
                                {
                                    IdGrupo = grupo.Id,
                                    Nombre = opDto.Nombre,
                                    PrecioExtra = opDto.PrecioExtra,
                                    EsDefault = opDto.EsDefault,
                                    IsActive = true,
                                    CreatedBy = "onboarding"
                                };
                                _context.OpcionesModificador.Add(opc);
                            }
                        }
                        await _context.SaveChangesAsync(ct);
                    }
                }
            }

            // 9. Personal Operativo con PIN
            var catCredPin = await _context.CatCredenciales.FirstOrDefaultAsync(c => c.Descripcion == "PIN", ct);
            if (catCredPin == null)
            {
                catCredPin = new CatCredencial { Descripcion = "PIN", IsActive = true, CreatedBy = "onboarding" };
                _context.CatCredenciales.Add(catCredPin);
                await _context.SaveChangesAsync(ct);
            }

            var rolMesero = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Mesero", ct);
            var rolManager = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Manager", ct);

            string slugEmpresa = Regex.Replace((empresa.Nombre ?? "empresa").ToLowerInvariant(), @"[^a-z0-9]", "");
            if (string.IsNullOrEmpty(slugEmpresa)) slugEmpresa = $"empresa{empresa.Id}";

            int usuariosCreados = 0;
            if (dto.Personal != null)
            {
                foreach (var personaDto in dto.Personal)
                {
                    if (string.IsNullOrWhiteSpace(personaDto.NombreCompleto) || string.IsNullOrWhiteSpace(personaDto.Pin))
                        continue;

                    string slugPersona = Regex.Replace(personaDto.NombreCompleto.ToLowerInvariant(), @"[^a-z0-9]", "");
                    string guidSuffix = Guid.NewGuid().ToString("N");
                    string correoGenerado = $"{slugPersona}.{guidSuffix[..4]}@{slugEmpresa}.mesafacil.local";

                    var usuario = new Usuario
                    {
                        IdEmpresa = empresa.Id,
                        IdSucursal = sucursal.Id,
                        NombreCompleto = personaDto.NombreCompleto,
                        Correo = correoGenerado,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    };
                    _context.Usuarios.Add(usuario);
                    await _context.SaveChangesAsync(ct);
                    usuariosCreados++;

                    int idRolAsignar = rolMesero?.Id ?? 3;
                    if (personaDto.Rol.Equals("Manager", StringComparison.OrdinalIgnoreCase) && rolManager != null)
                    {
                        idRolAsignar = rolManager.Id;
                    }

                    _context.UsuarioRoles.Add(new UsuarioRol
                    {
                        UsuarioId = usuario.Id,
                        IdRol = idRolAsignar,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    });

                    var (pinHash, pinSalt) = _hasher.HashPassword(personaDto.Pin);
                    _context.Credenciales.Add(new Credencial
                    {
                        IdUsuario = usuario.Id,
                        IdCredencial = catCredPin.Id,
                        Hash = pinHash,
                        Salt = pinSalt,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    });
                    await _context.SaveChangesAsync(ct);
                }
            }

            // 10. Actualizar PIN de Supervisor en Administrador
            if (!string.IsNullOrWhiteSpace(dto.PinSupervisorAdmin) && idUsuarioAdmin > 0)
            {
                var adminUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == idUsuarioAdmin, ct);
                if (adminUser != null)
                {
                    var (supHash, supSalt) = _hasher.HashPassword(dto.PinSupervisorAdmin);
                    adminUser.PinSupervisorHash = supHash;
                    adminUser.PinSupervisorSalt = supSalt;
                    adminUser.PinIntentosFallidos = 0;
                    adminUser.PinBloqueadoHasta = null;
                    adminUser.IdSucursal = sucursal.Id;
                    _context.Usuarios.Update(adminUser);
                    await _context.SaveChangesAsync(ct);
                }
            }

            // 11. Apertura de Turno Inicial
            int? turnoId = null;
            if (dto.AbrirTurnoInicial)
            {
                var turnoActivo = await _context.Turnos.FirstOrDefaultAsync(t => t.IdSucursal == sucursal.Id && t.Cierre == null, ct);
                if (turnoActivo != null)
                {
                    turnoId = turnoActivo.Id;
                }
                else
                {
                    int idUsuarioTurno = idUsuarioAdmin > 0
                        ? idUsuarioAdmin
                        : (await _context.Usuarios.FirstAsync(u => u.IdEmpresa == empresa.Id, ct)).Id;

                    var nuevoTurno = new Turno
                    {
                        IdSucursal = sucursal.Id,
                        IdUsuario = idUsuarioTurno,
                        Apertura = DateTime.UtcNow,
                        Cierre = null,
                        CajaInicial = dto.FondoCajaInicial >= 0 ? dto.FondoCajaInicial : 1000.00m,
                        IsActive = true,
                        CreatedBy = "onboarding"
                    };
                    _context.Turnos.Add(nuevoTurno);
                    await _context.SaveChangesAsync(ct);
                    turnoId = nuevoTurno.Id;
                }
            }

            await tx.CommitAsync(ct);

            response.Data = new ProvisionarRestauranteResponseDTO
            {
                EmpresaId = empresa.Id,
                SucursalId = sucursal.Id,
                MesasCreadas = mesasCreadas,
                ProductosCreados = prodsCreados,
                UsuariosCreados = usuariosCreados,
                TurnoId = turnoId,
                RutaRedirect = "/ventas/pos"
            };
            response.isSuccess = true;
            response.Message = "¡Restaurante aprovisionado con éxito! Todo listo para operar.";
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError("Error al aprovisionar restaurante en Onboarding: {0}", ex.ToString());
            response.isSuccess = false;
            response.Message = $"Error al aprovisionar el restaurante: {ex.Message}";
        }

        return response;
    }
}
