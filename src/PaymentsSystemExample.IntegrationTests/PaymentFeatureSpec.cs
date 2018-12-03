using LightBDD.Framework;
using LightBDD.XUnit2;
using LightBDD.Framework.Scenarios.Extended;
using Xunit;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using FluentAssertions;

using PaymentsSystemExample.Api;

// TODO: how to simulate that something exists?
// TODO: provide a way to mock a database service layer on Startup?
// TODO: start in memory database and provide it for integration tests
// TODO: start real database? using docker?

namespace PaymentsSystemExample.IntegrationTests
{
    [FeatureDescription(
        @"As a user when I call Payment API to fetch payment"
    )]
    [Label("Payment API Fetch")]
    public class PaymentFeatureSpec : FeatureFixture
    {
        private readonly HttpClient _client;
        private HttpResponseMessage _message;

        public PaymentFeatureSpec()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<PaymentApiStartup>());
            _client = server.CreateClient();
        }

        [Scenario]
        [MultiAssert]
        [Label("And payment doesn't exist")]
        public void PaymentDoesntExistTest()
        {
            Runner.RunScenario(
                _ => I_call_api_with_id(1),
                _ => I_get_status_code(404)
            );
        }

        [Scenario]
        [MultiAssert]
        [Label("And payment does exist")]
        public void PaymentDoesExistTest()
        {
            Runner.RunScenario(
                _ => I_call_api_with_id(2),
                _ => I_get_status_code(200),
                _ => I_get_payment_data_in_content()
            );
        }

        private void I_get_status_code(int expectedStatusCode)
        {
            _message.StatusCode.Should().Be(expectedStatusCode);
        }

        private void I_call_api_with_id(int id)
        {
            //TODO: remove result and make it async requires LightBDD Async Scenarios
            _message = _client.GetAsync($"/api/payment/{id}").Result;
        }

        private void I_get_payment_data_in_content()
        {
            Assert.True(true);
        }
    }
}
