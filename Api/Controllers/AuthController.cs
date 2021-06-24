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
        private readonly IAuthRecoverService recoverService;
        private readonly IAuthSetupService authSetupService;
        /// <summary>
        /// Authentication Controller public constructor
        /// </summary>
        /// <param name="recoverService"></param>
        /// <param name="authSetupService"></param>
        public AuthController(IAuthRecoverService recoverService, IAuthSetupService authSetupService)
        {
            this.recoverService = recoverService;
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

        [HttpPost("setpassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ApiResponse<StatusResponse>>> PostSetPassword([FromBody] ChangePasswordPayload payload, CancellationToken ct = default)
        {
            var result = await authSetupService.SetPasswordAsync(payload, ct);
            return HandleResponse(result);
        }

        [HttpPost("forgotpassword")]
        public async Task<ActionResult<ApiResponse<StatusResponse>>> PostForgotPassword([FromBody] ForgotPasswordPayload payload, CancellationToken ct = default)
        {
            var result = await recoverService.ForgotPasswordAsync(payload, ct);
            return HandleResponse(result);
        }

        [HttpPost("resetpassword")]
        public async Task<ActionResult<ApiResponse<StatusResponse>>> PostResetPassword([FromBody] ResetPasswordPayload payload, CancellationToken ct = default)
        {
            var result = await recoverService.ResetPasswordAsync(payload, ct);
            return HandleResponse(result);
        }
    }
}