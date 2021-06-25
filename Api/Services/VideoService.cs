using Api.Data;
using Api.Data.Entities.Parties;
using Api.Models.Constants;
using Api.Models.Enums;
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
        private readonly AppDbContext _dbContext;

        public VideoService(IUnitOfWork unitOfWork, ILogger<VideoService> logger, ISessionService sessionService, AppDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _sessionService = sessionService;
            _dbContext = dbContext;
        }
        public async Task<ApiResponse<ListResponse<VideoResponse>>> GetAllVideos(VideoFilterPayload payload, int skip = 0, int limit = 5)
        {
            var session = _sessionService.GetSession();
            if (session is null) return new ApiResponse<ListResponse<VideoResponse>>("User is not authenticated") { Code = ResponseCodes.Unauthenticated };


            if (!_sessionService.HasPermission(Permissions.CanViewVideos)) return new ApiResponse<ListResponse<VideoResponse>>("User does not have permission to view videos") { Code = ResponseCodes.NoPermission };


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


            if (!_sessionService.HasPermission(Permissions.CanViewVideos)) return new ApiResponse<VideoResponse>("User does not have permission to view videos") { Code = ResponseCodes.NoPermission };


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

        public async Task<ApiResponse<PriceCalculatorResponse>> CalculatePrice(PriceCalculatorPayload payload)
        {
            var session = _sessionService.GetSession();
            if (session is null) return new ApiResponse<PriceCalculatorResponse>("User is not authenticated") { Code = ResponseCodes.Unauthenticated };

            if (!_sessionService.HasPermission(Permissions.CanViewVideos)) return new ApiResponse<PriceCalculatorResponse>("User does not have permission to buy videos") { Code = ResponseCodes.NoPermission };

            var person = await _dbContext.Users.FindAsync(session.PersonaId);

            var titleExists = await _unitOfWork.VideoRepository.VideoExists(payload.Title);
            if (!titleExists) return new ApiResponse<PriceCalculatorResponse>("Title does not exist");

            var movie = await _unitOfWork.VideoRepository.GetByTitle(payload.Title);
            if (movie is null)
            {
                return new ApiResponse<PriceCalculatorResponse>("Error") { Code = ResponseCodes.NoData };
            }
            decimal cost;
            switch (movie.Type)
            {
                case VideoType.Regular:
                    cost = await RegularMoviePriceCalculator(payload.NumberOfDays, person.FirstName);
                    await persistToDb(payload.Title, cost, person.FirstName, payload.NumberOfDays);
                    return new ApiResponse<PriceCalculatorResponse>("") { Code = ResponseCodes.Success, Data = new PriceCalculatorResponse(payload.Title, cost) };

                case VideoType.ChildrenMovie:
                    if (payload.MaximumAge < 0) return new ApiResponse<PriceCalculatorResponse>("Please input maximum age");

                    cost = await ChildrenMoviePriceCalculator(payload.NumberOfDays, payload.MaximumAge, person.FirstName);
                    await persistToDb(payload.Title, cost, person.FirstName, payload.NumberOfDays);
                    return new ApiResponse<PriceCalculatorResponse>("") { Code = ResponseCodes.Success, Data = new PriceCalculatorResponse(payload.Title, cost) };

                case VideoType.NewRelease:
                    //accepts years between 2020 and 2040
                    if (payload.YearReleased < 2020 || payload.YearReleased > 2040) return new ApiResponse<PriceCalculatorResponse>("Please input a valid year");
                    cost = await NewReleaseMoviePriceCalculator(payload.NumberOfDays, payload.YearReleased, person.FirstName);
                    await persistToDb(payload.Title, cost, person.FirstName, payload.NumberOfDays);
                    return new ApiResponse<PriceCalculatorResponse>("") { Code = ResponseCodes.Success, Data = new PriceCalculatorResponse(payload.Title, cost) };
                default:
                    Console.WriteLine("Unknown input");
                    return new ApiResponse<PriceCalculatorResponse>("Please check video type.");
            }

        }

        private async Task persistToDb(string title, decimal cost, string firstName, int numberOfDays)
        {
            SearchHistory searchHistory = new SearchHistory
            {
                Title = title,
                Price = cost,
                UserFirstName = firstName,
                NumberOfDays = numberOfDays
            };
            var result = await _unitOfWork.SearchHistoryRepository.CreateAsync(searchHistory);

        }

        private async Task<Decimal> NewReleaseMoviePriceCalculator(int numberOfDays, int yearReleased, string FirstName)
        {
            var cost = StringConstants.NEW_RELEASE_RATE * numberOfDays - (yearReleased - 2020);
            return cost;
        }

        private async Task<Decimal> RegularMoviePriceCalculator(int numberOfDays, string FirstName)
        {
            var cost = StringConstants.REGULAR_RATE * numberOfDays;
            return cost;
        }

        private async Task<Decimal> ChildrenMoviePriceCalculator(int numberOfDays, int maxAge, string FirstName)
        {
            var cost = StringConstants.CHILDREN_RATE * numberOfDays + maxAge / 2;
            return cost;
        }
    }
    }
