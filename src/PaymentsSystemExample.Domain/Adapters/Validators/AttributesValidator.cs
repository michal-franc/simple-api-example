using FluentValidation;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.Domain.Adapters.Validators
{
    // TODO: of course this is an example we would need to cover all the fields in all the domain objects with validations
    public class AttributesValidator: AbstractValidator<Attributes> 
    {
        public AttributesValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount should be greater than 0.");
        }
    }

}