using System.Collections.Generic;

namespace PaymentsSystemExample.Domain.Adapters.JsonObjects
{
    public class PaymentRoot
    {
        public List<Payment> Data { get; set; }
    }
}