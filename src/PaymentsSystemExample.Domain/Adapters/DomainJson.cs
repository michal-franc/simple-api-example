using System;
using System.Collections.Generic;

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
        public Attributes attributes { get; set; }
    }

    public class RequestRoot
    {
        public List<RequestMetadata> data { get; set; }
    }

    public class ParsedPayment
    {
        public decimal Amount { get; }
        public DateTime ProcessingDate { get; }

        public ParsedPayment(Attributes attributes)
        {
            this.Amount = attributes.amount;
            this.ProcessingDate = attributes.processing_date;
        }
    }
}