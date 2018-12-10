using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FluentValidation;

namespace PaymentsSystemExample.Domain.Adapters.JsonObjects
{
    // TODO: of course this is an example we would need to cover all the fields in all the domain objects with validations
    public class AttributesValidator: AbstractValidator<Attributes> 
    {
        public AttributesValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount should be greater than 0.");
        }
    }

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

    public class Payment
    {
        public string Type { get; set; }
        public Guid Id { get; set; }
        public int Version { get; set; }

        [JsonProperty("organisation_id")]
        public Guid OrganisationId { get; set; }

        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }

        public IReadOnlyDictionary<string, string> Validate()
        {
            var validator = new PaymentValidator();
            var results = validator.Validate(this);
            var domainErrors = new Dictionary<string, string>();

            foreach(var result in results.Errors)
            {
                domainErrors.Add(result.PropertyName, result.ErrorMessage);
            }

            return domainErrors;
        }
    }
}