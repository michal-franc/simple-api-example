using System;
using System.Linq;
using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

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
        //TODO: pluralise
        ValidationErrors UpdatePayment(string paymentsRawData);
        //TODO: pluralise
        ValidationErrors CreatePayment(string paymentsRawData);
        bool DeletePayment(Guid id);
    }

    public class PaymentService : IPaymentService
    {
        private readonly List<Payment> InMemDB;

        public PaymentService()
        {
            this.InMemDB = new List<Payment>();
        }

        public Payment GetPayment(Guid id)
        {
            return this.InMemDB.Find(x => x.Id == id);
        }

        public ValidationErrors UpdatePayment(string paymentsRawData)
        {
            return new ValidationErrors();
        }

        public ValidationErrors CreatePayment(string paymentsRawData)
        {
            var payment = new Payment();
            this.InMemDB.Add(payment);
            return new ValidationErrors();
        }

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