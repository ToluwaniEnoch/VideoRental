using Api.Data;
using Api.Data.Entities.Parties;
using Api.Models.Constants;
using Api.Models.Enums;
using Api.Models.Payloads;
using Api.Models.Responses;
using Api.Services.Auth;
using System;
using System.Threading.Tasks;

namespace Api.Services
{
    internal class PriceCalculatorService : IPriceCalculatorService
    {
        private readonly ISessionService _sessionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _dbContext;

        public PriceCalculatorService(ISessionService sessionService, IUnitOfWork unitOfWork, AppDbContext dbContext)
        {
            _sessionService = sessionService;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }
        public async Task<ApiResponse<PriceCalculatorResponse>> CalculatePrice(PriceCalculatorPayload payload)
        {
            var session = _sessionService.GetSession();
            if (session is null) return new ApiResponse<PriceCalculatorResponse>("User is not authenticated") { Code = ResponseCodes.Unauthenticated };

            if (!_sessionService.HasPermission(Permissions.CanDoAnything)) return new ApiResponse<PriceCalculatorResponse>("User does not have permission to buy videos") { Code = ResponseCodes.NoPermission };

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
                    return new ApiResponse<PriceCalculatorResponse>("") { Data = new PriceCalculatorResponse(payload.Title, cost)};
                     
                case VideoType.ChildrenMovie:
                    if (payload.MaximumAge < 0) return new ApiResponse<PriceCalculatorResponse>("Please input maximum age");                    
                    
                    cost = await ChildrenMoviePriceCalculator(payload.NumberOfDays, payload.MaximumAge, person.FirstName);
                    await persistToDb(payload.Title, cost, person.FirstName, payload.NumberOfDays);
                    return new ApiResponse<PriceCalculatorResponse>("") { Data = new PriceCalculatorResponse(payload.Title, cost) };
                    
                case VideoType.NewRelease:
                    //accepts years between 2020 and 2040
                    if (payload.YearReleased < 2020 || payload.YearReleased > 2040) return new ApiResponse<PriceCalculatorResponse>("Please input a valid year");
                    cost = await NewReleaseMoviePriceCalculator(payload.NumberOfDays, payload.YearReleased, person.FirstName);
                    await persistToDb(payload.Title, cost, person.FirstName, payload.NumberOfDays);
                    return new ApiResponse<PriceCalculatorResponse>("") { Data = new PriceCalculatorResponse(payload.Title, cost) };
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
