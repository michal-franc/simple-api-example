using System.Globalization;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using PaymentsSystemExample.Domain.Adapters.CustomJsonConverters;

namespace PaymentsSystemExample.Domain.Adapters
{
    // TODO: This has to be singleton injected
    public class PaymentParserJson : IPaymentParser
    {
        // Outside of this class we wont need to change the parsing errors collection -> changing only to enumeration
        // This does not support validation per payment object
        // But if needed we can change this object to dictionary and potentialy during parsing identify
        // payment id as a key and collection of errors as a value
        public IEnumerable<string> ParsingErrors => _parsingErrors;
        public bool HasErrors => _parsingErrors.Count > 0;

        // ICollection removes ability to sort (we wont need sorting in this context so I am hiding this ability)
        private ICollection<string> _parsingErrors;

        public PaymentParserJson()
        {
            _parsingErrors = new List<string>();
        }

        public IEnumerable<Payment> Parse(string rawJson, string cultureCode)
        {
            // I had to move settings initialization here as i want to create multi tenant app that cculture is configured per request
            var serializerSettings = new JsonSerializerSettings
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
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            return JsonConvert.DeserializeObject<PaymentRoot>(rawJson, serializerSettings).Data;
        }
    }
}