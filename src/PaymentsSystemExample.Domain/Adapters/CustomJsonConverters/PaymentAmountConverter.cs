using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaymentsSystemExample.Domain.Adapters.CustomJsonConverters
{
    class PaymentAmountConverter: JsonConverter
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
}