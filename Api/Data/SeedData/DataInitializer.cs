using Api.Data.Entities.Parties;
using Api.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Api.Data.SeedData
{
    public static class DataInitializer
    {
        public static void SeedData(AppDbContext dbContext)
        {            
            SeedVideos(dbContext).GetAwaiter().GetResult();
        }
        private async static Task SeedVideos(AppDbContext dbContext)
        {
            var result = await dbContext.Videos.CountAsync();
            Console.WriteLine(result);
                
            if (result < 5)
            {
                Video video1 = new Video { Title = "Lion King", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Drama, MaximumAge = 20 };
                Video video2 = new Video { Title = "Avengers", Type = VideoType.Regular, Genre = VideoGenre.Action };
                Video video3 = new Video { Title = "Local Hero", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Comedy, MaximumAge = 40 };
                Video video4 = new Video { Title = "Vikings", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Horror, MaximumAge = 30 };
                Video video5 = new Video { Title = "Romeo and Juliet", Type = VideoType.NewRelease, Genre = VideoGenre.Romance, YearReleased = 2021 };
                Video video6 = new Video { Title = "Lord of Rings", Type = VideoType.NewRelease, Genre = VideoGenre.Drama, YearReleased = 2022 };
                await dbContext.Videos.AddRangeAsync(video1, video2, video3, video4, video5, video6);
                
                Console.WriteLine("Videos seeded");
                var videoResult = await dbContext.SaveChangesAsync();
            }
        }
    }
}
