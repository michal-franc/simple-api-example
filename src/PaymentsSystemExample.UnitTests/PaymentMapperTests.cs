using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace PaymentsSystemExample.UnitTests
{
    // Used http://json2csharp.com/ to quickly generate object
    public class BeneficiaryParty
    {
        public string account_name { get; set; }
        public string account_number { get; set; }
        public string account_number_code { get; set; }
        public int account_type { get; set; }
        public string address { get; set; }
        public string bank_id { get; set; }
        public string bank_id_code { get; set; }
        public string name { get; set; }
    }

    public class SenderCharge
    {
        public string amount { get; set; }
        public string currency { get; set; }
    }

    public class ChargesInformation
    {
        public string bearer_code { get; set; }
        public List<SenderCharge> sender_charges { get; set; }
        public string receiver_charges_amount { get; set; }
        public string receiver_charges_currency { get; set; }
    }

    public class DebtorParty
    {
        public string account_name { get; set; }
        public string account_number { get; set; }
        public string account_number_code { get; set; }
        public string address { get; set; }
        public string bank_id { get; set; }
        public string bank_id_code { get; set; }
        public string name { get; set; }
    }

    public class Fx
    {
        public string contract_reference { get; set; }
        public string exchange_rate { get; set; }
        public string original_amount { get; set; }
        public string original_currency { get; set; }
    }

    public class SponsorParty
    {
        public string account_number { get; set; }
        public string bank_id { get; set; }
        public string bank_id_code { get; set; }
    }

    public class Attributes
    {
        public decimal amount { get; set; }
        public BeneficiaryParty beneficiary_party { get; set; }
        public ChargesInformation charges_information { get; set; }
        public string currency { get; set; }
        public DebtorParty debtor_party { get; set; }
        public string end_to_end_reference { get; set; }
        public Fx fx { get; set; }
        public string numeric_reference { get; set; }
        public string payment_id { get; set; }
        public string payment_purpose { get; set; }
        public string payment_scheme { get; set; }
        public string payment_type { get; set; }
        public string processing_date { get; set; }
        public string reference { get; set; }
        public string scheme_payment_sub_type { get; set; }
        public string scheme_payment_type { get; set; }
        public SponsorParty sponsor_party { get; set; }
    }

    public class RequestMetadata
    {
        public string type { get; set; }
        public string id { get; set; }
        public int version { get; set; }
        public string organisation_id { get; set; }
        public Attributes attributes { get; set; }
    }

    public class RequestRoot
    {
        public List<RequestMetadata> data { get; set; }
    }

    public class ParsedPayment
    {
        public decimal Amount { get; }

        public ParsedPayment(Attributes attributes)
        {
            this.Amount = attributes.amount;
        }
    }

    public interface IPaymentMapper
    {
        IEnumerable<ParsedPayment> Map(string rawData);
    }

    class AmountConverter: JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(decimal?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                decimal value = default(decimal);

                // Allowing only decimal point here
                if(Decimal.TryParse(token.ToString(), NumberStyles.AllowDecimalPoint, serializer.Culture, out value))
                {
                    return value;
                }

                throw new JsonSerializationException("Incorrect decimal format: " + token.ToString());
            }

            if (token.Type == JTokenType.Null && objectType == typeof(decimal?))
            {
                return null;
            }

            throw new JsonSerializationException("Not supported token type: " + token.Type.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    // This has to be singleton injected
    public class PaymentMapperJson : IPaymentMapper
    {
        // Outside of this class we wont need to change the parsing errors collection -> changing only to enumeration
        // This does not support validation per payment object
        // But if needed we can change this object to dictionary and potentialy during parsing identify
        // payment id as a key and collection of errors as a value
        public IEnumerable<string> ParsingErrors => _parsingErrors;
        public bool HasErrors => _parsingErrors.Count > 0;

        // ICollection removes ability to sort (we wont need sorting in this context so I am hiding this ability)
        private ICollection<string> _parsingErrors;
        private JsonSerializerSettings _serializerSettings;

        public PaymentMapperJson(string cultureCode)
        {
            _parsingErrors = new List<string>();
            _serializerSettings = new JsonSerializerSettings
            { 
                // I would catch here first basic validations - like timezone in date, amount decimal separator, id not being a guid etc
                // Things like is this a correct currency code, or number code this would be a more Domain validation
                // And will be performed on different layer
                Error = (sender, args) => 
                {
                    _parsingErrors.Add(args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = true;
                },
                Culture = new CultureInfo(cultureCode),
                // As this is singleton for the global scope maintaned by container I am not worried about creating this objects.
                Converters = new List<JsonConverter> { new AmountConverter() }
            };
        }

        public IEnumerable<ParsedPayment> Map(string rawJson)
        {
            var parsedPayments = new List<ParsedPayment>();

            var rawPayments = JsonConvert.DeserializeObject<RequestRoot>(rawJson, _serializerSettings).data;

            foreach(var rawPayment in rawPayments)
            {
                parsedPayments.Add(new ParsedPayment(rawPayment.attributes));
            }

            return parsedPayments;
        }
    }

    // Helper class for test so we dont have to write en-GB in all the tests
    public class PaymentMapperJsonGB : PaymentMapperJson
    {
        public PaymentMapperJsonGB(): base("en-GB") {}
    }

    // These raw payment objects - add json to them and keep them internal to different layer
    // Test mapping here with special rules and validations added to json .net
    // then create a WritePayment class that when leaves the Adapter Layer is transformed to Payment Class that is read only.
    // So that no one can change the class in other layers using it
    public class WhenMappingPaymentAmountFromJsonTests
    {
        [Fact]
        public void AndAmountIsValid_ThenReturnPayment_AndNowParsingErrors()
        {
            var sut = new PaymentMapperJsonGB();
            var expectedAmount = 100.21m;

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        'amount': '{expectedAmount}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);
            Assert.Equal(expectedAmount, resultPayment.First().Amount);
            Assert.False(sut.HasErrors);
        }

        [Fact]
        public void AndInvalidDecimalSeparator_ThenReturnPayment_WithParsingErrors()
        {
            var sut = new PaymentMapperJsonGB();
            var expectedAmount = "100,21";

            var testJson = $@"{{
                'data': [{{
                    'attributes':{{
                        'amount': '{expectedAmount}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);
            Assert.True(sut.HasErrors);
        }

        [Fact]
        public void AndForDifferentCulture_WithCommaDecimalSeparator_RetunrPayment_AndNoParsingErrors()
        {
            var testCulture = "nl-BE";
            var sut = new PaymentMapperJson(testCulture);
            var expectedAmount = "100,21";

            var testJson = $@"{{
                'data': [{{
                    'attributes':{{
                        'amount': '{expectedAmount}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);
            Assert.False(sut.HasErrors);
            Assert.Equal(expectedAmount, resultPayment.First().Amount.ToString(CultureInfo.CreateSpecificCulture(testCulture)));
        }
    }
}