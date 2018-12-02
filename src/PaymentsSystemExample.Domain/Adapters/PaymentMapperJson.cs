using System.Globalization;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentsSystemExample.Domain.Adapters.CustomJsonConverters;

namespace PaymentsSystemExample.Domain.Adapters
{
    // TODO: This has to be singleton injected
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
                Converters = new List<JsonConverter> { new PaymentAmountConverter(), new ProcessingDateConverter() },
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
        }

        public IEnumerable<RequestMetadata> Map(string rawJson)
        {
            return JsonConvert.DeserializeObject<RequestRoot>(rawJson, _serializerSettings).Data;
        }
    }
}