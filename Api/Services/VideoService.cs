using Api.Data;
using Api.Data.Entities.Parties;
using Api.Models.Constants;
using Api.Models.Enums;
using Api.Models.Payloads;
using Api.Models.Responses;
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
        private readonly AppDbContext _dbContext;

        public VideoService(IUnitOfWork unitOfWork, ILogger<VideoService> logger, AppDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _dbContext = dbContext;
        }
        public async Task<ApiResponse<ListResponse<VideoResponse>>> GetAllVideos(VideoFilterPayload payload, int skip = 0, int limit = 5)
        {
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
            var video = await _unitOfWork.VideoRepository.GetByIdAsync(id);

            if (video is null)
                return new ApiResponse<VideoResponse>("Video does not exist")
                {
                    Code = ResponseCodes.BadRequest,
                };

            return new ApiResponse<VideoResponse>("Video Retreived Successfully")
            {
                Code = ResponseCodes.Success,
                Data = new VideoResponse(video.Id, video.Title, video.Type, video.Genre)
            };
        }

        public async Task<ApiResponse<VideoResponse>> RegisterVideo(CreateVideoPayload payload)
        {
            
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
                Genre = payload.Genre, 
                YearReleased = payload.YearReleased,
                MaximumAge = payload.MaximumAge
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
            if (payload.Title is null || payload.FirstName is null) return new ApiResponse<PriceCalculatorResponse>("Title and First name input cannot be empty") { Code = ResponseCodes.NoData};
            

            var titleExists = await _unitOfWork.VideoRepository.VideoExists(payload.Title);
            if (!titleExists) return new ApiResponse<PriceCalculatorResponse>("Title does not exist") { Code = ResponseCodes.BadRequest };

            var movie = await _unitOfWork.VideoRepository.GetByTitle(payload.Title);
            if (movie is null)
            {
                return new ApiResponse<PriceCalculatorResponse>("Service Error") { Code = ResponseCodes.BadRequest };
            }
            decimal cost;
            switch (movie.Type)
            {
                case VideoType.Regular:
                    cost = await RegularMoviePriceCalculator(payload.NumberOfDays, payload.FirstName);
                    await persistToDb(payload.Title, cost, payload.FirstName, payload.NumberOfDays);
                    return new ApiResponse<PriceCalculatorResponse>("") { Code = ResponseCodes.Success, Data = new PriceCalculatorResponse(payload.Title, cost) };

                case VideoType.ChildrenMovie:
                    if (movie.MaximumAge < 1) return new ApiResponse<PriceCalculatorResponse>("Please input maximum age") { Code = ResponseCodes.NoData};

                    cost = await ChildrenMoviePriceCalculator(payload.NumberOfDays, movie.MaximumAge, payload.FirstName);
                    await persistToDb(payload.Title, cost, payload.FirstName, payload.NumberOfDays);
                    return new ApiResponse<PriceCalculatorResponse>("") { Code = ResponseCodes.Success, Data = new PriceCalculatorResponse(payload.Title, cost) };

                case VideoType.NewRelease:
                    //accepts years between 2020 and 2040
                    if (movie.YearReleased < 2020 || movie.YearReleased > 2040) return new ApiResponse<PriceCalculatorResponse>("Please input a valid year") { Code = ResponseCodes.NoData };
                    cost = await NewReleaseMoviePriceCalculator(payload.NumberOfDays, movie.YearReleased, payload.FirstName);
                    await persistToDb(payload.Title, cost, payload.FirstName, payload.NumberOfDays);
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
            var cost = StringConstants.NEW_RELEASE_RATE * numberOfDays - (yearReleased - 2021);
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
