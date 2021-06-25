using Api.Models.Payloads;
using Api.Models.Responses;
using Api.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Authentication Controller
    /// </summary>
    [ApiController]
    [Route("/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthSetupService authSetupService;
        /// <summary>
        /// Authentication Controller public constructor
        /// </summary>
        /// <remarks>A customer account has been seeded.
        /// Please login with 
        /// Email:  JoshuaKing@localmail.com 
        /// Password: Password123#
        /// </remarks>
        /// <param name="authSetupService"></param>
        public AuthController(IAuthSetupService authSetupService)
        {
            this.authSetupService = authSetupService;
        }
        /// <summary>
        /// Login endpoint
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="ct"></param>
        /// <returns>Token if successful or Error Message if not</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> PostLogin([FromBody] LoginPayload payload, CancellationToken ct = default)
        {
            var result = await authSetupService.LoginAsync(payload, ct);
            return HandleResponse(result);
        }

       
    }
}