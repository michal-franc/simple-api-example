using System;
using Newtonsoft.Json;

namespace PaymentsSystemExample.Domain.Adapters.JsonObjects
{
    public class Payment
    {
        public string Type { get; set; }
        public Guid Id { get; set; }
        public int Version { get; set; }

        [JsonProperty("organisation_id")]
        public string OrganisationId { get; set; }

        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }
    }
}