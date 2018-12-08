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

// TODO: Real DB service -> docker dynamodb
// TODO: Mocked DB using localstack -> usefull to test interaction with API
// TODO: add check of payment and real payment builder with mocked data.

// TODO:  All the scenarios to cover
//  Posting not handled HTTP Methdo -> correct error
//  Posting to non existing endpoint -> 404

//  scenarios:
//  get (done)
//   - 200  (done)
//   - 404 -> no payment id (done)

//  put (full update replace)
//   - 200 
//   - incorrect data - validation error
//      - missing values
//      - incorrect amount
//      - incorrect date format
//  patch (partial update)
//    - payment id -> 
//  list -> get on resource with S
//  delete ->
//        404 -> payment
//        200 -> success
//  all  -> mocked basic auth token
//   - 403 -> no token
//   - 403 -> token - org mismatch
//   - error -> no org id no version and other metadata
//   - 500 -> how to test it? exception handling within services

// Updates need to check if version number has not changed
// Optimistic concurrency
// -> return -> 412 item modified
///  https://stackoverflow.com/questions/5369480/when-is-it-appropriate-to-respond-with-a-http-412-error 

namespace PaymentsSystemExample.IntegrationTests
{
    [FeatureDescription(
        @"As a user when I call Payment API to GET payment"
    )]
    [Label("Payment API GET")]
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
        [Label("And I get payment that doesn't exist")]
        public void PaymentDoesntExistTest()
        {
            var nonExistingPaymentId = Guid.NewGuid();

            Runner.RunScenario(
                _ => I_call_api_with_id(nonExistingPaymentId),
                _ => I_get_status_code(404)
            );
        }

        [Scenario]
        [MultiAssert]
        [Label("And I get payment that does exist")]
        public void PaymentDoesExistTest()
        {
            var existingPaymentId = Guid.NewGuid();

            Runner.RunScenario(
                //TODO: This step cannot be part of this test as we then test two functionalities if put fails this one will also fail - tests should be isolated
                _ => I_populate_db_with_payment(existingPaymentId),
                _ => I_call_api_with_id(existingPaymentId),
                _ => I_get_status_code(200),
                _ => I_get_payment_data_in_content()
            );
        }

        [Scenario]
        [MultiAssert]
        [Label("And I call api with incorrect id value")]
        public void PaymentIncorrectCall()
        {
            var incorrectId = "incorrect_ID";
            Runner.RunScenario(
                _ => I_call_api_with_incorrect_id(incorrectId),
                _ => I_get_status_code(400)
                // TODO: Should we verify error messsages?
                //_ => I_get_error_message("")
            );
        }

        private void I_populate_db_with_payment(Guid id)
        {
            //TODO: need db implementation here :X
        }

        // TODO: should this be checking status code? or should i hide code and use -> BadRequest -> ok etc
        private void I_get_status_code(int expectedStatusCode)
        {
            _message.StatusCode.Should().Be(expectedStatusCode);
        }

        private void I_delete_payment_with_id(Guid id)
        {
            //TODO: remove result and make it async requires LightBDD Async Scenarios
            var test = _client.DeleteAsync($"/api/payment/{id}").Result;
        }

        private void I_create_payment_with_id(Guid id)
        {
            //TODO: remove result and make it async requires LightBDD Async Scenarios
            var test = _client.PutAsJsonAsync($"/api/payment/{id}", id).Result;
        }

        private void I_call_api_with_id(Guid id)
        {
            //TODO: remove result and make it async requires LightBDD Async Scenarios
            _message = _client.GetAsync($"/api/payment/{id}").Result;
        }

        private void I_call_api_with_incorrect_id(string id)
        {
            //TODO: remove result and make it async requires LightBDD Async Scenarios
            _message = _client.GetAsync($"/api/payment/{id}").Result;
        }

        private void I_get_payment_data_in_content()
        {
            _message.Content.Should().NotBeNull();
        }
    }
}
