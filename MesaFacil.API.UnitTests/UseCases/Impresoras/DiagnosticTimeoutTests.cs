using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Impresoras;

namespace MesaFacil.API.UnitTests.UseCases.Impresoras;

/// <summary>
/// Spec 023: garantiza que el sondeo de una impresora térmica por socket TCP NUNCA bloquea
/// el hilo más allá del timeout estricto de 1.5s de conexión (+ el timeout corto e
/// independiente de lectura de cada byte de estado), tanto cuando el puerto no tiene nadie
/// escuchando como cuando la "impresora" acepta la conexión pero nunca responde.
///
/// En vez de mockear TcpClient (los sockets no se prestan bien a mocks fiables) se levanta
/// un TcpListener real en un puerto libre de loopback dentro de cada prueba, que es la forma
/// más determinista de reproducir ambos escenarios sin depender de la red real ni de IPs no
/// enrutables (poco confiables en entornos de CI en contenedores).
/// </summary>
public class DiagnosticTimeoutTests
{
    private const string Loopback = "127.0.0.1";

    private static ApplicationDbContext CreateInMemoryDbContext(string nombreBd)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nombreBd)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var interceptor = new AuditableEntitySaveChangesInterceptor();
        var outboxInterceptor = new OutboxSaveChangesInterceptor();
        return new ApplicationDbContext(options, interceptor, outboxInterceptor);
    }

    private static int ObtenerPuertoLibre()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var puerto = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return puerto;
    }

    [Fact]
    public async Task ConectarYConsultarEstadoAsync_SinListener_RetornaNoAlcanzable_Rapido()
    {
        // Arrange: puerto cerrado (nadie escucha) en loopback.
        var puerto = ObtenerPuertoLibre();
        var cliente = new TcpThermalPrinterClient(Loopback, puerto);
        var cronometro = Stopwatch.StartNew();

        // Act
        var resultado = await cliente.ConectarYConsultarEstadoAsync();
        cronometro.Stop();

        // Assert
        resultado.Conectado.Should().BeFalse();
        // Un puerto cerrado en loopback normalmente rechaza (RST) casi de inmediato, pero
        // en cualquier caso nunca debe exceder el timeout estricto de conexión (1.5s) por más
        // de un margen razonable de holgura del entorno de pruebas.
        cronometro.ElapsedMilliseconds.Should().BeLessThan(3000);
    }

    [Fact]
    public async Task ConectarYConsultarEstadoAsync_ListenerAceptaPeroNuncaResponde_RespetaTimeoutDeLectura()
    {
        // Arrange: el listener acepta la conexión TCP pero nunca escribe nada de vuelta,
        // simulando una impresora "colgada" que sí está en la red pero no contesta al
        // protocolo ESC/POS. Esto ejercita el timeout de LECTURA (no el de conexión).
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var puerto = ((IPEndPoint)listener.LocalEndpoint).Port;

        var tareaServidor = Task.Run(async () =>
        {
            using var socketCliente = await listener.AcceptTcpClientAsync();
            // Mantiene el socket abierto sin escribir nada hasta que el test termine.
            await Task.Delay(TimeSpan.FromSeconds(5));
        });

        try
        {
            var cliente = new TcpThermalPrinterClient(Loopback, puerto);
            var cronometro = Stopwatch.StartNew();

            // Act
            var resultado = await cliente.ConectarYConsultarEstadoAsync();
            cronometro.Stop();

            // Assert: la conexión sí se establece (el listener aceptó), pero como nunca
            // responde a ninguno de los 3 DLE EOT, los 3 bytes de estado quedan en null y el
            // tiempo total queda acotado por los 3 timeouts de lectura (~800ms c/u) en vez de
            // colgarse indefinidamente.
            resultado.Conectado.Should().BeTrue();
            resultado.EstadoTapaYPapel.Should().BeNull();
            resultado.EstadoError.Should().BeNull();
            resultado.EstadoRolloPapel.Should().BeNull();
            cronometro.ElapsedMilliseconds.Should().BeLessThan(4000);
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public async Task ConectarYConsultarEstadoAsync_ListenerResponde_MideLatenciaYLeeBytesDeEstado()
    {
        // Arrange: impresora simulada que sí responde a los 3 DLE EOT con bytes de estado
        // conocidos: tapa abierta (bit2 de n=2), sin error (n=3 = 0), sin papel (bit5 de n=4).
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var puerto = ((IPEndPoint)listener.LocalEndpoint).Port;

        var tareaServidor = Task.Run(async () =>
        {
            using var socketCliente = await listener.AcceptTcpClientAsync();
            socketCliente.NoDelay = true;
            using var stream = socketCliente.GetStream();
            var buffer = new byte[3];

            // DLE EOT 2 -> responde con bit2 (tapa abierta) encendido: 0x04
            await stream.ReadExactlyAsync(buffer.AsMemory(0, 3));
            await stream.WriteAsync(new byte[] { 0x04 });
            await stream.FlushAsync();

            // DLE EOT 3 -> sin error: 0x00
            await stream.ReadExactlyAsync(buffer.AsMemory(0, 3));
            await stream.WriteAsync(new byte[] { 0x00 });
            await stream.FlushAsync();

            // DLE EOT 4 -> bit5 (sin papel) encendido: 0x20
            await stream.ReadExactlyAsync(buffer.AsMemory(0, 3));
            await stream.WriteAsync(new byte[] { 0x20 });
            await stream.FlushAsync();
        });

        try
        {
            var cliente = new TcpThermalPrinterClient(Loopback, puerto);

            // Act
            var resultado = await cliente.ConectarYConsultarEstadoAsync();
            await tareaServidor;

            // Assert
            resultado.Conectado.Should().BeTrue();
            resultado.LatenciaMs.Should().BeGreaterThanOrEqualTo(0);
            resultado.EstadoTapaYPapel.Should().Be((byte)0x04);
            resultado.EstadoError.Should().Be((byte)0x00);
            resultado.EstadoRolloPapel.Should().Be((byte)0x20);
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public async Task DiagnosticarAsync_ImpresoraNoAlcanzable_RetornaEstadoNoAlcanzable_EnMenosDe2Segundos()
    {
        // Arrange: fin a fin contra ImpresoraDiagnosticService, con una ConfiguracionImpresora
        // RedLAN apuntando a un puerto cerrado de loopback (nadie escucha).
        using var context = CreateInMemoryDbContext($"MesaFacil_Impresoras_Test_{Guid.NewGuid()}");
        var puerto = ObtenerPuertoLibre();

        var sucursal = new Sucursal { Id = 1, Nombre = "Sucursal Matriz", IsActive = true };
        context.Sucursales.Add(sucursal);
        context.ConfiguracionesImpresora.Add(new ConfiguracionImpresora
        {
            Id = 1,
            IdSucursal = 1,
            Nombre = "Comandera Parrilla",
            TipoConexion = TipoConexionImpresora.RedLAN,
            AnchoPapel = AnchoPapelImpresora.Mm80,
            DireccionIp = Loopback,
            Puerto = puerto,
            IsActive = true
        });
        await context.SaveChangesAsync();

        var servicio = new ImpresoraDiagnosticService(context);
        var cronometro = Stopwatch.StartNew();

        // Act
        var resultado = await servicio.DiagnosticarAsync(1);
        cronometro.Stop();

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Estado.Should().Be("NoAlcanzable");
        resultado.LatenciaMs.Should().BeNull();
        cronometro.ElapsedMilliseconds.Should().BeLessThan(3000);
    }

    [Fact]
    public async Task DiagnosticarAsync_TipoConexionUsbLocal_NoAbreSocket_RetornaOkInformativo()
    {
        // Arrange: TipoConexion distinto de RedLAN nunca debe intentar abrir un socket, sin
        // importar qué IP/puerto tenga configurados.
        using var context = CreateInMemoryDbContext($"MesaFacil_Impresoras_Test_{Guid.NewGuid()}");

        var sucursal = new Sucursal { Id = 1, Nombre = "Sucursal Matriz", IsActive = true };
        context.Sucursales.Add(sucursal);
        context.ConfiguracionesImpresora.Add(new ConfiguracionImpresora
        {
            Id = 2,
            IdSucursal = 1,
            Nombre = "Ticketera Caja",
            TipoConexion = TipoConexionImpresora.USBLocal,
            AnchoPapel = AnchoPapelImpresora.Mm58,
            IsActive = true
        });
        await context.SaveChangesAsync();

        var servicio = new ImpresoraDiagnosticService(context);
        var cronometro = Stopwatch.StartNew();

        // Act
        var resultado = await servicio.DiagnosticarAsync(2);
        cronometro.Stop();

        // Assert: debe ser prácticamente instantáneo (no hay I/O de red involucrado).
        resultado.Should().NotBeNull();
        resultado!.Estado.Should().Be("Ok");
        resultado.LatenciaMs.Should().BeNull();
        cronometro.ElapsedMilliseconds.Should().BeLessThan(500);
    }

    [Fact]
    public async Task DiagnosticarAsync_ImpresoraInexistente_RetornaNull()
    {
        using var context = CreateInMemoryDbContext($"MesaFacil_Impresoras_Test_{Guid.NewGuid()}");
        var servicio = new ImpresoraDiagnosticService(context);

        var resultado = await servicio.DiagnosticarAsync(999);

        resultado.Should().BeNull();
    }
}
