using Newtonsoft.Json;

namespace PaymentsSystemExample.Domain.Adapters.JsonObjects
{
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
}