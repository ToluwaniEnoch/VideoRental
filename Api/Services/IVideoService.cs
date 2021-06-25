using Api.Models.Payloads;
using Api.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
    public interface IVideoService
    {
        Task<ApiResponse<VideoResponse>> RegisterVideo(CreateVideoPayload payload);
        Task<ApiResponse<VideoResponse>> GetVideoByIdAsync(Guid id);
        Task<ApiResponse<ListResponse<VideoResponse>>> GetAllVideos(VideoFilterPayload payload, int skip = 0, int limit = 5);
        Task<ApiResponse<PriceCalculatorResponse>> CalculatePrice(PriceCalculatorPayload payload);

    }
}
