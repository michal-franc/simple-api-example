using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.Domain.Adapters
{
    public interface IPaymentMapper
    {
        IEnumerable<Payment> Map(string rawData);
    }
}