using System;
using System.Linq;
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

    public interface IPaymentService
    {
        Payment GetPayment(Guid id);
        ValidationErrors UpdatePayments(string rawPaymentsData, string cultureCode);
        ValidationErrors CreatePayments(string rawPaymentsData, string cultureCode);
        bool DeletePayment(Guid id);
    }

    public class PaymentService : IPaymentService
    {
        private readonly List<Payment> InMemDB;
        private IPaymentParser _paymentParser;

        public PaymentService(IPaymentParser paymentsParser)
        {
            this.InMemDB = new List<Payment>();
            _paymentParser = paymentsParser;
        }

        public Payment GetPayment(Guid id)
        {
            return this.InMemDB.Find(x => x.Id == id);
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