using System.Collections.Generic;

namespace PaymentsSystemExample.Domain.Adapters.JsonObjects
{
    public class PaymentRequestRoot
    {
        public List<PaymentRequestMetadata> Data { get; set; }
    }
}