using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.Domain.Adapters
{
    public interface IPaymentParser
    {
        IEnumerable<string> ParsingErrors { get; }
        bool HasErrors { get; }

        IEnumerable<Payment> Parse(string rawData, string cultureCode);
    }
}