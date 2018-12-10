using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

using PaymentsSystemExample.Domain.Adapters;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using PaymentsSystemExample.IntegrationTests.TestDataBuilders;


namespace PaymentsSystemExample.IntegrationTests.PaymentsFeaturesSpec
{
    public partial class PaymentApiBaseFeatureFixture
    {
        protected HttpResponseMessage _message;

        protected async Task I_can_see_a_message_containing(string messagePart)
        {
            var rawData = await _message.Content.ReadAsStringAsync();
            rawData.Should().Contain(messagePart);
        }

        protected Payment I_create_payment_with_invalid_amount()
        {
            return new Payment();
        }

        protected async Task I_populate_db_with_payment(Payment payment)
        {
            await _localDynamoDB.Create(new [] { payment });
        }

        protected async Task I_get_status_code(int expectedStatusCode)
        {
            _message.StatusCode.Should().Be(expectedStatusCode);
        }

        protected async Task I_call_api_get_with(Guid id)
        {
            _message = await _client.GetAsync($"/api/v1/payment/{id}");
        }

        protected async Task I_call_api_get_with(string id)
        {
            _message = await _client.GetAsync($"/api/v1/payment/{id}");
        }

        protected async Task I_call_api_delete_with(Guid id)
        {
            _message = await _client.DeleteAsync($"/api/v1/payment/{id}");
        }

        private StringContent CreatePutRequest(Payment payment)
        {
            var paymentRoot = new PaymentRoot();
            paymentRoot.Data = new List<Payment>();
            paymentRoot.Data.Add(payment);

            var serializedPaymentRoot = JsonConvert.SerializeObject(paymentRoot);
            var stringContent = new StringContent(serializedPaymentRoot);
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return stringContent;
        }

        protected async Task I_call_api_put_without_culture_code(Payment payment)
        {
            var stringContent = CreatePutRequest(payment);
            _message = await _client.PutAsync($"/api/v1/payment/", stringContent);
        }

        protected async Task I_call_api_put_with_culture_code(Payment payment, string cultureCode)
        {
            var stringContent = CreatePutRequest(payment);
            stringContent.Headers.Add("X-CultureCode", cultureCode);

            _message = await _client.PutAsync($"/api/v1/payment/", stringContent);
        }

        protected async Task I_call_api_delete_with(string id)
        {
            _message = await _client.DeleteAsync($"/api/v1/payment/{id}");
        }

        protected async Task I_get_payment_data_in_the_content(Payment payment)
        {
            _message.Content.Should().NotBeNull();
            var rawData = await _message.Content.ReadAsStringAsync();
            var paymentFromDB = JsonConvert.DeserializeObject<Payment>(rawData);

            paymentFromDB.Id.Should().Be(payment.Id);
            paymentFromDB.OrganisationId.Should().Be(payment.OrganisationId);
        }
    }
}