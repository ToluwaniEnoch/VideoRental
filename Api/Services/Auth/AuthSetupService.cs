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
        private readonly UserManager<Persona> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IJwtTokenGenerator sessionTokenGenerator;


        public AuthSetupService(UserManager<Persona> userManager, IUnitOfWork unitOfWork, IJwtTokenGenerator sessionTokenGenerator)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.sessionTokenGenerator = sessionTokenGenerator;
            
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginPayload payload, CancellationToken ct = default)
        {
            var person = await userManager.FindByEmailAsync(payload.Email);

            if (person is null || (!await userManager.CheckPasswordAsync(person, payload.Password))) return new ApiResponse<LoginResponse>("Incorrect Login Credentials") { Code = ResponseCodes.Unauthenticated, Data = null };

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

        
    }
}