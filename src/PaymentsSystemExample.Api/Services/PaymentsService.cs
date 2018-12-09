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
        private List<string> _parsingErrors;
        private Dictionary<string, string> _domainErrors;
        public IEnumerable<string> ParsingErrors => _parsingErrors;
        public IReadOnlyDictionary<string, string> DomainErrors => _domainErrors;
        public bool HasErrors => _parsingErrors.Count > 0 || _domainErrors.Count > 0;

        public ValidationErrors()
        {
            _parsingErrors= new List<string>();
            _domainErrors = new Dictionary<string, string>();
        }

        public void AddParsingError(string error)
        {
            _parsingErrors.Add(error);
        }
        
        public void AddDomainError(string attribute, string error)
        {
            _domainErrors.Add(attribute, error);
        }
    }

    // This service is the layer where we do 
    // domain validations and if these are fine moving on to databse layer
    public interface IPaymentService
    {
        Task<Payment> GetPayment(Guid id);
        ValidationErrors UpdatePayments(string rawPaymentsData, string cultureCode);
        Task<ValidationErrors> CreatePayments(string rawPaymentsData, string cultureCode);
        Task<bool> DeletePayment(Guid id);
    }

    public class PaymentService : IPaymentService
    {
        private IPaymentParser _paymentParser;
        private IPaymentPersistenceService _paymentPersistenceService;

        public PaymentService(IPaymentParser paymentsParser, IPaymentPersistenceService paymentPersistenceService)
        {
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

        public async Task<ValidationErrors> CreatePayments(string rawPaymentsData, string cultureCode)
        {
            var validationErrors = new ValidationErrors();

            var payments = _paymentParser.Parse(rawPaymentsData, cultureCode);

            if(_paymentParser.HasErrors)
            {
                foreach(var parsingError in _paymentParser.ParsingErrors)
                {
                    validationErrors.AddParsingError(parsingError);
                }
            }

            foreach(var payment in payments)
            {
                //var domainErrors = payment.Validate();
                //foreach(var domainError in domainErrors)
                //{
                //   validationErrors.AddDomainError(domainError.paymentId, domainError.attribute, domainError.error);
                //}
            }

            if(!validationErrors.HasErrors)
            {
                _paymentPersistenceService.Create(payments);
            }

            return validationErrors;
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