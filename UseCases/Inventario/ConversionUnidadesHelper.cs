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
            decimal enGramos = orig switch
            {
                "KG" => cantidad * 1000m,
                "G" => cantidad,
                "MG" => cantidad / 1000m,
                "LB" => cantidad * 453.59237m,
                "OZ" => cantidad * 28.349523m,
                _ => cantidad
            };

            decimal resultado = dest switch
            {
                "KG" => enGramos / 1000m,
                "G" => enGramos,
                "MG" => enGramos * 1000m,
                "LB" => enGramos / 453.59237m,
                "OZ" => enGramos / 28.349523m,
                _ => enGramos
            };

            return Math.Round(resultado, 6);
        }

        // Conversiones de Volumen
        if (EsVolumen(orig) && EsVolumen(dest))
        {
            decimal enMililitros = orig switch
            {
                "L" => cantidad * 1000m,
                "ML" => cantidad,
                "CC" => cantidad,
                "OZ_FL" or "FL_OZ" => cantidad * 29.5735m,
                "GAL" => cantidad * 3785.41m,
                _ => cantidad
            };

            decimal resultado = dest switch
            {
                "L" => enMililitros / 1000m,
                "ML" => enMililitros,
                "CC" => enMililitros,
                "OZ_FL" or "FL_OZ" => enMililitros / 29.5735m,
                "GAL" => enMililitros / 3785.41m,
                _ => enMililitros
            };

            return Math.Round(resultado, 6);
        }

        // Conteo
        if (orig == "DOC" && dest == "PZA") return cantidad * 12m;
        if (orig == "PZA" && dest == "DOC") return Math.Round(cantidad / 12m, 6);

        // Sin factor conocido, retornar cantidad nominal
        return cantidad;
    }

    public static bool EsMasa(string codigo)
    {
        return codigo is "KG" or "G" or "MG" or "LB" or "OZ";
    }

    public static bool EsVolumen(string codigo)
    {
        return codigo is "L" or "ML" or "CC" or "OZ_FL" or "FL_OZ" or "GAL";
    }
}
