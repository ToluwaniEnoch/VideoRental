using Api.Models.Payloads;
using Api.Models.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Auth
{
    public interface IAuthSetupService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginPayload payload, CancellationToken ct = default);

    }
}