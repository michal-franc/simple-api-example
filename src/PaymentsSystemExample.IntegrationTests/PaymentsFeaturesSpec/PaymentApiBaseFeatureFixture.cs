using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

using LightBDD.Framework;
using LightBDD.XUnit2;
using LightBDD.Framework.Scenarios.Extended;

using PaymentsSystemExample.Api;
using PaymentsSystemExample.Api.Services;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace PaymentsSystemExample.IntegrationTests.PaymentsFeaturesSpec
{
    public partial class PaymentApiBaseFeatureFixture : FeatureFixture
    {
        protected readonly HttpClient _client;
        protected IPaymentPersistenceService _localDynamoDB;

        public PaymentApiBaseFeatureFixture()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            // This create in proc hosted api
            var hostBuilder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseStartup<PaymentApiStartup>();

            var server = new TestServer(hostBuilder);
            _client = server.CreateClient();


            _localDynamoDB = new LocalPaymentPersistenceServiceDynamoDB(configuration);
        }
    }
}