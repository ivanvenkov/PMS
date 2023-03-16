using API.Application.Models;
using FluentValidation;

namespace API.Application.Validators
{
    public class AdmitVehicleRequestValidator : AbstractValidator<AdmitVehicleRequest>
    {
        public AdmitVehicleRequestValidator()
        {
            RuleFor(x => x.RegNumber).NotEmpty().NotNull();
            RuleFor(x => x.RegNumber).Must(x => !x.ToCharArray().Any(char.IsWhiteSpace)).WithMessage("Empty spaces not allowed.");
            RuleFor(x => x.RegNumber).Length(6, 8);
        }
    }
}
