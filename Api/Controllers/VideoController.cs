using Api.Models.Payloads;
using Api.Models.Responses;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class VideoController : BaseController
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<VideoResponse>>> PostRegister([FromBody] CreateVideoPayload payload)
        {
            var result = await _videoService.RegisterVideo(payload);
            return HandleResponse(result);
        }

        [HttpGet("id")]
        public async Task<ActionResult<ApiResponse<VideoResponse>>> GetVideoById(Guid id)
        {
            var result = await _videoService.GetVideoByIdAsync(id);
            return HandleResponse(result);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ListResponse<VideoResponse>>>> GetVideos([FromQuery] VideoFilterPayload payload, [FromQuery] int skip = 0, [FromQuery] int limit = 20)
        {
            var result = await _videoService.GetAllVideos(payload, skip, limit);
            return HandleResponse(result);
        }
    }
}
