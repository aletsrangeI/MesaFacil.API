using Interface.Suscripciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Attributes;

/// <summary>
/// Spec 021: uso: [RequireFeature("ModuloRecetas")] sobre un controlador o acción.
///
/// Comportamiento con FeatureGating:Enabled == false (valor por defecto en demos, pilotos y
/// Restaurantes Fundadores): este filtro es un NO-OP transparente — ni siquiera resuelve
/// IFeatureGateService ni toca la base de datos, simplemente deja pasar la petición sin
/// overhead adicional ni cambio de status code.
///
/// Comportamiento con FeatureGating:Enabled == true: si la Empresa del usuario autenticado no
/// tiene la característica solicitada en su plan (o vencida su suscripción y fuera del periodo
/// de gracia), retorna 402 Payment Required con { errorCode, featureRequerida, planSugerido }.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireFeatureAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _featureName;

    public RequireFeatureAttribute(string featureName)
    {
        _featureName = featureName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

        // Modo demo (valor por defecto): no-op transparente, cero overhead.
        if (!configuration.GetValue<bool>("FeatureGating:Enabled", false))
        {
            await next();
            return;
        }

        var empresaIdClaim = context.HttpContext.User.FindFirst("empresa_id")?.Value;
        if (!int.TryParse(empresaIdClaim, out var empresaId))
        {
            context.Result = ConstruirRespuestaBloqueo(_featureName);
            return;
        }

        var featureGateService = context.HttpContext.RequestServices.GetRequiredService<IFeatureGateService>();
        var tieneAcceso = await featureGateService.TieneAccesoAsync(empresaId, _featureName);

        if (!tieneAcceso)
        {
            context.Result = ConstruirRespuestaBloqueo(_featureName);
            return;
        }

        await next();
    }

    private static ObjectResult ConstruirRespuestaBloqueo(string featureName)
    {
        return new ObjectResult(new
        {
            errorCode = "FEATURE_NOT_IN_PLAN",
            featureRequerida = featureName,
            planSugerido = SugerirPlan(featureName)
        })
        {
            StatusCode = StatusCodes.Status402PaymentRequired
        };
    }

    private static string SugerirPlan(string featureName) => featureName switch
    {
        "ModuloCfdiXml" or "ModuloCxP" => "Tier3_Multi",
        _ => "Tier2_Pro"
    };
}
