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
        private Dictionary<string, Dictionary<string, string>> _domainErrors;
        public IEnumerable<string> ParsingErrors => _parsingErrors;

        // I could use IReadOnlyDictionary here due to limitatation of .NET generics
        public IReadOnlyDictionary<string, Dictionary<string,string>> DomainErrors => _domainErrors;
        public bool HasErrors => _parsingErrors.Count > 0 || _domainErrors.Count > 0;

        public ValidationErrors()
        {
            _parsingErrors= new List<string>();
            _domainErrors = new Dictionary<string, Dictionary<string, string>>();
        }

        public void AddParsingError(string error)
        {
            _parsingErrors.Add(error);
        }
        
        public void AddDomainError(string objectId, string attribute, string error)
        {
            if(!_domainErrors.ContainsKey(objectId))
            {
                _domainErrors.Add(objectId, new Dictionary<string,string>());
            }
            _domainErrors[objectId].Add(attribute, error);
        }
    }

    // This service is the layer where we do 
    // domain validations and if these are fine moving on to databse layer
    public interface IPaymentService
    {
        Task<Payment> GetPayment(Guid id);
        Task<IEnumerable<Payment>> GetPayments(Guid organisationId);
        Task<ValidationErrors> UpdatePayments(string rawPaymentsData, string cultureCode);
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

        public async Task<IEnumerable<Payment>> GetPayments(Guid organisationId)
        {
            return await _paymentPersistenceService.List(organisationId);
        }

        public async Task<ValidationErrors> UpdatePayments(string rawPaymentsData, string cultureCode)
        {
            var payments = _paymentParser.Parse(rawPaymentsData, cultureCode);

            var validationErrors = CheckValidationErrors(payments);
            if(!validationErrors.HasErrors)
            {
                await _paymentPersistenceService.Update(payments);
            }

            return validationErrors;
        }

        public async Task<ValidationErrors> CreatePayments(string rawPaymentsData, string cultureCode)
        {
            var payments = _paymentParser.Parse(rawPaymentsData, cultureCode);

            var validationErrors = CheckValidationErrors(payments);
            if(!validationErrors.HasErrors)
            {
                await _paymentPersistenceService.Create(payments);
            }

            return validationErrors;
        }

        private ValidationErrors CheckValidationErrors(IEnumerable<Payment> payments)
        {
            var validationErrors = new ValidationErrors();

            if(_paymentParser.HasErrors)
            {
                foreach(var parsingError in _paymentParser.ParsingErrors)
                {
                    validationErrors.AddParsingError(parsingError);
                }
            }

            foreach(var payment in payments)
            {
                var domainErrors = payment.Validate();
                foreach(var domainError in domainErrors)
                {
                   validationErrors.AddDomainError(payment.Id.ToString(), domainError.Key, domainError.Value);
                }
            }

            return validationErrors;
        }

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