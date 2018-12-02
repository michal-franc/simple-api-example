using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.Domain.Adapters
{
    public interface IPaymentMapper
    {
        IEnumerable<PaymentRequestMetadata> Map(string rawData);
    }
}