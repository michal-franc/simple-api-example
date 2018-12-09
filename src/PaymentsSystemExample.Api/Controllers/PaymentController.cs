using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentsSystemExample.Domain.Adapters;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using PaymentsSystemExample.Api.Services;
using PaymentsSystemExample.Api.Extensions;

namespace PaymentsSystemExample.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this._paymentService = paymentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var guid = id.TryConvertIdToGuid();
            if(guid == default(Guid))
            { 
                return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            }

            var payment = await _paymentService.GetPayment(guid);

            if(payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        // TODO: return 422 for validation error
        // TODO: return 409 for duplicate error

        // Assumed here that with multiple payments I will discard whole batch if one payment is 'incorrect'
        // We only touch database if the whole batch is succesfull (I don't want to deal with transactions or compensating actions at the moment)
        // This forces user to resend whole batch
        // Alternative solution - create the ones that are ok and return information which ones were faulty
        // This lowers the amount sent on 'retry' request
        [HttpPut]
        public async Task<ActionResult> Put([FromBody]string content, [FromHeader(Name = "X-CultureCode")] string cultureCode)
        {
            if(string.IsNullOrWhiteSpace(cultureCode))
            {
                return BadRequest("X-CultureCode header missing.");
            }

            // TODO: return validation errors and display them
            // TODO: we need to verify if the version was not changed and if all validations pass
            var result = await _paymentService.CreatePayments(content, cultureCode);

            if(result.HasErrors)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok();
            }
        }
        //TODO!!: Request vcalidation errors on controller side
        //TODO!!: Domain validatione erors on domain side
        //TODO!!: you need to do parsing on the cotrnoller side
        //TODO!!: service should operate with domain objects not raw string
        //TODO!!: return 422 for validation error

        [HttpPost]
        public async Task<ActionResult> Post(string paymentsRawData, [FromHeader (Name = "X-CultureCode")]string cultureCode)
        {
            if(string.IsNullOrWhiteSpace(cultureCode))
            {
                return BadRequest("X-CultureCode header missing.");
            }

            // TODO: as with PUT - we need to verify if the version was not changed and if all validations pass
            // TODO: if it is not truth we discard whole request
            var result = await this._paymentService.UpdatePayments(paymentsRawData, cultureCode);

            if(result.HasErrors)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var guid = id.TryConvertIdToGuid();
            if(guid == default(Guid))
            { 
                // TODO: send this message in content negionated format Json or xml - depending on client
                // At the moment this is a plain text - not perfect
                return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            }

            if (await this._paymentService.DeletePayment(guid))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPatch]
        public ActionResult HttpPatch()
        {
            //TODO: implement if there is time :)
            return NotFound();
        }
    }
}
