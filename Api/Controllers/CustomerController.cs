using Api.Models.Responses;
using Api.Models.Payloads;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Api.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService) : base()
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CustomerResponse>>> PostRegister([FromBody] CreateCustomerPayload payload)
        {
            var result = await _customerService.RegisterAsync(payload);
            return HandleResponse(result);
        }      


    }
}
