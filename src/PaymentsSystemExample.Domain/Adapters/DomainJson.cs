using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaymentsSystemExample.Domain.Adapters
{
    // Used http://json2csharp.com/ to quickly generate object
    public class BeneficiaryParty
    {
        public string account_name { get; set; }
        public string account_number { get; set; }
        public string account_number_code { get; set; }
        public int account_type { get; set; }
        public string address { get; set; }
        public string bank_id { get; set; }
        public string bank_id_code { get; set; }
        public string name { get; set; }
    }

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

    public class DebtorParty
    {
        public string account_name { get; set; }
        public string account_number { get; set; }
        public string account_number_code { get; set; }
        public string address { get; set; }
        public string bank_id { get; set; }
        public string bank_id_code { get; set; }
        public string name { get; set; }
    }

    public class Fx
    {
        public string contract_reference { get; set; }
        public string exchange_rate { get; set; }
        public string original_amount { get; set; }
        public string original_currency { get; set; }
    }

    public class SponsorParty
    {
        public string account_number { get; set; }
        public string bank_id { get; set; }
        public string bank_id_code { get; set; }
    }

    public class Attributes
    {
        public decimal amount { get; set; }
        public BeneficiaryParty beneficiary_party { get; set; }
        public ChargesInformation charges_information { get; set; }
        public string currency { get; set; }
        public DebtorParty debtor_party { get; set; }
        public string end_to_end_reference { get; set; }
        public Fx fx { get; set; }
        public string numeric_reference { get; set; }
        public string payment_id { get; set; }
        public string payment_purpose { get; set; }
        public string payment_scheme { get; set; }
        public string payment_type { get; set; }
        public DateTime processing_date { get; set; }
        public string reference { get; set; }
        public string scheme_payment_sub_type { get; set; }
        public string scheme_payment_type { get; set; }
        public SponsorParty sponsor_party { get; set; }
    }

    public class RequestMetadata
    {
        public string type { get; set; }
        public string id { get; set; }
        public int version { get; set; }
        public string organisation_id { get; set; }

        [JsonProperty("attributes")]
        public PaymentInJson PaymentInJson { get; set; }
    }

    public class RequestRoot
    {
        public List<RequestMetadata> data { get; set; }
    }

    // this will be used to make read only casting + removing Json requirement
    // Could potentialy moved to separate project that is detailing domain
    // And json could be moved to project json adapter
    public interface IPayment
    {
        string Id { get; }
        decimal Amount { get; }
        DateTime ProcessingDate { get; }
        string Currency { get; }
        string E2EReference { get; }
        string NumericReference { get; }
        string Purpose { get; }
        string Scheme { get; }
        string Type { get; }
        string Reference { get; }
        string SchemeType { get; }
        string SchemeSubType { get; }
    }

    // This is read and write class just for Json.NET be able to write to props
    // It will be casted to IPayment to hide the setters
    public class PaymentInJson : IPayment
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
    }
}