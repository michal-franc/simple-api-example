using System;
using System.Collections.Generic;
using Xunit;

namespace PaymentsSystemExample.UnitTests
{
    public class Forex 
    {
        public string Reference { get; }
        public decimal ExchangeRate { get; }
        public decimal OriginalAmount { get; }
        public string OriginalCurrency { get; }
    }

    public class Charge
    {
        public decimal Amount { get; }
        public string Currency { get; }
    }

    public class Charges
    {
        public string BearerCode { get; }

        public IEnumerable<Charge> ListOfCharges { get; }
    }

    public class Account 
    {
        public string Name { get; }
        public string Number { get; }
        public string NumberCode { get; }
        public string Type { get; }
    }

    public class Scheme
    {
        public string SubType { get; }
        public string Type { get; }
    }

    public class Party 
    {
        public string Name { get; }
        public Account Account { get; }
        public string Address { get; }
        public string BankdId { get; }
        public string BankdIdCode { get; }
    }
    
    public class Payment 
    {
        public Guid Id { get; }
        public decimal Amount { get; }

        public Party Beneficiary { get; }
        public Party Debtor { get; }
        public Party Sponsor { get; }

        public string Currency { get; }
        public string E2EReference { get; }
        public string Purpose { get; }
        public string Scheme { get; }
        public string Type { get; }
        public DateTime ProcessingDate { get; }

        public Payment(decimal amount)
        {
            this.Amount = amount;
        }
    }

    public static class TestPaymentBuilder
    {
        public static Payment Create()
        {
            return new Payment(10.0m);
        }
    }

    public class PaymentsDomainTests
    {
        [Fact]
        public void TestPaymentBuilding()
        {
            var payment = TestPaymentBuilder.Create();
            Assert.True(payment.Amount == 10.0m);
        }
    }
}
