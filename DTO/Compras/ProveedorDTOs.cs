using System.Text.RegularExpressions;

namespace DTO.Compras;

public class ProveedorDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public string RFC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Contacto { get; set; }
    public string? Direccion { get; set; }
    public string? RegimenFiscal { get; set; }
    public int DiasCredito { get; set; }
    public string? Banco { get; set; }
    public string? CuentaBancaria { get; set; }
    public bool IsActive { get; set; }
    public int TotalCompras { get; set; }
    public decimal MontoTotalComprado { get; set; }
}

public class CrearProveedorDTO
{
    public int IdEmpresa { get; set; } = 1;
    public string RFC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Contacto { get; set; }
    public string? Direccion { get; set; }
    public string? RegimenFiscal { get; set; }
    public int DiasCredito { get; set; } = 0;
    public string? Banco { get; set; }
    public string? CuentaBancaria { get; set; }
}

public class ActualizarProveedorDTO : CrearProveedorDTO
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ProveedorSimpleDTO
{
    public int Id { get; set; }
    public string RFC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    public int DiasCredito { get; set; }
}

public static class RfcHelper
{
    // Regex estándar SAT México:
    // Moral: 3 letras, 6 dígitos (AAMMDD), 3 caracteres alfanuméricos homoclave (12 caracteres)
    // Física: 4 letras, 6 dígitos (AAMMDD), 3 caracteres alfanuméricos homoclave (13 caracteres)
    private static readonly Regex RfcRegex = new(
        @"^[A-Z&Ñ]{3,4}\d{6}[A-V1-9][A-Z1-9][0-9A]$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool EsRfcValido(string? rfc, out string error)
    {
        error = string.Empty;
        if (string.IsNullOrWhiteSpace(rfc))
        {
            error = "El RFC es obligatorio.";
            return false;
        }

        rfc = rfc.Trim().ToUpperInvariant();
        if (rfc.Length != 12 && rfc.Length != 13)
        {
            error = "El RFC debe tener exactamente 12 caracteres (persona moral) o 13 caracteres (persona física).";
            return false;
        }

        // Caso especial SAT para ventas al público en general o extranjeros
        if (rfc == "XAXX010101000" || rfc == "XEXX010101000")
            return true;

        if (!RfcRegex.IsMatch(rfc))
        {
            error = "El formato del RFC no es válido según el estándar del SAT (3 o 4 letras + fecha AAMMDD + homoclave de 3 caracteres).";
            return false;
        }

        return true;
    }
}
