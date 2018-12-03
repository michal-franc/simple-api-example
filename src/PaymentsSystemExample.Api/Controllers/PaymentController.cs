using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.Api.Controllers
{
    public static class PaymentService
    {
        private static List<Payment> InMemDB;

        static PaymentService()
        {
            InMemDB = new List<Payment>();
        }

        public static Payment GetPayment(string id)
        {
            return InMemDB.Find(x => x.Id == id);
        }

        public static void CreatePayment(string id)
        {
            InMemDB.Add(new Payment { Id = id });
        }

        public static void DeletePayment(string id)
        {
            InMemDB.Remove(InMemDB.Where(x => x.Id == id).Single());
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<Payment> Get(string id)
        {
            var payment = PaymentService.GetPayment(id);

            if(payment == null)
            {
                return NotFound();
            }

            return payment;
        }

        [HttpPut("{id}")]
        public void Put(string id/*, [FromBody] string value*/)
        {
            PaymentService.CreatePayment(id);
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            PaymentService.DeletePayment(id);
        }
    }
}
