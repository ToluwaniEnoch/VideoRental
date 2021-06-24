namespace Api.Models.Responses
{
    public record ApiResponse<TData>(string? Message) : StatusResponse(Message)
    {
        public TData? Data { get; init; }
    }
}