using System;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;
using LightBDD.Framework;
using LightBDD.XUnit2;
using LightBDD.Framework.Scenarios.Extended;
using Xunit;

using PaymentsSystemExample.Domain.Adapters;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using PaymentsSystemExample.IntegrationTests.TestDataBuilders;

namespace PaymentsSystemExample.IntegrationTests.PaymentsFeaturesSpec
{
    [FeatureDescription(
        @"As a user when I call Payment API to PUT payment"
    )]
    [Label("Payment API PUT")]
    public class PutSpec : PaymentApiBaseFeatureFixture
    {
        [Scenario]
        [MultiAssert]
        [Label("With a valid Payment and Culture Code")]
        public async Task WithValidPaymentAndCultureCode()
        {
            var paymentId = Guid.NewGuid();
            var payment = TestPaymentBuilder.Create(paymentId);

            await Runner.RunScenarioAsync(
               _ => I_call_api_put_with_culture_code(payment, "en-GB"),
               _ => I_get_status_code(200),
               _ => I_call_api_get_with(paymentId),
               _ => I_get_status_code(200)
            );
        }

        [Scenario]
        [MultiAssert]
        [Label("Without Culture Code")]
        public async Task WithoutCultureCode()
        {
            var paymentId = Guid.NewGuid();
            var payment = TestPaymentBuilder.Create(paymentId);

            await Runner.RunScenarioAsync(
               _ => I_call_api_put_without_culture_code(payment),
               _ => I_get_status_code(400),
               _ => I_can_see_a_message_containing("X-CultureCode header missing.")
            );
        }

        [Scenario]
        [MultiAssert]
        [Label("With Payment amount -1")]
        public async Task WithNegativeAmount()
        {
            var paymentId = Guid.NewGuid();
            var payment = TestPaymentBuilder.Create(paymentId);
            payment.Attributes.Amount = -1;

            await Runner.RunScenarioAsync(
               _ => I_call_api_put_with_culture_code(payment, "en-GB"),
               _ => I_get_status_code(400),
               _ => I_can_see_a_message_containing("Amount should be greater than 0.")
            );
        }

        [Scenario]
        [MultiAssert]
        [Label("With Payment Type missing")]
        public async Task WithMissingPaymentType()
        {
            var paymentId = Guid.NewGuid();
            var payment = TestPaymentBuilder.Create(paymentId);
            payment.Type = string.Empty;

            await Runner.RunScenarioAsync(
               _ => I_call_api_put_with_culture_code(payment, "en-GB"),
               _ => I_get_status_code(400),
               _ => I_can_see_a_message_containing("Payment Type missing.")
            );
        }
    }
}