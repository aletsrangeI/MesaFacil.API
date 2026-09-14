using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Spec 019 §4.5 — Protocolo Zero-Config y Emparejamiento por QR.
/// Expone las interfaces de red IPv4 privadas (LAN) del Edge Node para que
/// el modal de emparejamiento en MesaFacil.UI pueda construir el QR con la
/// IP local real (192.168.x.x, 10.x.x.x, 172.16-31.x.x) en lugar de la
/// URL de la nube (window.location.origin), permitiendo comunicación directa
/// dentro de la sucursal cuando el enlace WAN se interrumpe.
/// </summary>
[ApiController]
[Route("api/network")]
[AllowAnonymous]
public class NetworkController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<NetworkController> _logger;

    public NetworkController(IConfiguration config, ILogger<NetworkController> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas las IPs IPv4 privadas (LAN) de la máquina donde corre el Edge Node,
    /// junto con el puerto en el que sirve el frontend (UI) y el backend (API),
    /// para que el QR de emparejamiento apunte a la red local del restaurante.
    ///
    /// GET /api/network/lan-ips
    /// Response: { ips: string[], preferred: string, uiPort: int, apiPort: int }
    /// </summary>
    [HttpGet("lan-ips")]
    public IActionResult GetLanIps()
    {
        try
        {
            var lanIps = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(ni =>
                    ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
                .Where(ua =>
                    ua.Address.AddressFamily == AddressFamily.InterNetwork &&
                    IsPrivateLanAddress(ua.Address))
                .Select(ua => ua.Address.ToString())
                .Distinct()
                .OrderBy(ip => ip)
                .ToList();

            var apiPort = HttpContext.Connection.LocalPort;
            var uiPort = _config.GetValue<int?>("Pairing:UiPort") ?? 5173;

            _logger.LogInformation("Spec019 LAN-IPs: [{Ips}], API:{ApiPort}, UI:{UiPort}",
                string.Join(", ", lanIps), apiPort, uiPort);

            return Ok(new
            {
                ips = lanIps,
                apiPort,
                uiPort,
                preferred = lanIps.FirstOrDefault(ip => ip.StartsWith("192.168."))
                    ?? lanIps.FirstOrDefault(ip => ip.StartsWith("10."))
                    ?? lanIps.FirstOrDefault()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al detectar IPs LAN del Edge Node.");
            return StatusCode(500, new { error = "No se pudo detectar la IP local del servidor." });
        }
    }

    /// <summary>
    /// Determina si una IPv4 cae en rangos privados LAN:
    ///   10.0.0.0/8  |  172.16.0.0/12  |  192.168.0.0/16
    /// Excluye link-local (169.254.x.x) y Tailscale (100.x.x.x).
    /// </summary>
    private static bool IsPrivateLanAddress(IPAddress addr)
    {
        var b = addr.GetAddressBytes();
        if (b[0] == 10) return true;
        if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;
        if (b[0] == 192 && b[1] == 168) return true;
        return false;
    }
}
