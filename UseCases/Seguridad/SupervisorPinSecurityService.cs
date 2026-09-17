using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Common;
using Domain.Entities;
using DTO.Seguridad;
using Interface.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence.Context;

namespace UseCases.Seguridad;

/// <summary>
/// Spec 024: Candado de Supervisor con PIN de 4 dígitos.
///
/// Roles de supervisor: reutiliza el esquema de roles ya existente en el proyecto (Rol.Nombre
/// "Admin" y "Manager", sembrados en DatabaseInitializer) como equivalentes a "Administrador" y
/// "Gerente" del spec — no se crea un rol nuevo.
///
/// Hashing: reutiliza IPasswordHasher (Pbkdf2PasswordHasher, ya usado para contraseñas y PIN de
/// login rápido) en vez de agregar BCrypt.Net-Next como dependencia nueva.
///
/// Rate-limiting (decisión documentada, ver tasks.md): el contador de intentos fallidos es POR
/// USUARIO SUPERVISOR. Como el PIN no identifica de antemano a qué supervisor pertenece, un
/// intento con un PIN que no matchea a NINGÚN supervisor válido de la sucursal incrementa el
/// contador de TODOS los supervisores candidatos (Admin/Manager activos, con PIN configurado, de
/// la misma Empresa que el Pedido) que en ese momento no estén ya bloqueados. El primero en llegar
/// a 3 intentos fallidos queda bloqueado 5 minutos y su contador se reinicia. Si un supervisor
/// concreto autentica correctamente, sólo se resetea su propio contador/bloqueo.
///
/// Token efímero: se construye manualmente con la misma sección de configuración "Jwt" (Issuer /
/// Audience / SigningKey) que WebApi.Modules.Authentication.JwtTokenService, para no invertir la
/// dirección de referencias entre proyectos (UseCases no puede depender de WebApi). Expira a los
/// 60 segundos y lleva los claims "accionProtegida" e "idPedidoDetalle" que el endpoint protegido
/// de borrado de PedidoDetalle valida antes de permitir la operación.
/// </summary>
public class SupervisorPinSecurityService : ISupervisorPinSecurityService
{
    private const int MaxIntentosFallidos = 3;
    private static readonly TimeSpan DuracionBloqueo = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan DuracionTokenAutorizacion = TimeSpan.FromSeconds(60);
    private static readonly string[] RolesSupervisor = { "Admin", "Manager" };
    private static readonly Regex PinRegex = new("^\\d{4}$", RegexOptions.Compiled);

    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly IConfiguration _configuration;

    public SupervisorPinSecurityService(ApplicationDbContext context, IPasswordHasher hasher, IConfiguration configuration)
    {
        _context = context;
        _hasher = hasher;
        _configuration = configuration;
    }

    public async Task<Response<bool>> ConfigurarPinAsync(ConfigurarPinRequestDTO request, int idUsuarioSolicitante, CancellationToken ct = default)
    {
        var response = new Response<bool>();

        if (!PinRegex.IsMatch(request.NuevoPin ?? string.Empty))
        {
            response.Message = "El PIN debe ser exactamente 4 dígitos numéricos.";
            return response;
        }

        var idUsuario = request.IdUsuario ?? idUsuarioSolicitante;

        var usuario = await _context.Usuarios
            .Include(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.Id == idUsuario, ct);

        if (usuario is null || !usuario.IsActive)
        {
            response.Message = "Usuario no encontrado o inactivo.";
            return response;
        }

        var esSupervisor = usuario.UsuarioRoles.Any(ur => RolesSupervisor.Contains(ur.Rol.Nombre));
        if (!esSupervisor)
        {
            response.Message = "Sólo usuarios con rol Gerente/Administrador pueden configurar un PIN de supervisor.";
            return response;
        }

        var (hash, salt) = _hasher.HashPassword(request.NuevoPin);
        usuario.PinSupervisorHash = hash;
        usuario.PinSupervisorSalt = salt;
        usuario.PinIntentosFallidos = 0;
        usuario.PinBloqueadoHasta = null;

        await _context.SaveChangesAsync(ct);

        response.Data = true;
        response.isSuccess = true;
        response.Message = "PIN de supervisor configurado correctamente.";
        return response;
    }

    public async Task<AutorizarSupervisorPinResponseDTO> AutorizarAsync(AutorizarSupervisorPinRequestDTO request, CancellationToken ct = default)
    {
        var respuesta = new AutorizarSupervisorPinResponseDTO();
        var ahora = DateTime.UtcNow;

        var pedido = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.IdPedido, ct);
        if (pedido is null)
        {
            respuesta.Mensaje = "Pedido no encontrado.";
            return respuesta;
        }

        var candidatos = await _context.Usuarios
            .Include(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .Where(u => u.IdEmpresa == pedido.IdEmpresa
                        && u.IsActive
                        && u.PinSupervisorHash != null
                        && u.UsuarioRoles.Any(ur => RolesSupervisor.Contains(ur.Rol.Nombre)))
            .ToListAsync(ct);

        if (candidatos.Count == 0)
        {
            respuesta.Mensaje = "No hay supervisores con PIN configurado en esta sucursal/empresa.";
            return respuesta;
        }

        var disponibles = candidatos.Where(c => !c.PinBloqueadoHasta.HasValue || c.PinBloqueadoHasta.Value <= ahora).ToList();

        if (disponibles.Count == 0)
        {
            var desbloqueoMasProximo = candidatos.Min(c => c.PinBloqueadoHasta) ?? ahora.Add(DuracionBloqueo);
            respuesta.Bloqueado = true;
            respuesta.BloqueadoHastaUtc = desbloqueoMasProximo;
            respuesta.Mensaje = $"PIN de supervisor bloqueado por intentos fallidos. Intente nuevamente después de {desbloqueoMasProximo:HH:mm:ss} UTC.";
            return respuesta;
        }

        var supervisorAutorizado = disponibles.FirstOrDefault(c => _hasher.Verify(request.Pin, c.PinSupervisorHash!, c.PinSupervisorSalt));

        if (supervisorAutorizado is null)
        {
            foreach (var candidato in disponibles)
            {
                candidato.PinIntentosFallidos++;
                if (candidato.PinIntentosFallidos >= MaxIntentosFallidos)
                {
                    candidato.PinBloqueadoHasta = ahora.Add(DuracionBloqueo);
                    candidato.PinIntentosFallidos = 0;

                    _context.EventosPedido.Add(new EventoPedido
                    {
                        IdPedido = request.IdPedido,
                        IdUsuarioSupervisor = candidato.Id,
                        TipoEvento = "PinSupervisorBloqueado",
                        Payload = JsonSerializer.Serialize(new { candidato.Id, BloqueadoHastaUtc = candidato.PinBloqueadoHasta }),
                        IsActive = true,
                        CreatedAt = ahora,
                        CreatedBy = "SupervisorPinSecurityService"
                    });
                }
            }

            await _context.SaveChangesAsync(ct);

            respuesta.Mensaje = "PIN de supervisor inválido.";
            respuesta.Bloqueado = disponibles.Any(c => c.PinBloqueadoHasta.HasValue && c.PinBloqueadoHasta.Value > ahora);
            return respuesta;
        }

        // Autorización exitosa: reset del contador/bloqueo propio y emisión del token efímero.
        supervisorAutorizado.PinIntentosFallidos = 0;
        supervisorAutorizado.PinBloqueadoHasta = null;

        _context.EventosPedido.Add(new EventoPedido
        {
            IdPedido = request.IdPedido,
            IdUsuarioSupervisor = supervisorAutorizado.Id,
            TipoEvento = "AutorizacionSupervisorPin",
            Payload = JsonSerializer.Serialize(new
            {
                request.AccionProtegida,
                IdPedidoDetalle = request.IdPedidoDetalle,
                request.Motivo
            }),
            IsActive = true,
            CreatedAt = ahora,
            CreatedBy = "SupervisorPinSecurityService"
        });

        await _context.SaveChangesAsync(ct);

        var rolPrincipal = supervisorAutorizado.UsuarioRoles
            .Select(ur => ur.Rol.Nombre)
            .FirstOrDefault(n => RolesSupervisor.Contains(n)) ?? "Supervisor";

        respuesta.Autorizado = true;
        respuesta.SupervisorId = supervisorAutorizado.Id;
        respuesta.NombreSupervisor = $"{supervisorAutorizado.NombreCompleto} ({rolPrincipal})";
        respuesta.TokenAutorizacion = GenerarTokenAutorizacion(supervisorAutorizado.Id, request.AccionProtegida, request.IdPedido, request.IdPedidoDetalle, ahora);
        respuesta.Mensaje = "Autorizado.";
        return respuesta;
    }

    public bool ValidarTokenAutorizacion(string? token, string accionEsperada, Guid idPedidoDetalle, out int? idUsuarioSupervisor)
    {
        idUsuarioSupervisor = null;
        if (string.IsNullOrWhiteSpace(token)) return false;

        var (issuer, audience, signingKeyBytes) = LeerConfigJwt();

        // MapInboundClaims=false: evita que JwtSecurityTokenHandler renombre "sub" al URI largo
        // de ClaimTypes.NameIdentifier (su comportamiento heredado por defecto), para poder leer
        // el id del supervisor con el mismo nombre de claim corto ("sub") con el que se emitió.
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var parametros = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5)
        };

        ClaimsPrincipal principal;
        try
        {
            principal = handler.ValidateToken(token, parametros, out _);
        }
        catch
        {
            return false;
        }

        var typ = principal.FindFirst("typ")?.Value;
        var accionClaim = principal.FindFirst("accionProtegida")?.Value;
        var idPedidoDetalleClaim = principal.FindFirst("idPedidoDetalle")?.Value;
        var subClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? principal.FindFirst("sub")?.Value;

        if (typ != "supervisor-auth") return false;
        if (!string.Equals(accionClaim, accionEsperada, StringComparison.OrdinalIgnoreCase)) return false;
        if (!string.Equals(idPedidoDetalleClaim, idPedidoDetalle.ToString(), StringComparison.OrdinalIgnoreCase)) return false;

        if (int.TryParse(subClaim, out var supervisorId))
            idUsuarioSupervisor = supervisorId;

        return true;
    }

    public bool ValidarTokenDescuento(string? token, Guid idPedido, out int? idUsuarioSupervisor)
    {
        idUsuarioSupervisor = null;
        if (string.IsNullOrWhiteSpace(token)) return false;

        var (issuer, audience, signingKeyBytes) = LeerConfigJwt();

        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var parametros = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5)
        };

        ClaimsPrincipal principal;
        try
        {
            principal = handler.ValidateToken(token, parametros, out _);
        }
        catch
        {
            return false;
        }

        var typ = principal.FindFirst("typ")?.Value;
        var accionClaim = principal.FindFirst("accionProtegida")?.Value;
        var idPedidoClaim = principal.FindFirst("idPedido")?.Value;
        var subClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? principal.FindFirst("sub")?.Value;

        if (typ != "supervisor-auth") return false;
        if (!string.Equals(accionClaim, "DescuentoExcesivo", StringComparison.OrdinalIgnoreCase)) return false;
        if (!string.Equals(idPedidoClaim, idPedido.ToString(), StringComparison.OrdinalIgnoreCase)) return false;

        if (int.TryParse(subClaim, out var supervisorId))
            idUsuarioSupervisor = supervisorId;

        return true;
    }

    private string GenerarTokenAutorizacion(int idSupervisor, string accionProtegida, Guid idPedido, Guid? idPedidoDetalle, DateTime nowUtc)
    {
        var (issuer, audience, signingKeyBytes) = LeerConfigJwt();
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKeyBytes), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, idSupervisor.ToString()),
            new("typ", "supervisor-auth"),
            new("accionProtegida", accionProtegida),
            new("idPedido", idPedido.ToString()),
            new("idPedidoDetalle", (idPedidoDetalle ?? Guid.Empty).ToString())
        };

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: nowUtc,
            expires: nowUtc.Add(DuracionTokenAutorizacion),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private (string Issuer, string Audience, byte[] SigningKeyBytes) LeerConfigJwt()
    {
        var issuer = _configuration["Jwt:Issuer"] ?? string.Empty;
        var audience = _configuration["Jwt:Audience"] ?? string.Empty;
        var signingKey = _configuration["Jwt:SigningKey"] ?? string.Empty;
        return (issuer, audience, Encoding.UTF8.GetBytes(signingKey));
    }
}
