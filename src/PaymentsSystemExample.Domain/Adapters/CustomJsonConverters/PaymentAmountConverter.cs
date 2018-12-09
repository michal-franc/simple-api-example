using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaymentsSystemExample.Domain.Adapters.CustomJsonConverters
{
    public class PaymentAmountConverter: JsonConverter
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

            throw new JsonSerializationException($"Not supported token type: {token.Type.ToString()} with value: {token.ToString()}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();

            if(type == typeof(decimal) || type == typeof(decimal?))
            {
                JToken token = JToken.FromObject(value.ToString());
                token.WriteTo(writer);
                return;
            }

            throw new JsonSerializationException($"PaymentAmountConverter used on unsupported field type expected: 'decimal' or 'decimal?' got: {type}");
        }
    }
}