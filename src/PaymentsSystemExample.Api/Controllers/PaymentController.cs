using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using PaymentsSystemExample.Api.Services;

namespace PaymentsSystemExample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this._paymentService = paymentService;
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
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

            var payment = this._paymentService.GetPayment(guid);

            if(payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        // TODO: return 422 for validation error
        // TODO: return 409 for duplicate error

        [HttpPut("{id}")]
        // TODO: not void here but return a message succesfull or something + 200
        public ActionResult Put(string paymentsRawData)
        {
            // TODO: support for more payments than one
            // TODO: move this to action filter?
            // TODO: This should be checked in the Domain validation process
            // var guid = this.TryConvertIdToGuid(payment.Id);
            // if(guid == default(Guid))
            // { 
            //    return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            //}

            // TODO: with multiple payments? should we discard the whole request if one of them is failed? or return the ones that were successful and inform the ones that failed?
            // TODO: discard would require - transaction?
            // TODO: lack of dicards - lower the amount of data sent with retry as user doesnt have to sent it again but they need to deduplicate it
            // TODO: users would probably retry the whole package ... so i will assume dicards whole set
            // TODO: return validation errors and display them
            var result = this._paymentService.CreatePayment(paymentsRawData);

            if(result.HasErrors)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok();
            }
        }

        [HttpPost("{id}")]
        // TODO: not void here but return a message succesfull or something + 200
        public ActionResult Post(string id, [FromBody] string value)
        {
            // TODO: not found
            // TODO move this to action filter?
            var guid = this.TryConvertIdToGuid(id);
            if(guid == default(Guid))
            { 
                return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            }

            this._paymentService.UpdatePayment(guid, value);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            // TODO move this to action filter?
            var guid = this.TryConvertIdToGuid(id);
            if(guid == default(Guid))
            { 
                return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            }

            // TODO: not found

            if (this._paymentService.DeletePayment(guid))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
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
