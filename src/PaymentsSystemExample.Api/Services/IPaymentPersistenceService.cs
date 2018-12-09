using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using Amazon.Auth;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;

namespace PaymentsSystemExample.Api.Services
{
    public interface IPaymentPersistenceService
    {
        Task<Payment> Get(Guid paymentid);
        Task<bool> Delete(Guid paymentid);
        Task<bool> Create(IEnumerable<Payment> payments);
    }

    // TODO: Add Payment
    // TODO: Get Payment
    // TODO: Delete Payment
    // TODO: List Payments per org
    // TODO: one filter as a example (but mention that dynamodb is limited in this area especially with blob based documents)
    // TODO: Update Payment

    public class PaymentPersistenceServiceDynamoDB: IPaymentPersistenceService
    {
        string testDynamodbHost = "http://localhost:4569";
        AmazonDynamoDBClient _client;

        public PaymentPersistenceServiceDynamoDB()
        {
            var config = new AmazonDynamoDBConfig();
            config.ServiceURL = testDynamodbHost;
            config.UseHttp = true;
            _client = new AmazonDynamoDBClient("test" , "test" , "test", config);
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