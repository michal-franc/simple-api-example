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
        @"As a user when I call Payment API to GET payment"
    )]
    [Label("Payment API GET")]
    public class GetSpec : PaymentApiBaseFeatureFixture
    {
        [Scenario]
        [MultiAssert]
        [Label("And the payment does exist")]
        public async Task PaymentDoesExistTest()
        {
            var paymentId = Guid.NewGuid();
            var payment = TestPaymentBuilder.Create(paymentId);

            await Runner.RunScenarioAsync(
               _ => I_populate_db_with_payment(payment),
               _ => I_call_api_get_with(paymentId),
               _ => I_get_status_code(200),
               _ => I_get_payment_data_in_the_content(payment)
            );
        }

        [Scenario]
        [MultiAssert]
        [Label("And the payment doesn't exist")]
        public async Task PaymentDoesntExistTest()
        {
            var paymentId = Guid.NewGuid();

            await Runner.RunScenarioAsync(
               _ => I_call_api_get_with(paymentId),
               _ => I_get_status_code(404)
            );
        }

        [Scenario]
        [MultiAssert]
        [Label("And I use incorrect id")]
        public async Task PaymentIncorrectId()
        {
            await Runner.RunScenarioAsync(
               _ => I_call_api_get_with("incorrect_id"),
               _ => I_get_status_code(400),
               _ => I_can_see_a_message_containing("Expected Guid format.")
            );
        }
    }
}
