using System;
using System.Linq;
using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.Api.Services
{
    public interface IPaymentService
    {
        Payment GetPayment(Guid id);
        void UpdatePayment(Guid id, string value);
        void CreatePayment(Guid id);
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

        public void CreatePayment(Guid id)
        {
            this.InMemDB.Add(new Payment { Id = id });
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