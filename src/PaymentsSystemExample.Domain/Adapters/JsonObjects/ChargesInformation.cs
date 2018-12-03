using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaymentsSystemExample.Domain.Adapters.JsonObjects
{
    public class ChargesInformation
    {
        [JsonProperty("bearer_code")]
        public string BearerCode { get; set; }

        [JsonProperty("sender_charges")]
        public List<SenderCharge> SenderCharges { get; set; }

        [JsonProperty("receiver_charges_amount")]
        public string ReceiverChargesAmount { get; set; }

        [JsonProperty("receiver_charges_currency")]
        public string ReceiverChargesCurrency { get; set; }
    }
}