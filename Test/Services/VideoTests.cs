using Api.Data;
using Api.Data.Entities.Parties;
using Api.Models.Enums;
using Api.Models.Payloads;
using Api.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.Services
{
    public class VideoTests : BaseTest
    {
        [Fact]
        public async void ReturnAllVideos_ValidPayload_ReturnSuccess()
        {
            var collection = GetCollection().BuildServiceProvider();
            var videoService = collection.GetService<IVideoService>();
            var dbContext = collection.GetService<AppDbContext>();
            var video1 = new Video { Title = "Hobbits", Type = VideoType.Regular, Genre = VideoGenre.Action };
            var video2 = new Video { Title = "James the boy", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Comedy, MaximumAge = 30 };
            var video3 = new Video { Title = "Finisher", Type = VideoType.NewRelease, Genre = VideoGenre.Horror, YearReleased = 2027 };
            var video4 = new Video { Title = "Robots", Type = VideoType.Regular, Genre = VideoGenre.Horror };
            var video5 = new Video { Title = "Avatar", Type = VideoType.Regular, Genre = VideoGenre.Drama };
            var video6 = new Video { Title = "Makish", Type = VideoType.Regular, Genre = VideoGenre.Action };

            await dbContext.Videos.AddRangeAsync(video1, video2, video3, video4, video5, video6);
            await dbContext.SaveChangesAsync();
            var result = videoService.GetAllVideos(null, 0, 5);
            Assert.Equal(6, result.Result.Data.Total);
        }

        [Fact]
        public async void ReturnVideoById_ValidPayload_ReturnSuccess()
        {
            var collection = GetCollection().BuildServiceProvider();
            var videoService = collection.GetService<IVideoService>();
            var dbContext = collection.GetService<AppDbContext>();
            var video1 = new Video { Title = "Hobbits", Type = VideoType.Regular, Genre = VideoGenre.Action };
            var video2 = new Video { Title = "James the boy", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Comedy, MaximumAge = 30 };
            var video3 = new Video { Title = "Finisher", Type = VideoType.NewRelease, Genre = VideoGenre.Horror, YearReleased = 2027 };
            var video4 = new Video { Title = "Robots", Type = VideoType.Regular, Genre = VideoGenre.Horror };
            var video5 = new Video { Title = "Avatar", Type = VideoType.Regular, Genre = VideoGenre.Drama };
            var video6 = new Video { Title = "Makish", Type = VideoType.Regular, Genre = VideoGenre.Action };

            await dbContext.Videos.AddRangeAsync(video1, video2, video3, video4, video5, video6);
            await dbContext.SaveChangesAsync();
            var result = videoService.GetVideoByIdAsync(video1.Id);
            Assert.Equal(video1.Title, result.Result.Data.Title);
        }
    }
}
