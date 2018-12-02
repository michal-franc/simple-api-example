using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaymentsSystemExample.Domain.Adapters
{
    // Used http://json2csharp.com/ to quickly generate object
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

    public class Fx
    {
        public string contract_reference { get; set; }
        public string exchange_rate { get; set; }
        public string original_amount { get; set; }
        public string original_currency { get; set; }
    }

    public class Attributes
    {
        public ChargesInformation charges_information { get; set; }
        public Fx fx { get; set; }
    }

    public class RequestMetadata
    {
        public string type { get; set; }
        public string id { get; set; }
        public int version { get; set; }
        public string organisation_id { get; set; }

        [JsonProperty("attributes")]
        public Payment PaymentInJson { get; set; }
    }

    public class RequestRoot
    {
        public List<RequestMetadata> Data { get; set; }
    }

    public class Party
    {
        // We could create a potential Account class here 
        // for simplicity I left it like this

        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("account_number")]
        public string AccountNumber { get; set; }

        [JsonProperty("account_number_code")]
        public string AccountNumberCode { get; set; }

        [JsonProperty("account_type")]
        public int AccountType { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("bank_id")]
        public string BankId { get; set; }

        [JsonProperty("bank_id_code")]
        public string BankIdCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Payment
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
    }
}