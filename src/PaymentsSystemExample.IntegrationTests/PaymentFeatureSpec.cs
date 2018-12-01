using LightBDD.Framework;
using LightBDD.XUnit2;
using LightBDD.Framework.Scenarios.Extended;
using Xunit;

namespace PaymentsSystemExample.IntegrationTests
{
    [FeatureDescription(
        @"As a user when I call Payment API to fetch payment"
    )]
    [Label("Payment API Fetch")]
    public class PaymentFeatureSpec : FeatureFixture
    {
        [Scenario]
        [MultiAssert]
        [Label("And payment doesn't exist")]
        public void PaymentDoesntExistTest()
        {
            Runner.RunScenario(
                _ => I_get_status_code(404)
            );
        }

        [Scenario]
        [MultiAssert]
        [Label("And payment does exist")]
        public void PaymentDoesExistTest()
        {
            Runner.RunScenario(
                _ => I_get_status_code(200),
                _ => I_get_payment_data_in_content()
            );
        }

        private void I_get_status_code(int expectedStatusCode)
        {
            Assert.True(false);
        }

        private void I_get_payment_data_in_content()
        {
            Assert.True(false);
        }
    }
}
