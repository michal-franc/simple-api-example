using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var guid = id.TryConvertIdToGuid();
            if(guid == default(Guid))
            { 
                return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            }

            try
            {
                var payment = await _paymentService.GetPayment(guid);

                if(payment == null)
                {
                    return NotFound();
                }

                return Ok(payment);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error on Get");
                return StatusCode(500);
            }
        }

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

            try
            {
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
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error on Put");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(string paymentsRawData, [FromHeader (Name = "X-CultureCode")]string cultureCode)
        {
            if(string.IsNullOrWhiteSpace(cultureCode))
            {
                return BadRequest("X-CultureCode header missing.");
            }

            try
            {
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
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error on Post");
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var guid = id.TryConvertIdToGuid();
            if(guid == default(Guid))
            { 
                return BadRequest($"Incorrect payment id sent - '{id}' -  Expected Guid format.");
            }

            try
            {
                if (await this._paymentService.DeletePayment(guid))
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error on Delete");
                return StatusCode(500);
            }
        }

        [HttpPatch]
        public ActionResult HttpPatch()
        {
            //TODO: implement if there is time :) - oh there definitely won't be time to do it
            return NotFound();
        }
    }
}
