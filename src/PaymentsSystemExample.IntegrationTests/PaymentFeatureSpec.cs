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
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using PaymentsSystemExample.Api.Formatters;
using PaymentsSystemExample.Api.Services;
using PaymentsSystemExample.Domain.Adapters;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

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

            return payment;
        }
    }

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
            var hostBuilder = new WebHostBuilder()
                .ConfigureServices(coll =>{
                    //var paymentPersistenceExisting = coll.FirstOrDefault(d => d.ServiceType == typeof(IPaymentPersistenceService));
                    //coll.Remove(paymentPersistenceExisting);
                    //coll.AddSingleton<IPaymentPersistenceService, InMemPersistenceService>();
                })
                .UseStartup<PaymentApiStartup>();
            var server = new TestServer(hostBuilder);
            _client = server.CreateClient();
        }

        [Scenario]
        [MultiAssert]
        [Label("And I get payment that does exist")]
        public async Task PaymentDoesExistTest()
        {
            var paymentId = Guid.NewGuid();

            await Runner.RunScenarioAsync(
               _ => I_populate_db_with_payment(paymentId),
               _ => I_call_api_with_id(paymentId),
               _ => I_get_status_code(200),
               _ => I_get_payment_data_in_content()
            );
        }

        private async Task I_populate_db_with_payment(Guid id)
        {
            var persister = new LocalPaymentPersistenceServiceDynamoDB();
            await persister.Create(new [] { TestPaymentBuilder.Create(id) });
        }

        // TODO: should this be checking status code? or should i hide code and use -> BadRequest -> ok etc
        private async Task I_get_status_code(int expectedStatusCode)
        {
            _message.StatusCode.Should().Be(expectedStatusCode);
        }

        private async Task I_delete_payment_with_id(Guid id)
        {
            //TODO: remove result and make it async requires LightBDD Async Scenarios
            var test = await _client.DeleteAsync($"/api/v1/payment/{id}");
        }

        private async Task I_create_payment_with_id(Guid id)
        {
            //TODO: remove result and make it async requires LightBDD Async Scenarios
            var test = await _client.PutAsJsonAsync($"/api/v1/payment/{id}", id);
        }

        private async Task I_call_api_with_id(Guid id)
        {
            //TODO: remove result and make it async requires LightBDD Async Scenarios
            _message = await _client.GetAsync($"/api/v1/payment/{id}");
        }

        private async Task I_call_api_with_incorrect_id(string id)
        {
            //TODO: remove result and make it async requires LightBDD Async Scenarios
            _message = await _client.GetAsync($"/api/v1/payment/{id}");
        }

        private async Task I_get_payment_data_in_content()
        {
            _message.Content.Should().NotBeNull();
        }
    }
}
