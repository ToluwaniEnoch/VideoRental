using Api.Models.Payloads;
using Api.Models.Responses;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class PriceCalculatorController : BaseController
    {
        private readonly IPriceCalculatorService _priceCalculatorService;

        public PriceCalculatorController(IPriceCalculatorService priceCalculatorService) : base()
        {
            _priceCalculatorService = priceCalculatorService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PriceCalculatorResponse>>> PostCalculate([FromBody] PriceCalculatorPayload payload)
        {
            var result = await _priceCalculatorService.CalculatePrice(payload);
            return HandleResponse(result);
        }
    }
}
