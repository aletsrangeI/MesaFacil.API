using System.Text.RegularExpressions;
using DTO.Sucursal;
using FluentValidation;

namespace Validator;

public class SucursalDTOValidator : AbstractValidator<SucursalDTO>
{
    public SucursalDTOValidator()
    {
        RuleFor(x => x.IdEmpresa)
            .GreaterThan(0)
            .WithMessage("El campo IdEmpresa es requerido y debe ser mayor a 0.");

        // Nombre requerido con límite
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(150).WithMessage("El campo Nombre no debe exceder los 150 caracteres.");

        // Dirección opcional con límite
        RuleFor(x => x.Direccion)
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.Direccion))
            .WithMessage("El campo Direccion no debe exceder los 300 caracteres.");

        // Zona horaria opcional; si se envía, validar forma IANA básica: Region/City
        RuleFor(x => x.ZonaHoraria)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.ZonaHoraria))
            .WithMessage("El campo ZonaHoraria no debe exceder los 100 caracteres.")
            .Matches(new Regex(@"^[A-Za-z]+(?:/[A-Za-z0-9_\-+]+)+$"))
            .When(x => !string.IsNullOrWhiteSpace(x.ZonaHoraria))
            .WithMessage("ZonaHoraria debe tener formato tipo IANA, por ejemplo: America/Mexico_City.");
    }
}