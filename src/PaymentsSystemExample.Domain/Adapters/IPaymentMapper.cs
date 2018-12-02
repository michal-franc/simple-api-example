using System.Collections.Generic;

namespace PaymentsSystemExample.Domain.Adapters
{
    public interface IPaymentMapper
    {
        IEnumerable<RequestMetadata> Map(string rawData);
    }
}