using System.Text;

namespace UseCases.Impresoras;

/// <summary>
/// Spec 023: generación de secuencias de bytes ESC/POS estándar (compatibles con Epson
/// TM-T88 y la gran mayoría de impresoras térmicas usadas en México), tanto para el envío
/// de tickets de prueba como para la consulta de sensores físicos (DLE EOT n).
/// </summary>
public static class EscPosCommands
{
    /// <summary>ESC @ — Inicializa la impresora (limpia buffer, restaura formato por defecto).</summary>
    public static readonly byte[] Inicializar = { 0x1B, 0x40 };

    /// <summary>
    /// DLE EOT n — Consulta de estado en tiempo real (n = 1..4). La impresora responde con
    /// un único byte de estado. Esta es la secuencia real definida por el estándar ESC/POS
    /// (Epson TM-T88 y compatibles):
    ///   n=1: Estado general de la impresora (online/offline, cajón, recuperación).
    ///   n=2: Estado de la tapa/cover y del sensor de alimentación de papel.
    ///   n=3: Estado de errores (cabezal, temperatura, auto-cutter).
    ///   n=4: Estado de los sensores del rollo de papel (near-end / end).
    /// </summary>
    public static byte[] ConsultaEstado(int n) => new byte[] { 0x10, 0x04, (byte)n };

    /// <summary>GS V 66 0 — Corte total de papel (full cut). GS V 65 0 sería corte parcial.</summary>
    public static byte[] Corte(bool corteTotal = true) => new byte[] { 0x1D, 0x56, (byte)(corteTotal ? 66 : 65), 0x00 };

    /// <summary>
    /// ESC p m t1 t2 — Pulso de apertura de cajón de dinero (m=0 pin 2, m=1 pin 5;
    /// t1/t2 en unidades de 2ms, valores por defecto 25/250 según el pulso típico de 50ms/500ms).
    /// </summary>
    public static byte[] AperturaCajon(byte pin = 0, byte tiempoEncendido = 25, byte tiempoApagado = 250) =>
        new byte[] { 0x1B, 0x70, pin, tiempoEncendido, tiempoApagado };

    /// <summary>Salto(s) de línea (LF, 0x0A).</summary>
    public static byte[] SaltoLinea(int lineas = 1)
    {
        var bytes = new byte[Math.Max(1, lineas)];
        Array.Fill(bytes, (byte)0x0A);
        return bytes;
    }

    /// <summary>
    /// Convierte texto a bytes crudos. Se usa Encoding.ASCII (sin acentos) por máxima
    /// compatibilidad entre marcas/firmwares de impresoras térmicas sin depender de la
    /// tabla de códigos configurada en el equipo (CP437/CP850 varía por modelo y requeriría
    /// el paquete adicional System.Text.Encoding.CodePages); los mensajes del ticket de
    /// prueba se redactan sin acentos por este motivo.
    /// </summary>
    public static byte[] Texto(string texto) => Encoding.ASCII.GetBytes(texto);
}
