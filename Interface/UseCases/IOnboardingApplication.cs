using Common;
using DTO.Onboarding;

namespace Interface.UseCases;

public interface IOnboardingApplication
{
    Task<Response<OnboardingEstadoDTO>> ObtenerEstadoAsync(int idEmpresa, CancellationToken ct = default);
    Task<Response<ProvisionarRestauranteResponseDTO>> ProvisionarRestauranteAsync(
        ProvisionarRestauranteRequestDTO dto, 
        int idUsuarioAdmin, 
        int idEmpresa, 
        CancellationToken ct = default);
}
