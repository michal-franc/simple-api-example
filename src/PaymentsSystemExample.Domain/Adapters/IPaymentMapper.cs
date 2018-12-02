using System.Collections.Generic;

namespace PaymentsSystemExample.Domain.Adapters
{
    public interface IPaymentMapper
    {
        IEnumerable<ParsedPayment> Map(string rawData);
    }
}