namespace UseCases.Inventario;

public static class ConversionUnidadesHelper
{
    /// <summary>
    /// Convierte una cantidad desde la unidad de medida origen hacia la unidad de medida destino.
    /// Si existe un factor explícito en la base de datos o si son de masa/volumen estándar, aplica el factor multiplicador.
    /// Retorna la cantidad convertida a la unidad destino.
    /// </summary>
    public static decimal ConvertirCantidad(
        decimal cantidad,
        string codigoOrigen,
        string codigoDestino,
        decimal? factorExplicitBd = null)
    {
        if (cantidad == 0) return 0m;
        if (string.IsNullOrWhiteSpace(codigoOrigen) || string.IsNullOrWhiteSpace(codigoDestino))
            return cantidad;

        var orig = codigoOrigen.Trim().ToUpperInvariant();
        var dest = codigoDestino.Trim().ToUpperInvariant();

        if (orig == dest) return cantidad;

        if (factorExplicitBd.HasValue && factorExplicitBd.Value > 0)
        {
            return Math.Round(cantidad * factorExplicitBd.Value, 6);
        }

        // Conversiones de Masa
        if (EsMasa(orig) && EsMasa(dest))
        {
            decimal enGramos = A_Gramos(cantidad, orig);
            return Math.Round(De_Gramos(enGramos, dest), 6);
        }

        // Conversiones de Volumen
        if (EsVolumen(orig) && EsVolumen(dest))
        {
            decimal enMililitros = A_Mililitros(cantidad, orig);
            return Math.Round(De_Mililitros(enMililitros, dest), 6);
        }

        // Conversiones Volumen -> Masa (Densidad culinaria estándar: 1 ml = 1 g)
        if (EsVolumen(orig) && EsMasa(dest))
        {
            decimal enMililitros = A_Mililitros(cantidad, orig);
            decimal enGramos = enMililitros;
            return Math.Round(De_Gramos(enGramos, dest), 6);
        }

        // Conversiones Masa -> Volumen (Densidad culinaria estándar: 1 g = 1 ml)
        if (EsMasa(orig) && EsVolumen(dest))
        {
            decimal enGramos = A_Gramos(cantidad, orig);
            decimal enMililitros = enGramos;
            return Math.Round(De_Mililitros(enMililitros, dest), 6);
        }

        // Conteo
        if (orig == "DOC" && dest == "PZA") return cantidad * 12m;
        if (orig == "PZA" && dest == "DOC") return Math.Round(cantidad / 12m, 6);

        // Sin factor conocido, retornar cantidad nominal
        return cantidad;
    }

    private static decimal A_Gramos(decimal cantidad, string codigo) => codigo switch
    {
        "KG" => cantidad * 1000m,
        "G" => cantidad,
        "MG" => cantidad / 1000m,
        "LB" => cantidad * 453.59237m,
        "OZ" => cantidad * 28.349523m,
        _ => cantidad
    };

    private static decimal De_Gramos(decimal gramos, string codigo) => codigo switch
    {
        "KG" => gramos / 1000m,
        "G" => gramos,
        "MG" => gramos * 1000m,
        "LB" => gramos / 453.59237m,
        "OZ" => gramos / 28.349523m,
        _ => gramos
    };

    private static decimal A_Mililitros(decimal cantidad, string codigo) => codigo switch
    {
        "L" => cantidad * 1000m,
        "ML" => cantidad,
        "CC" => cantidad,
        "OZ_FL" or "FL_OZ" => cantidad * 29.5735m,
        "GAL" => cantidad * 3785.41m,
        _ => cantidad
    };

    private static decimal De_Mililitros(decimal ml, string codigo) => codigo switch
    {
        "L" => ml / 1000m,
        "ML" => ml,
        "CC" => ml,
        "OZ_FL" or "FL_OZ" => ml / 29.5735m,
        "GAL" => ml / 3785.41m,
        _ => ml
    };

    public static bool EsMasa(string codigo)
    {
        return codigo is "KG" or "G" or "MG" or "LB" or "OZ";
    }

    public static bool EsVolumen(string codigo)
    {
        return codigo is "L" or "ML" or "CC" or "OZ_FL" or "FL_OZ" or "GAL";
    }
}
