using Api.Models.Constants;

namespace Api.Models.Responses
{
    public record StatusResponse(string? Message)
    {
        public int Code { get; set; } = ResponseCodes.Success;
    }
}