using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaymentsSystemExample.Domain.Adapters.CustomJsonConverters
{
    class ProcessingDateConverter: JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(DateTime) || objectType == typeof(DateTime?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                DateTime value = default(DateTime);

                // Allowing only YYYY-MM-DD format
                // As we are forcing specific format we can use InvariantCulture
                if(DateTime.TryParseExact(token.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                {
                    return value;
                }

                throw new JsonSerializationException("Incorrect decimal format: " + token.ToString());
            }

            if (token.Type == JTokenType.Null && objectType == typeof(DateTime?))
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