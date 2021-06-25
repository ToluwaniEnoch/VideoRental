using Api.Data;
using Api.Data.Entities.Parties;
using Api.Models.Constants;
using Api.Models.Enums;
using Api.Models.Payloads;
using Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test.Services.PriceCalculator
{
    public class PriceCalculatorTests : BaseTest
    {

        #region CalculatePrice_ POSITIVE TEST CASES
        [Fact]
        public async void CalculatePrice_RegularMovie_ValidPayload_ReturnSuccess()
        {
            var collection = GetCollection().BuildServiceProvider();
            var videoService = collection.GetService<IVideoService>();
            var dbContext = collection.GetService<AppDbContext>();
            var video = new Video { Title = "Hobbits", Type = VideoType.Regular, Genre = VideoGenre.Action };
            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();
            var pricePayload = new PriceCalculatorPayload { FirstName = "Toluwani", NumberOfDays = 5, Title = "Hobbits" };
            var result = videoService.CalculatePrice(pricePayload);
            Assert.Equal(50M, result.Result.Data.Cost);      
        }

        [Fact]
        public async void CalculatePrice_ChildrenMovie_ValidPayload_ReturnSuccess()
        {
            var collection = GetCollection().BuildServiceProvider();
            var videoService = collection.GetService<IVideoService>();
            var dbContext = collection.GetService<AppDbContext>();
            var video = new Video { Title = "Tom and Jerry", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Action, MaximumAge = 20 };
            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();
            var pricePayload = new PriceCalculatorPayload { FirstName = "Toluwani", NumberOfDays = 5, Title = "Tom and Jerry" };
            var result = videoService.CalculatePrice(pricePayload);
            Assert.Equal(50M, result.Result.Data.Cost);
        }

        [Fact]
        public async void CalculatePrice_NewRelease_ValidPayload_ReturnSuccess()
        {
            var collection = GetCollection().BuildServiceProvider();
            var videoService = collection.GetService<IVideoService>();
            var dbContext = collection.GetService<AppDbContext>();
            var video = new Video { Title = "Avengers", Type = VideoType.NewRelease, Genre = VideoGenre.Action, YearReleased = 2025 };
            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();
            var pricePayload = new PriceCalculatorPayload { FirstName = "Toluwani", NumberOfDays = 5, Title = "Avengers" };
            var result = videoService.CalculatePrice(pricePayload);
            Assert.Equal(71M, result.Result.Data.Cost);
        }
        #endregion

        #region CalculatePrice_NEGATIVE TEST CASES
        [Fact]
        public async void CalculatePrice_ChildrenMovie_InvalidPayload_NoMaximumAge_ReturnFailure()
        {
            var collection = GetCollection().BuildServiceProvider();
            var videoService = collection.GetService<IVideoService>();
            var dbContext = collection.GetService<AppDbContext>();
            var video = new Video { Title = "Tom and Jerry", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Action};
            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();
            var pricePayload = new PriceCalculatorPayload { FirstName = "Toluwani", NumberOfDays = 5, Title = "Tom and Jerry" };
            var result = videoService.CalculatePrice(pricePayload);
            Assert.Equal(ResponseCodes.NoData, result.Result.Code);
        }

        [Fact]
        public async void CalculatePrice_NewRelease_InvalidPayload_NoYearReleased_ReturnFailure()
        {
            var collection = GetCollection().BuildServiceProvider();
            var videoService = collection.GetService<IVideoService>();
            var dbContext = collection.GetService<AppDbContext>();
            var video = new Video { Title = "Tom and Jerry", Type = VideoType.NewRelease, Genre = VideoGenre.Action };
            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();
            var pricePayload = new PriceCalculatorPayload { FirstName = "Toluwani", NumberOfDays = 5, Title = "Tom and Jerry" };
            var result = videoService.CalculatePrice(pricePayload);
            Assert.Equal(ResponseCodes.NoData, result.Result.Code);
        }

        [Fact]
        public async void CalculatePrice_NewRelease_InvalidPayload_WrongTitle_ReturnFailure()
        {
            var collection = GetCollection().BuildServiceProvider();
            var videoService = collection.GetService<IVideoService>();
            var dbContext = collection.GetService<AppDbContext>();
            var video = new Video { Title = "Tom and Jerry", Type = VideoType.NewRelease, Genre = VideoGenre.Action, YearReleased = 2025 };
            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();
            var pricePayload = new PriceCalculatorPayload { FirstName = "Toluwani", NumberOfDays = 5, Title = "Big Bang Theory" };
            var result = videoService.CalculatePrice(pricePayload);
            Assert.Equal(ResponseCodes.BadRequest, result.Result.Code);
        }

        [Fact]
        public async void CalculatePrice_InvalidPayload_NoFirstName_ReturnFailure()
        {
            var collection = GetCollection().BuildServiceProvider();
            var videoService = collection.GetService<IVideoService>();
            var dbContext = collection.GetService<AppDbContext>();
            var video = new Video { Title = "Tom and Jerry", Type = VideoType.NewRelease, Genre = VideoGenre.Action, YearReleased = 2025 };
            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();
            var pricePayload = new PriceCalculatorPayload { NumberOfDays = 5, Title = "Big Bang Theory" };
            var result = videoService.CalculatePrice(pricePayload);
            Assert.Equal(ResponseCodes.NoData, result.Result.Code);
        }


        #endregion


    }
}