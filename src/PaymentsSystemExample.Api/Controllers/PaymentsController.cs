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
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            this._paymentService = paymentService;
        }

        //TODO: Paging
        //TODO: filters?
        [HttpGet("{organisationId}")]
        public IActionResult Get(string organisationId)
        {
            var guid = organisationId.TryConvertIdToGuid();
            if(guid == default(Guid))
            { 
                // TODO: send this message in content negionated format Json or xml - depending on client
                // At the moment this is a plain text - not perfect
             return BadRequest($"Incorrect organisation id sent - '{organisationId}' -  Expected Guid format.");
            }

            throw new NotImplementedException();
        }
    }
}