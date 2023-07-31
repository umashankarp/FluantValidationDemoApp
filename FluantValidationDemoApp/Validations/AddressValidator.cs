using FluantValidationDemoApp.DTOs;
using FluentValidation;

namespace FluantValidationDemoApp.Validations
{
    public class AddressValidator: AbstractValidator<AddressDTO>
    {

        public AddressValidator()
        {
            RuleFor(x => x.State).NotNull().NotEmpty();
            RuleFor(x => x.Country).NotEmpty().WithMessage("Please specify a Country.");

            RuleFor(x => x.Postcode).NotNull();
            RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
        }

        private bool BeAValidPostcode(string postcode)
        {
            return postcode.Length==6;

        }
    }
}
