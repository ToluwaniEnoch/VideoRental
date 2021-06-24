using Api.Authentication;
using Api.Data;
using Api.Data.Entities.Account;
using Api.Models.Constants;
using Api.Models.Internals;
using Api.Models.Payloads;
using Api.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Auth
{
    internal class AuthSetupService : IAuthSetupService
    {
        private readonly ILogger<AuthRecoverService> logger;
        private readonly UserManager<Persona> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly ISessionService sessionService;
        private readonly IJwtTokenGenerator sessionTokenGenerator;
        private readonly IHostEnvironment hostEnvironment;
        private readonly IDistributedCache distributedCache;
        private readonly RoleManager<Role> roleManager;

        public AuthSetupService(ILogger<AuthRecoverService> logger, UserManager<Persona> userManager, IUnitOfWork unitOfWork, ISessionService sessionService,
            IJwtTokenGenerator sessionTokenGenerator, IHostEnvironment hostEnvironment, IDistributedCache distributedCache, RoleManager<Role> roleManager)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.sessionService = sessionService;
            this.sessionTokenGenerator = sessionTokenGenerator;
            this.hostEnvironment = hostEnvironment;
            this.distributedCache = distributedCache;
            this.roleManager = roleManager;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginPayload payload, CancellationToken ct = default)
        {
            var person = await userManager.FindByEmailAsync(payload.Email);

            if (person is null || (!await userManager.CheckPasswordAsync(person, payload.Password))) return new ApiResponse<LoginResponse>("Incorrect Login Credentials") { Code = ResponseCodes.Unauthenticated, Data = null };
            logger.LogInformation($"User {person.FirstName} has been authenticated");

            var role = await unitOfWork.RoleRepository.GetUserRoleAsync(person.Id, ct);

            if (role is null) return new ApiResponse<LoginResponse>("Cannot find user role") { Code = ResponseCodes.NoData, Data = null };

            return new ApiResponse<LoginResponse>(null) { Data = new LoginResponse(ConstructTokenFromSession(person, role)) };
        }

        private string ConstructTokenFromSession(Persona person, Role role)
        {
            var session = new Session(person.Id, $"{person.FirstName} {person.LastName}", person.Email, role.Name, role.Id);
            var expiryTime = TimeSpan.FromHours(6);
            var token = sessionTokenGenerator.GenerateToken(session, expiryTime);
            return token;
        }

        public async Task<ApiResponse<StatusResponse>> SetPasswordAsync(ChangePasswordPayload payload, CancellationToken ct = default)
        {
            var session = sessionService.GetSession();
            if (session is null) return new ApiResponse<StatusResponse>("User must be authenticated") { Code = ResponseCodes.Unauthenticated };
            var person = await userManager.FindByIdAsync(session.PersonaId.ToString());
            if (person is null) return new ApiResponse<StatusResponse>("User Account Not found") { Code = ResponseCodes.NoData };
            logger.LogInformation($"{person.FirstName} is attempting to set password");

            var checkOldPassword = await userManager.CheckPasswordAsync(person, payload.OldPassword);

            if (!checkOldPassword) return new ApiResponse<StatusResponse>("Incorrect Old Password") { Code = ResponseCodes.Unauthenticated };

            var removeResult = await userManager.RemovePasswordAsync(person);
            if (!removeResult.Succeeded)
            {
                logger.LogInformation($"Unable to remove password for user{person.Email}");
                return new ApiResponse<StatusResponse>(removeResult.Errors?.FirstOrDefault()?.Description) { Code = ResponseCodes.ServiceError };
            }

            var result = await userManager.AddPasswordAsync(person, payload.Password);
            if (!result.Succeeded)
            {
                logger.LogInformation($"Unable to set password for user {person.Email}");
                return new ApiResponse<StatusResponse>(result.Errors?.FirstOrDefault()?.Description) { Code = ResponseCodes.ServiceError };
            }
            return new ApiResponse<StatusResponse>("Password reset complete");
        }
    }
}