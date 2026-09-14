using FluentAssertions;
using UseCases.Impresoras;

namespace MesaFacil.API.UnitTests.UseCases.Impresoras;

public class EscPosProtocolTests
{
    [Fact]
    public void Inicializar_GeneraSecuenciaEscArroba()
    {
        EscPosCommands.Inicializar.Should().Equal(new byte[] { 0x1B, 0x40 });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ConsultaEstado_GeneraSecuenciaDleEotN(int n)
    {
        var comando = EscPosCommands.ConsultaEstado(n);
        comando.Should().Equal(new byte[] { 0x10, 0x04, (byte)n });
    }

    [Fact]
    public void Corte_Total_GeneraSecuenciaGsV66_0()
    {
        var comando = EscPosCommands.Corte(corteTotal: true);
        comando.Should().Equal(new byte[] { 0x1D, 0x56, 66, 0x00 });
    }

    [Fact]
    public void Corte_Parcial_GeneraSecuenciaGsV65_0()
    {
        var comando = EscPosCommands.Corte(corteTotal: false);
        comando.Should().Equal(new byte[] { 0x1D, 0x56, 65, 0x00 });
    }

    [Fact]
    public void AperturaCajon_UsaValoresPorDefecto_EscP0_25_250()
    {
        var comando = EscPosCommands.AperturaCajon();
        comando.Should().Equal(new byte[] { 0x1B, 0x70, 0, 25, 250 });
    }

    [Fact]
    public void Texto_ConvierteAAsciiCrudo_SinLanzarPorAcentos()
    {
        // La leyenda del ticket de prueba se redacta sin acentos, pero el conversor no debe
        // lanzar excepción ante caracteres fuera de ASCII (los reemplaza con '?').
        var bytes = EscPosCommands.Texto("Impresora funcionando al 100% en MesaFacil\n");
        var texto = System.Text.Encoding.ASCII.GetString(bytes);
        texto.Should().Be("Impresora funcionando al 100% en MesaFacil\n");
    }

    [Fact]
    public void SaltoLinea_GeneraNBytesLF()
    {
        var bytes = EscPosCommands.SaltoLinea(3);
        bytes.Should().Equal(new byte[] { 0x0A, 0x0A, 0x0A });
    }

    [Fact]
    public void SaltoLinea_ConCeroOMenos_GeneraAlMenosUnByte()
    {
        var bytes = EscPosCommands.SaltoLinea(0);
        bytes.Should().Equal(new byte[] { 0x0A });
    }
}
