using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using PaymentsSystemExample.Domain.Adapters;

namespace PaymentsSystemExample.Api.Services
{
    public class ValidationErrors
    {
        private Dictionary<string, string> _errors;
        public IReadOnlyDictionary<string, string> Errors => _errors;
        public bool HasErrors => Errors.Count > 0;

        public ValidationErrors()
        {
            _errors = new Dictionary<string, string>();
        }

        public void Add(string attribute, string error)
        {
            _errors.Add(attribute, error);
        }
    }

    // This service is the layer where we do 
    // domain validations and if these are fine moving on to databse layer
    public interface IPaymentService
    {
        Task<Payment> GetPayment(Guid id);
        ValidationErrors UpdatePayments(string rawPaymentsData, string cultureCode);
        ValidationErrors CreatePayments(string rawPaymentsData, string cultureCode);
        Task<bool> DeletePayment(Guid id);
    }

    public class PaymentService : IPaymentService
    {
        private readonly List<Payment> InMemDB;
        private IPaymentParser _paymentParser;
        private IPaymentPersistenceService _paymentPersistenceService;

        public PaymentService(IPaymentParser paymentsParser, IPaymentPersistenceService paymentPersistenceService)
        {
            this.InMemDB = new List<Payment>();
            _paymentParser = paymentsParser;
            _paymentPersistenceService = paymentPersistenceService;
        }

        public async Task<Payment> GetPayment(Guid id)
        {
            return await _paymentPersistenceService.Get(id);
        }

        public IEnumerable<Payment> ListPayments()
        {
            throw new NotImplementedException();
        }

        public ValidationErrors UpdatePayments(string rawPaymentsData, string cultureCode)
        {
            return new ValidationErrors();
        }

        public ValidationErrors CreatePayments(string rawPaymentsData, string cultureCode)
        {
            var payments = _paymentParser.Parse(rawPaymentsData, cultureCode);

            if(_paymentParser.HasErrors)
            {
                return new ValidationErrors();
            }

            var payment = payments.First();
            this.InMemDB.Add(payment);
            return new ValidationErrors();
        }

        //TODO: organisation id has to be passed here so that we dont allow users to remove all the payments
        //TODO: this also needs to be encoded with token
        //TODO: Should use tombstone here and different worker for data removal (to give ability for user to revert action just in case)
        public async Task<bool> DeletePayment(Guid id)
        {
            if(await _paymentPersistenceService.Delete(id))
            {
                return true;
            }

            return false;
        }
    }
}