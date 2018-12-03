using Newtonsoft.Json;

namespace PaymentsSystemExample.Domain.Adapters.JsonObjects
{
    public class PaymentRequestMetadata
    {
        public string type { get; set; }
        public string id { get; set; }
        public int version { get; set; }
        public string organisation_id { get; set; }

        [JsonProperty("attributes")]
        public Payment PaymentInJson { get; set; }
    }
}