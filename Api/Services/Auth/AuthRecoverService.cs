using Api.Data.Entities.Account;
using Api.Models.Constants;
using Api.Models.Payloads;
using Api.Models.Responses;
using Api.Services.Auth.Notifications;
using Api.Services.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Auth
{
    internal class AuthRecoverService : IAuthRecoverService
    {
        private readonly ILogger<AuthRecoverService> logger;
        private readonly IOtpGenerator otpGenerator;
        private readonly UserManager<Persona> userManager;
        private readonly IMediator mediator;

        public AuthRecoverService(ILogger<AuthRecoverService> logger, IOtpGenerator otpGenerator, UserManager<Persona> userManager, IMediator mediator)
        {
            this.logger = logger;
            this.otpGenerator = otpGenerator;
            this.userManager = userManager;
            this.mediator = mediator;
        }

        public async Task<ApiResponse<StatusResponse>> ForgotPasswordAsync(ForgotPasswordPayload payload, CancellationToken ct = default)
        {
            var person = await userManager.FindByEmailAsync(payload.Email);
            if (person is null) return new ApiResponse<StatusResponse>("Email not found") { Code = ResponseCodes.NoData };

            var otp = otpGenerator.Generate(person.Id.ToString(), 10);
            logger.LogInformation("Forgot password initiated for {0} {1} with token {2}", person.FirstName, person.LastName, otp);
            _ = mediator.Publish(new PersonForgotPasswordNotification { Person = person, Otp = otp });
            return new ApiResponse<StatusResponse>("Forgot Password initiated");
        }

        public async Task<ApiResponse<StatusResponse>> ResetPasswordAsync(ResetPasswordPayload payload, CancellationToken ct = default)
        {
            var person = await userManager.FindByEmailAsync(payload.Email);
            if (person is null) return new ApiResponse<StatusResponse>("Email not found") { Code = ResponseCodes.NoData };
            logger.LogInformation("Reset password initiated by {}", person.Email);

            if (!otpGenerator.Verify(person.Id.ToString(), payload.Token, 10)) return new ApiResponse<StatusResponse>("Token is invalid") { Code = ResponseCodes.InvalidToken };

            var removeResult = await userManager.RemovePasswordAsync(person);
            if (!removeResult.Succeeded)
            {
                logger.LogInformation($"Unable to remove password for user{person.Email}");
                return new ApiResponse<StatusResponse>(removeResult.Errors.FirstOrDefault()?.Description) { Code = ResponseCodes.ServiceError };
            }

            var result = await userManager.AddPasswordAsync(person, payload.NewPassword);
            if (!result.Succeeded)
            {
                logger.LogInformation($"Unable to set password for user {person.Email}");
                return new ApiResponse<StatusResponse>(removeResult.Errors?.FirstOrDefault()?.Description) { Code = ResponseCodes.ServiceError };
            }

            logger.LogInformation($"Password reset complete for user {person.Email}");
            _ = mediator.Publish(new PersonResetPasswordNotification { Person = person });
            return new ApiResponse<StatusResponse>("Password reset complete");
        }
    }
}