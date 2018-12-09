using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.Api.Services
{
    public interface IPaymentPersistenceService
    {
        Task<Payment> Get(Guid paymentid);
        Task<IEnumerable<Payment>> List(Guid organisationId);
        Task<bool> Delete(Guid paymentid);
        Task<bool> Create(IEnumerable<Payment> payments);
        Task<bool> Update(IEnumerable<Payment> payments);
    }
}