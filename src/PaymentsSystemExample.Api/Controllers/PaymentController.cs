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

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<Payment> Get(string id)
        {
            // TODO move this to action filter?
            var guid = this.TryConvertIdToGuid(id);
            if(guid == default(Guid))
            { 
                // TODO: send this message in content negionated format
                // Json or xml - depending on client
                // At the moment this is a plain text
                return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            }

            var payment = PaymentService.GetPayment(guid);

            if(payment == null)
            {
                return NotFound();
            }

            return payment;
        }

        [HttpPut("{id}")]
        // TODO: not void here but return a message succesfull or something + 200
        public ActionResult Put(string id/*, [FromBody] string value*/)
        {
            // TODO move this to action filter?
            var guid = this.TryConvertIdToGuid(id);
            if(guid == default(Guid))
            { 
                return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            }

            PaymentService.CreatePayment(guid);

            return Ok();
        }

        [HttpPost("{id}")]
        // TODO: not void here but return a message succesfull or something + 200
        public ActionResult Post(string id, [FromBody] string value)
        {
            // TODO move this to action filter?
            var guid = this.TryConvertIdToGuid(id);
            if(guid == default(Guid))
            { 
                return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            }

            PaymentService.UpdatePayment(guid, value);

            return Ok();
        }

        [HttpDelete("{id}")]
        // TODO: not void here but return a message succesfull or something + 200
        public ActionResult Delete(string id)
        {
            // TODO move this to action filter?
            var guid = this.TryConvertIdToGuid(id);
            if(guid == default(Guid))
            { 
                return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            }

            PaymentService.DeletePayment(guid);

            return Ok();
        }

        private Guid TryConvertIdToGuid(string id) 
        {
            Guid outGuid;

            if(Guid.TryParse(id, out outGuid))
            {
                return outGuid;
            }

            return default(Guid);
        }
    }
}
