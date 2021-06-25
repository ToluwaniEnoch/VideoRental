using Api.Data.Entities.Account;
using Api.Data.Entities.Parties;
using Api.Models.Constants;
using Api.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Video video1 = new Video { Title = "Lion King", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Drama };
                Video video2 = new Video { Title = "Avengers", Type = VideoType.Regular, Genre = VideoGenre.Action };
                Video video3 = new Video { Title = "Local Hero", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Comedy };
                Video video4 = new Video { Title = "Vikings", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Horror };
                Video video5 = new Video { Title = "Romeo and Juliet", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Romance };
                Video video6 = new Video { Title = "Lord of Rings", Type = VideoType.ChildrenMovie, Genre = VideoGenre.Drama };
                await dbContext.Videos.AddRangeAsync(video1, video2, video3, video4, video5, video6);
                
                Console.WriteLine("Videos seeded");
                var videoResult = await dbContext.SaveChangesAsync();
            }
        }
    }
}
