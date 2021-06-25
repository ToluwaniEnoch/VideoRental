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

    public class VideoController : BaseController
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        /// <summary>
        /// Adds Video to the db. 
        /// Please note that Titles are unique.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<VideoResponse>>> PostRegister([FromBody] CreateVideoPayload payload)
        {
            var result = await _videoService.RegisterVideo(payload);
            return HandleResponse(result);
        }

        /// <summary>
        /// Returns a video with a specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<VideoResponse>>> GetVideoById(Guid id)
        {
            var result = await _videoService.GetVideoByIdAsync(id);
            return HandleResponse(result);
        }

        /// <summary>
        /// Returns all videos, 5 per page
        /// You can increase number of videos per page by increasing the value for limit
        /// Note that video type and Video Genre are enum
        /// Video type: 0:Regular , 1: Children Movie 2: New Release
        /// Video Genre: 0: Action, 1: Drama, 2: Romance, 3: Comedy, 4: Horror
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ListResponse<VideoResponse>>>> GetVideos([FromQuery] VideoFilterPayload payload, [FromQuery] int skip = 0, [FromQuery] int limit = 5)
        {
            var result = await _videoService.GetAllVideos(payload, skip, limit);
            return HandleResponse(result);
        }

        /// <summary>
        /// Returns the cost of renting a movie
        /// Note that price depends on number of days and type of movie
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost("cost")]        
        public async Task<ActionResult<ApiResponse<PriceCalculatorResponse>>> PostCalculate([FromBody] PriceCalculatorPayload payload)
        {
            var result = await _videoService.CalculatePrice(payload);
            return HandleResponse(result);
        }
    }
}
