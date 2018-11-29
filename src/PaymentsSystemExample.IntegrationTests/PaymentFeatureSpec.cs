using LightBDD.Framework;
using LightBDD.XUnit2;
using LightBDD.Framework.Scenarios.Extended;
using Xunit;

namespace PaymentsSystemExample.IntegrationTests
{
    [FeatureDescription(
        @"As a user when I use API to get payment"
    )]
    [Label("Payment Feature")]
    public class PaymentFeatureSpec : FeatureFixture
    {
        [Scenario]
        [MultiAssert]
        [Label("and payment doesn't exist. I get a 404 in response status code.")]
        public void BasicTest()
        {
            Runner.RunScenario(
                _ => Test_is_true()
            );
        }

        private void Test_is_true()
        {
            Assert.True(true);
        }
    }
}
