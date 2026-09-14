using System.Diagnostics;
using System.Net.Sockets;

namespace UseCases.Impresoras;

/// <summary>
/// Spec 023: resultado crudo de sondear una impresora térmica ESC/POS vía socket TCP
/// (sin interpretar aún el significado de negocio de los bits de estado).
/// </summary>
public class ResultadoSondeoImpresora
{
    public bool Conectado { get; set; }
    public int LatenciaMs { get; set; }

    /// <summary>Byte de respuesta a DLE EOT 2 (tapa/alimentación de papel), o null si no se pudo leer.</summary>
    public byte? EstadoTapaYPapel { get; set; }

    /// <summary>Byte de respuesta a DLE EOT 3 (errores de cabezal/temperatura/cutter), o null si no se pudo leer.</summary>
    public byte? EstadoError { get; set; }

    /// <summary>Byte de respuesta a DLE EOT 4 (sensores del rollo de papel), o null si no se pudo leer.</summary>
    public byte? EstadoRolloPapel { get; set; }
}

/// <summary>
/// Spec 023: cliente TCP crudo hacia una impresora térmica ESC/POS (puerto 9100 típico).
/// Implementa un timeout ESTRICTO de 1.5s para la conexión (vía CancellationTokenSource +
/// TcpClient.ConnectAsync(host, port, CancellationToken), soportado nativamente desde .NET 6+
/// sin necesidad de Task.Delay/Task.WhenAny "carreras" que dejarían el socket huérfano) y un
/// timeout corto e independiente por cada lectura de byte de estado, de forma que una
/// impresora que acepta la conexión pero nunca contesta (cable "medio" desconectado, firmware
/// colgado) tampoco pueda bloquear el hilo más allá de lo esperado.
/// </summary>
public class TcpThermalPrinterClient
{
    public static readonly TimeSpan TimeoutConexion = TimeSpan.FromMilliseconds(1500);
    private static readonly TimeSpan TimeoutLecturaEstado = TimeSpan.FromMilliseconds(800);

    private readonly string _ip;
    private readonly int _puerto;

    public TcpThermalPrinterClient(string ip, int puerto)
    {
        _ip = ip;
        _puerto = puerto;
    }

    /// <summary>
    /// Abre el socket (timeout de 1.5s) y, si conecta, envía DLE EOT 2/3/4 para leer los
    /// tres bytes de estado relevantes para el autodiagnóstico. Nunca lanza excepciones:
    /// cualquier falla de red se refleja en Conectado = false.
    /// </summary>
    public async Task<ResultadoSondeoImpresora> ConectarYConsultarEstadoAsync()
    {
        var resultado = new ResultadoSondeoImpresora();
        var cronometro = Stopwatch.StartNew();

        using var cts = new CancellationTokenSource(TimeoutConexion);
        TcpClient? cliente = null;
        try
        {
            cliente = new TcpClient();
            await cliente.ConnectAsync(_ip, _puerto, cts.Token);
            cronometro.Stop();

            resultado.Conectado = true;
            resultado.LatenciaMs = (int)cronometro.ElapsedMilliseconds;

            using var stream = cliente.GetStream();
            resultado.EstadoTapaYPapel = await ConsultarByteEstadoAsync(stream, 2);
            resultado.EstadoError = await ConsultarByteEstadoAsync(stream, 3);
            resultado.EstadoRolloPapel = await ConsultarByteEstadoAsync(stream, 4);
        }
        catch (OperationCanceledException)
        {
            resultado.Conectado = false;
        }
        catch (SocketException)
        {
            resultado.Conectado = false;
        }
        catch (IOException)
        {
            resultado.Conectado = false;
        }
        finally
        {
            cliente?.Dispose();
        }

        return resultado;
    }

    /// <summary>
    /// Envía una secuencia de bytes cruda (ticket de prueba, corte, apertura de cajón, etc.)
    /// reutilizando el mismo timeout estricto de conexión. Retorna false ante cualquier falla
    /// de red controlada, nunca lanza.
    /// </summary>
    public async Task<bool> EnviarBytesAsync(byte[] payload)
    {
        using var cts = new CancellationTokenSource(TimeoutConexion);
        try
        {
            using var cliente = new TcpClient();
            await cliente.ConnectAsync(_ip, _puerto, cts.Token);
            using var stream = cliente.GetStream();
            await stream.WriteAsync(payload, cts.Token);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (SocketException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static async Task<byte?> ConsultarByteEstadoAsync(NetworkStream stream, int n)
    {
        using var readCts = new CancellationTokenSource(TimeoutLecturaEstado);
        try
        {
            var comando = EscPosCommands.ConsultaEstado(n);
            await stream.WriteAsync(comando, readCts.Token);

            var buffer = new byte[1];
            var leidos = await stream.ReadAsync(buffer, readCts.Token);
            return leidos > 0 ? buffer[0] : null;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
    }
}
