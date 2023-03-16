using API.Application.Models;
using FluentValidation;

namespace API.Application.Validators
{
    public class RemoveVehicleRequestValidator : AbstractValidator<RemoveVehicleRequest>
    {
        public RemoveVehicleRequestValidator()
        {
            RuleFor(x => x.RegNumber).NotEmpty().NotNull();
            RuleFor(x => x.RegNumber).Must(x => !x.ToCharArray().Any(char.IsWhiteSpace)).WithMessage("Empty spaces not allowed.");
            RuleFor(x => x.RegNumber).Length(6, 8);
        }
    }
}
