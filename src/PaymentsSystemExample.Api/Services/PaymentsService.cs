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
        bool DeletePayment(Guid id);
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
            try
            {
                return await _paymentPersistenceService.Get(id);
            }
            catch (System.Exception)
            {
                throw;
            }
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

        // TODO: should we support bulk delete?
        public bool DeletePayment(Guid id)
        {
            if(InMemDB.Any(x => x.Id == id))
            {
                this.InMemDB.Remove(InMemDB.Where(x => x.Id == id).Single());
                return true;
            }

            return false;
        }
    }
}