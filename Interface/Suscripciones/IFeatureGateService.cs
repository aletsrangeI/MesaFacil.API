namespace Interface.Suscripciones;

/// <summary>
/// Spec 021: motor de verificación de acceso a características por plan de suscripción (tier).
///
/// IMPORTANTE (Directriz de negocio): mientras la sección de configuración
/// "FeatureGating:Enabled" esté en false (valor por defecto durante demos comerciales,
/// pilotos y el programa de Restaurantes Fundadores), TieneAccesoAsync siempre retorna true
/// sin consultar la base de datos, garantizando que ninguna pantalla ni funcionalidad quede
/// bloqueada. El chequeo real contra el catálogo de planes solo ocurre cuando el negocio active
/// el licenciamiento (Enabled = true) en el futuro.
/// </summary>
public interface IFeatureGateService
{
    /// <summary>
    /// Indica si la Empresa indicada tiene acceso al feature/módulo solicitado
    /// (ej. "ModuloRecetas", "ModuloKds", "ModuloSplitBill", "ModuloCfdiXml", "ModuloCxP").
    /// </summary>
    Task<bool> TieneAccesoAsync(int empresaId, string featureName);
}
