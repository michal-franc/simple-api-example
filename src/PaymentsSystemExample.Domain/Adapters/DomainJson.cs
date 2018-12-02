using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaymentsSystemExample.Domain.Adapters
{
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

    public class SenderCharge
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
    }

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

        public Fx Fx { get; set;}

        [JsonProperty("charges_information")]
        public ChargesInformation ChargesInformation { get; set;}
    }
}