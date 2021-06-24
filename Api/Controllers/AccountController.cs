//using Api.Data.Entities.Account;
//using Api.Models.Payloads;
//using Api.Models.Responses;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Api.Controllers
//{
//    public class AccountController : BaseController
//    {
//        private SignInManager<Persona> _signManager;
//        private readonly ILogger<AccountController> _logger;
//        private UserManager<Persona> _userManager;

//        public AccountController(UserManager<Persona> userManager, SignInManager<Persona> signManager, ILogger<AccountController> logger)
//        {
//            _userManager = userManager;
//            _signManager = signManager;
//            _logger = logger;
//        }
//        [HttpPost]
//        public async Task<ActionResult<ApiResponse<AccountResponse>>> PostRegister([FromBody] CreateAccountPayload payload)
//        {
//            _logger.LogInformation($"User Registration initiated for {payload.FirstName} {payload.LastName}.");
//            var result = await _accountService.RegisterAsync(payload);
//            return HandleResponse(result);
//        }
//    }
//}
