using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

using LightBDD.Framework;
using LightBDD.XUnit2;
using LightBDD.Framework.Scenarios.Extended;

using PaymentsSystemExample.Api;
using PaymentsSystemExample.Api.Services;

namespace PaymentsSystemExample.IntegrationTests.PaymentsFeaturesSpec
{
    public partial class PaymentApiBaseFeatureFixture : FeatureFixture
    {
        protected readonly HttpClient _client;
        protected IPaymentPersistenceService _localDynamoDB;

        public PaymentApiBaseFeatureFixture()
        {
            // This create in proc hosted api
            var hostBuilder = new WebHostBuilder().UseStartup<PaymentApiStartup>();
            var server = new TestServer(hostBuilder);
            _client = server.CreateClient();
            _localDynamoDB = new LocalPaymentPersistenceServiceDynamoDB();
        }
    }
}