using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FluentValidation;
using PaymentsSystemExample.Domain.Adapters.Validators;

namespace PaymentsSystemExample.Domain.Adapters.JsonObjects
{
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