using System;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.IntegrationTests.TestDataBuilders
{
    internal static class TestPaymentBuilder 
    {
        public static Payment Create(Guid id)
        {
            var payment = new Payment();
            payment.Id = id;
            payment.OrganisationId = Guid.NewGuid();
            payment.Version = 0;
            payment.Type = "Payment";
            payment.Attributes = new Attributes();
            payment.Attributes.Amount = 10.0m;

            return payment;
        }
    }

}