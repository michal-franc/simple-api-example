using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using Amazon;
using Amazon.Auth;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;

namespace PaymentsSystemExample.Api.Services
{
    // Service for local testing
    public class LocalPaymentPersistenceServiceDynamoDB: PaymentPersistenceServiceDynamoDB
    {
        // Localstack URL - should be injected through config
        private const string LocalDynamoDBHost = "http://localhost:4569";

        // LocalStack doesnt have credentials but AWS SDk requires values
        private const string CredentialsStub = "test";

        public LocalPaymentPersistenceServiceDynamoDB()
        {
            var config = new AmazonDynamoDBConfig();
            config.ServiceURL = LocalDynamoDBHost;
            config.UseHttp = true;

            // this should be potentially injected through the IOC but ... ideally I would move the DynamoDB and amazong SDto separate project
            // and I wouldnt have to expose DynamoDB to WEB layer - at the moment this is not really Inversion of Control
            // I kept this structure for simplicity
            _client = new AmazonDynamoDBClient(CredentialsStub, CredentialsStub, CredentialsStub, config);
        }
    }

    // Service for production usage
    public class PaymentPersistenceServiceDynamoDB: IPaymentPersistenceService
    {
        protected AmazonDynamoDBClient _client;
        private JsonSerializerSettings _attributesSerializerSettings;

        public PaymentPersistenceServiceDynamoDB()
        {
            // For simplicity usually this is either in the aws profile or ENV
            // This is still called even when using Local
            _client = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);

            // As i decided to keep attributes as a blob I need to serialize it
            _attributesSerializerSettings = new JsonSerializerSettings
            {
                Culture = new CultureInfo("en-GB"),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.Default,
                Formatting = Formatting.None
            };
        }

        public async Task<bool> Update(IEnumerable<Payment> payments)
        {
            try
            {
                var table = Table.LoadTable(_client, "payments");
                var batchWrite = table.CreateBatchWrite();

                foreach(var payment in payments)
                {
                    var document = PaymentToDynamoDB(payment);
                    batchWrite.AddDocumentToPut(document);
                }

                await batchWrite.ExecuteAsync();
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<bool> Create(IEnumerable<Payment> payments)
        {
            try
            {
                var table = Table.LoadTable(_client, "payments");
                var batchWrite = table.CreateBatchWrite();

                foreach(var payment in payments)
                {
                    var document = PaymentToDynamoDB(payment);
                    batchWrite.AddDocumentToPut(document);
                }

                await batchWrite.ExecuteAsync();
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<bool> Delete(Guid paymentId)
        {
            try
            {
                var table = Table.LoadTable(_client, "payments");
                var result = await table.DeleteItemAsync(paymentId);
                return result != null;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<Payment> Get(Guid paymentId)
        {
            try
            {
                var table = Table.LoadTable(_client, "payments");

                var document = await table.GetItemAsync(paymentId);
                if(document is null)
                {
                    return null;
                }

                var payment = new Payment();
                payment.Attributes = JsonConvert.DeserializeObject<Attributes>(document["attributes"], _attributesSerializerSettings);
                payment.Id = Guid.Parse(document["PaymentId"]);
                payment.OrganisationId = Guid.Parse(document["OrganisationId"]);
                payment.Version = int.Parse(document["Version"]);
                payment.Type = document["Type"];

                return payment;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private Document PaymentToDynamoDB(Payment payment)
        {
            var document = new Document();
            document["PaymentId"] = payment.Id;
            document["OrganisationId"] = payment.OrganisationId;
            document["Version"] = payment.Version;
            document["Type"] = payment.Type;
            document["attributes"] = JsonConvert.SerializeObject(payment.Attributes, _attributesSerializerSettings);
            return document;
        }
    }
}