using System;
using System.Linq;
using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.Api.Services
{
    public class ValidationErrors
    {
        public IReadOnlyDictionary<string, string> Errors { get; set; }
        public bool HasErrors => Errors.Count > 0;

        public ValidationErrors()
        {
            this.Errors = new Dictionary<string, string>();
        }
    }

    public interface IPaymentService
    {
        Payment GetPayment(Guid id);
        void UpdatePayment(Guid id, string value);
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

        public void UpdatePayment(Guid id, string value)
        {
            var payment = InMemDB.Find(x => x.Id == id);
            this.InMemDB.Remove(payment);
            payment.Attributes.Currency = value;
            this.InMemDB.Add(payment);
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