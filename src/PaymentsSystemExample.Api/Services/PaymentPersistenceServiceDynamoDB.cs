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
        private const string LocalDynamoDBHost = "http://localhost:4569";
        private const string CredentialsStub = "test";

        public LocalPaymentPersistenceServiceDynamoDB()
        {
            var config = new AmazonDynamoDBConfig();
            config.ServiceURL = LocalDynamoDBHost;
            config.UseHttp = true;
            _client = new AmazonDynamoDBClient(CredentialsStub, CredentialsStub, CredentialsStub, config);
        }
    }

    // Service for production usage
    public class PaymentPersistenceServiceDynamoDB: IPaymentPersistenceService
    {
        protected AmazonDynamoDBClient _client;

        public PaymentPersistenceServiceDynamoDB()
        {
            // For simplicity usually this is either in the aws profile or ENV
            _client = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);
        }

        public async Task<bool> Update(IEnumerable<Payment> payments)
        {
            try
            {
                var table = Table.LoadTable(_client, "payments");
                var batchWrite = table.CreateBatchWrite();

                foreach(var payment in payments)
                {
                    var document = new Document();
                    document["PaymentId"] = payment.Id;
                    document["OrganisationId"] = payment.OrganisationId;
                    document["Version"] = payment.Version;
                    document["Type"] = payment.Type;

                    var serializerSettings = new JsonSerializerSettings
                    {
                        Culture = new CultureInfo("en-GB"),
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        NullValueHandling = NullValueHandling.Ignore,
                        StringEscapeHandling = StringEscapeHandling.Default,
                        Formatting = Formatting.None
                    };

                    document["attributes"] = JsonConvert.SerializeObject(payment.Attributes, serializerSettings);

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
                    var document = new Document();
                    document["PaymentId"] = payment.Id;
                    document["OrganisationId"] = payment.OrganisationId;
                    document["Version"] = payment.Version;
                    document["Type"] = payment.Type;

                    var serializerSettings = new JsonSerializerSettings
                    {
                        Culture = new CultureInfo("en-GB"),
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        NullValueHandling = NullValueHandling.Ignore,
                        StringEscapeHandling = StringEscapeHandling.Default,
                        Formatting = Formatting.None
                    };

                    document["attributes"] = JsonConvert.SerializeObject(payment.Attributes, serializerSettings);

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

        // TODO: should the dynamodb be primary key payment id - sort key -> org id? so that we can create a composite key?
        public async Task<bool> Delete(Guid paymentId)
        {
            var table = Table.LoadTable(_client, "payments");
            var result = await table.DeleteItemAsync(paymentId);
            return result != null;
        }

        public async Task<Payment> Get(Guid paymentId)
        {
            var table = Table.LoadTable(_client, "payments");
            var serializerSettings = new JsonSerializerSettings
            {
                Culture = new CultureInfo("en-GB"),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.Default,
                Formatting = Formatting.None
            };

            var document = await table.GetItemAsync(paymentId);
            if(document is null)
            {
                return null;
            }

            var payment = new Payment();
            payment.Attributes = JsonConvert.DeserializeObject<Attributes>(document["attributes"], serializerSettings);
            payment.Id = Guid.Parse(document["PaymentId"]);
            payment.OrganisationId = Guid.Parse(document["OrganisationId"]);
            payment.Version = int.Parse(document["Version"]);
            payment.Type = document["Type"];

            return payment;
        }
    }
}