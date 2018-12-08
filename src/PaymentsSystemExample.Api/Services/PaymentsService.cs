using System;
using System.Linq;
using System.Collections.Generic;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.Api.Services
{
    public static class PaymentService
    {
        private static List<Payment> InMemDB;

        static PaymentService()
        {
            InMemDB = new List<Payment>();
        }

        public static Payment GetPayment(Guid id)
        {
            return InMemDB.Find(x => x.Id == id);
        }

        public static void UpdatePayment(Guid id, string value)
        {
            var payment = InMemDB.Find(x => x.Id == id);
            InMemDB.Remove(payment);
            payment.Attributes.Currency = value;
            InMemDB.Add(payment);
        }

        public static void CreatePayment(Guid id)
        {
            InMemDB.Add(new Payment { Id = id });
        }

        public static void DeletePayment(Guid id)
        {
            InMemDB.Remove(InMemDB.Where(x => x.Id == id).Single());
        }
    }
}