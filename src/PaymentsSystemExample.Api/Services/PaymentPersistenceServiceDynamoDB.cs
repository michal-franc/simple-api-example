using System;
using System.Net;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using Amazon;
using Amazon.Auth;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

using Microsoft.Extensions.Configuration;

namespace PaymentsSystemExample.Api.Services
{
    // Service for local testing
    public class LocalPaymentPersistenceServiceDynamoDB: PaymentPersistenceServiceDynamoDB
    {
        // LocalStack doesnt have credentials but AWS SDk requires values
        private const string CredentialsStub = "test";

        public LocalPaymentPersistenceServiceDynamoDB(IConfiguration configuration)
        {
            var host = configuration["DynamoDBHost"];
            var config = new AmazonDynamoDBConfig();

            config.ServiceURL = host;
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
        private const string TableName = "payments";

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
                var table = Table.LoadTable(_client, TableName);
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

        public async Task<IEnumerable<Payment>> List(Guid organisationId)
        {
            try
            {
                var paymentList = new List<Payment>();

                var table = Table.LoadTable(_client, TableName);

                var filter = new ScanFilter();
                filter.AddCondition("OrganisationId", ScanOperator.Equal, organisationId);

                var search = table.Scan(filter);

                var list = new List<Document>();

                do
                {
                    list = await search.GetNextSetAsync();
                    foreach (var document in list)
                    {
                        paymentList.Add(DynamodbToPayment(document));
                    }
                }
                while (!search.IsDone);

                return paymentList;
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
                var table = Table.LoadTable(_client, TableName);
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
                var request = new DeleteItemRequest
                {
                    TableName = TableName,
                    Key = new Dictionary<string,AttributeValue>() { { "PaymentId", new AttributeValue { S = paymentId.ToString() } } },

                    // required to check if actual item was deleted
                    ReturnValues = new ReturnValue("ALL_OLD")
                };

                var result = await _client.DeleteItemAsync(request);

                // With ALL_OLD document is returned in attributes - if it was deleted then attributes colleciton wont be empty
                return result.Attributes.Count > 0;
            }
            catch (System.Exception ex)
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

                return DynamodbToPayment(document);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private Payment DynamodbToPayment(Document document)
            {
                var payment = new Payment();
            payment.Attributes = JsonConvert.DeserializeObject<Attributes>(document["attributes"], _attributesSerializerSettings);
            payment.Id = Guid.Parse(document["PaymentId"]);
            payment.OrganisationId = Guid.Parse(document["OrganisationId"]);
            payment.Version = int.Parse(document["Version"]);
            payment.Type = document["Type"];
            return payment;
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