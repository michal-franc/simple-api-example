using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaymentsSystemExample.Domain.Adapters.JsonObjects
{
    public class Attributes
    {
        [JsonProperty("payment_id")]
        public string Id { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("processing_date")]
        public DateTime ProcessingDate { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("end_to_end_reference")]
        public string E2EReference { get; set; }

        [JsonProperty("numeric_reference")]
        public string NumericReference { get; set; }

        [JsonProperty("payment_purpose")]
        public string Purpose { get; set; }

        [JsonProperty("payment_scheme")]
        public string Scheme { get; set; }

        [JsonProperty("payment_type")]
        public string Type { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("scheme_payment_type")]
        public string SchemeType { get; set; }

        [JsonProperty("scheme_payment_sub_type")]
        public string SchemeSubType { get; set; }

        [JsonProperty("beneficiary_party")]
        public Party Beneficiary {get; set; }

        [JsonProperty("sponsor_party")]
        public Party Sponsor {get; set; }

        [JsonProperty("debtor_party")]
        public Party Debtor {get; set; }

        public Fx Fx { get; set;}

        [JsonProperty("charges_information")]
        public ChargesInformation ChargesInformation { get; set;}
    }
}