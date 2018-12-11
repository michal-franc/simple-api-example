using FluentValidation;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.Domain.Adapters.Validators
{
    public class PaymentValidator: AbstractValidator<Payment> 
    {
        public PaymentValidator()
        {
            RuleFor(x => x.Type).NotEmpty().WithMessage("Payment Type missing.");
            RuleFor(x => x.Id).NotNull().WithMessage("Payment Id missing.");
            RuleFor(x => x.OrganisationId).NotEmpty().WithMessage("Organisation Id missing.");
            RuleFor(x => x.Attributes).SetValidator(new AttributesValidator());
        }
    }

}