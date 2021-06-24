using Api.Models.Payloads;
using Api.Models.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Auth
{
    public interface IAuthRecoverService
    {
        Task<ApiResponse<StatusResponse>> ForgotPasswordAsync(ForgotPasswordPayload payload, CancellationToken ct = default);

        Task<ApiResponse<StatusResponse>> ResetPasswordAsync(ResetPasswordPayload payload, CancellationToken ct = default);
    }
}