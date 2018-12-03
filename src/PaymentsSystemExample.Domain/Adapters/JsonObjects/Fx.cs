using Newtonsoft.Json;

namespace PaymentsSystemExample.Domain.Adapters.JsonObjects
{
    public class Fx
    {
        [JsonProperty("contract_reference")]
        public string ContractReference { get; set; }

        [JsonProperty("exchange_rate")]
        public string ExchangeRate { get; set; }

        [JsonProperty("original_amount")]
        public string OriginalAmount { get; set; }

        [JsonProperty("original_currency")]
        public string OriginalCurency { get; set; }
    }
}