using Api.Data;
using Api.Data.Entities.Parties;
using Api.Models.Constants;
using Api.Models.Payloads;
using Api.Models.Responses;
using Api.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
    internal class VideoService : IVideoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VideoService> _logger;
        private readonly ISessionService _sessionService;

        public VideoService(IUnitOfWork unitOfWork, ILogger<VideoService> logger, ISessionService sessionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _sessionService = sessionService;
        }
        public async Task<ApiResponse<ListResponse<VideoResponse>>> GetAllVideos(VideoFilterPayload payload, int skip = 0, int limit = 20)
        {
            var session = _sessionService.GetSession();
            if (session is null) return new ApiResponse<ListResponse<VideoResponse>>("User is not authenticated") { Code = ResponseCodes.Unauthenticated };


            if (!_sessionService.HasPermission(Permissions.CanDoAnything)) return new ApiResponse<ListResponse<VideoResponse>>("User does not have permission to view videos") { Code = ResponseCodes.NoPermission };


            var videoQuery = _unitOfWork.VideoRepository.GetQuery();

            return new ApiResponse<ListResponse<VideoResponse>>("videos retrieved")
            {
                Data = new ListResponse<VideoResponse>(await videoQuery.LongCountAsync(), await videoQuery
                   .Skip(skip).Take(limit)

                   .Select(x => new VideoResponse(x.Id, x.Title, x.Type, x.Genre)).ToListAsync())

            };
        }

        public async Task<ApiResponse<VideoResponse>> GetVideoByIdAsync(Guid id)
        {
            var session = _sessionService.GetSession();
            if (session is null) return new ApiResponse<VideoResponse>("User is not authenticated") { Code = ResponseCodes.Unauthenticated };


            if (!_sessionService.HasPermission(Permissions.CanDoAnything)) return new ApiResponse<VideoResponse>("User does not have permission to view videos") { Code = ResponseCodes.NoPermission };


            var video = await _unitOfWork.VideoRepository.GetByIdAsync(id);

            if (video is null)
                return new ApiResponse<VideoResponse>("Video does not exist")
                {
                    Code = ResponseCodes.NoData,
                };

            return new ApiResponse<VideoResponse>("Video Retreived Successfully")
            {
                Code = ResponseCodes.Success,
                Data = new VideoResponse(video.Id, video.Title, video.Type, video.Genre)
            };
        }

        public async Task<ApiResponse<VideoResponse>> RegisterVideo(CreateVideoPayload payload)
        {
            var session = _sessionService.GetSession();
            if (session is null) return new ApiResponse<VideoResponse>("User is not authenticated") { Code = ResponseCodes.Unauthenticated };

            if (!_sessionService.HasPermission(Permissions.CanUploadVideos)) return new ApiResponse<VideoResponse>("User does not have permission to view videos") { Code = ResponseCodes.NoPermission };

            _logger.LogInformation($"Registration initiated for video Titled: {payload.Title}");

            if (string.IsNullOrEmpty(payload.Title))
            {
                return new ApiResponse<VideoResponse>("Title must be provided") { Code = ResponseCodes.NoData };
            }

            var videoExists = await _unitOfWork.VideoRepository.VideoExists(payload.Title);
            if (videoExists)
                return new ApiResponse<VideoResponse>("This title already exists in db")
                {
                    Code = ResponseCodes.ResourceAlreadyExist,
                };

            var video = new Video
            {
                Title = payload.Title, 
                Type = payload.Type, 
                Genre = payload.Genre 
            };

            await _unitOfWork.VideoRepository.CreateAsync(video);

            return new ApiResponse<VideoResponse>("")
            {
                Code = ResponseCodes.Success,
                Data = new VideoResponse(video.Id, video.Title, video.Type, video.Genre)

            };
        }
    }
    }
