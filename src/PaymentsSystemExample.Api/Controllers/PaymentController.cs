﻿using System;
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

            this._paymentService.CreatePayment(guid);

            return Ok();
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
