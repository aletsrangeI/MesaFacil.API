using Domain.Entities;
using DTO.Impresoras;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.Impresoras;

/// <summary>
/// Spec 023: motor de autodiagnóstico y de envío de ticket de prueba para impresoras
/// térmicas ESC/POS. Retorna directamente los DTOs de respuesta (sin envolver en
/// Common.Response&lt;T&gt;) porque el contrato REST de estos dos endpoints está fijado
/// exactamente por la sección 3 del spec.md.
/// </summary>
public interface IImpresoraDiagnosticService
{
    /// <summary>Retorna null si la impresora no existe.</summary>
    Task<DiagnosticoImpresoraResponseDTO?> DiagnosticarAsync(int idImpresora);

    /// <summary>Retorna null si la impresora no existe.</summary>
    Task<TestPrintResponseDTO?> EnviarTestPrintAsync(int idImpresora);
}

public class ImpresoraDiagnosticService : IImpresoraDiagnosticService
{
    private readonly ApplicationDbContext _context;

    public ImpresoraDiagnosticService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DiagnosticoImpresoraResponseDTO?> DiagnosticarAsync(int idImpresora)
    {
        var impresora = await _context.ConfiguracionesImpresora.FindAsync(idImpresora);
        if (impresora == null) return null;

        var dto = new DiagnosticoImpresoraResponseDTO
        {
            IdImpresora = impresora.Id,
            Nombre = impresora.Nombre ?? string.Empty,
            DireccionIp = impresora.DireccionIp,
            Puerto = impresora.Puerto
        };

        // USBLocal / NavegadorDialogo: no hay nada que sondear por red, se informa como
        // "no aplica" sin abrir ningún socket.
        if (impresora.TipoConexion != TipoConexionImpresora.RedLAN)
        {
            dto.Estado = "Ok";
            dto.MensajeDiagnostico = impresora.TipoConexion == TipoConexionImpresora.USBLocal
                ? "Impresora USB local: no aplica diagnóstico remoto para este tipo de conexión. Verifique el spooler de Windows y el cable USB."
                : "Impresora vía diálogo del navegador: no aplica diagnóstico remoto para este tipo de conexión.";
            dto.AccionSugerida = "No se requiere acción de red; use la impresión de prueba del sistema operativo o del navegador.";
            return dto;
        }

        if (string.IsNullOrWhiteSpace(impresora.DireccionIp))
        {
            dto.Estado = "NoAlcanzable";
            dto.MensajeDiagnostico = "La impresora no tiene una dirección IP configurada.";
            dto.AccionSugerida = "Edite la configuración de la impresora y capture su dirección IP en la red local.";
            return dto;
        }

        var cliente = new TcpThermalPrinterClient(impresora.DireccionIp, impresora.Puerto);
        var sondeo = await cliente.ConectarYConsultarEstadoAsync();

        if (!sondeo.Conectado)
        {
            dto.Estado = "NoAlcanzable";
            dto.MensajeDiagnostico = "La impresora no responde en la red local. Verifique que esté encendida y conectada al router.";
            dto.AccionSugerida = "Revise el cable de red (luces del puerto RJ45), la IP configurada y que la impresora esté encendida.";
            return dto;
        }

        dto.LatenciaMs = sondeo.LatenciaMs;

        // Mapeo de bits de estado ESC/POS (estándar Epson TM-T88 y compatibles):
        //  - DLE EOT 2 (EstadoTapaYPapel), bit 2 (0x04): tapa/cover abierta.
        //  - DLE EOT 3 (EstadoError), bits 2 (0x04, error recuperable/auto), 4 (0x10, error no
        //    recuperable) y 5 (0x20, error de auto-cutter): se agrupan como "error de hardware".
        //  - DLE EOT 4 (EstadoRolloPapel), bits 5 y 6 (0x20 / 0x40): sensor de fin de rollo
        //    activado ⇒ sin papel. (Los bits 2/3, 0x04/0x08, indicarían "papel por agotarse" —
        //    no distinguido en este contrato de respuesta, que solo expone SinPapel booleano).
        var tapaAbierta = sondeo.EstadoTapaYPapel.HasValue && (sondeo.EstadoTapaYPapel.Value & 0x04) != 0;
        var sinPapel = sondeo.EstadoRolloPapel.HasValue &&
                       ((sondeo.EstadoRolloPapel.Value & 0x20) != 0 || (sondeo.EstadoRolloPapel.Value & 0x40) != 0);
        var errorHardware = sondeo.EstadoError.HasValue &&
                             ((sondeo.EstadoError.Value & 0x04) != 0 ||
                              (sondeo.EstadoError.Value & 0x10) != 0 ||
                              (sondeo.EstadoError.Value & 0x20) != 0);

        dto.TapaAbierta = tapaAbierta;
        dto.SinPapel = sinPapel;

        if (errorHardware)
        {
            dto.Estado = "ErrorHardware";
            dto.MensajeDiagnostico = "La impresora reporta un error de hardware (posible falla de cabezal, temperatura o del mecanismo de corte).";
            dto.AccionSugerida = "Apague y encienda la impresora. Si el error persiste, contacte a soporte técnico.";
        }
        else if (tapaAbierta)
        {
            dto.Estado = "TapaAbierta";
            dto.MensajeDiagnostico = "La palanca/tapa de la impresora está abierta.";
            dto.AccionSugerida = "Cierre firmemente la tapa de la impresora hasta escuchar el clic de seguro.";
        }
        else if (sinPapel)
        {
            dto.Estado = "SinPapel";
            dto.MensajeDiagnostico = "La impresora está conectada a la red pero el rollo de papel térmico se ha agotado.";
            dto.AccionSugerida = $"Abra la palanca lateral, inserte un nuevo rollo térmico de {(int)impresora.AnchoPapel}mm con el papel saliendo por arriba y cierre firmemente la tapa.";
        }
        else
        {
            dto.Estado = "Ok";
            dto.MensajeDiagnostico = "La impresora está conectada y lista para imprimir.";
            dto.AccionSugerida = "Ninguna acción requerida.";
        }

        return dto;
    }

    public async Task<TestPrintResponseDTO?> EnviarTestPrintAsync(int idImpresora)
    {
        var impresora = await _context.ConfiguracionesImpresora.FindAsync(idImpresora);
        if (impresora == null) return null;

        if (impresora.TipoConexion != TipoConexionImpresora.RedLAN)
        {
            return new TestPrintResponseDTO
            {
                Exitoso = true,
                Mensaje = "Esta impresora usa una conexión local (USB o diálogo del navegador); envíe el ticket de prueba desde el spooler de Windows o el navegador."
            };
        }

        if (string.IsNullOrWhiteSpace(impresora.DireccionIp))
        {
            return new TestPrintResponseDTO
            {
                Exitoso = false,
                Mensaje = "La impresora no tiene una dirección IP configurada."
            };
        }

        try
        {
            using var ms = new MemoryStream();
            void Escribir(byte[] b) => ms.Write(b, 0, b.Length);

            // Secuencia real ESC/POS: inicializar -> encabezado -> leyenda -> avance de papel
            // -> corte (si Autocorte está activo). Texto en Encoding.ASCII (ver EscPosCommands).
            Escribir(EscPosCommands.Inicializar);
            Escribir(EscPosCommands.Texto("MesaFacil\n"));
            Escribir(EscPosCommands.Texto($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}\n"));
            Escribir(EscPosCommands.Texto("Impresora funcionando al 100% en MesaFacil\n"));
            Escribir(EscPosCommands.SaltoLinea(3));
            if (impresora.Autocorte)
            {
                Escribir(EscPosCommands.Corte());
            }

            var cliente = new TcpThermalPrinterClient(impresora.DireccionIp, impresora.Puerto);
            var enviado = await cliente.EnviarBytesAsync(ms.ToArray());

            return new TestPrintResponseDTO
            {
                Exitoso = enviado,
                Mensaje = enviado
                    ? "Ticket de prueba enviado exitosamente a la impresora."
                    : "No fue posible conectar con la impresora para enviar el ticket de prueba. Verifique la conexión de red."
            };
        }
        catch (Exception ex)
        {
            // Nunca debe tirar una excepción no controlada ni colgar el hilo del POS.
            return new TestPrintResponseDTO
            {
                Exitoso = false,
                Mensaje = "Ocurrió un error al enviar el ticket de prueba: " + ex.Message
            };
        }
    }
}
