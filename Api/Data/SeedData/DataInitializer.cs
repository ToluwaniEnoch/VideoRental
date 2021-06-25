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
        public static void SeedData(UserManager<Persona> userManager, AppDbContext dbContext)
        {
            SeedRoles(dbContext).GetAwaiter().GetResult();
            SeedUsers(userManager, dbContext);
            SeedVideos(dbContext).GetAwaiter().GetResult();
        }

        private static void SeedUsers(UserManager<Persona> userManager, AppDbContext dbContext)
        {
            if (userManager.FindByEmailAsync("JoshuaKing@localmail.com").GetAwaiter().GetResult() == null)
            {
                Persona user = new Persona("JoshuaKing@localmail.com", "Joshua", "King");
                
                Console.WriteLine("creating seed data");
                var result = userManager.CreateAsync(user, "Password123#").GetAwaiter().GetResult();

               
                
                if (result.Succeeded)
                {
                    var role = dbContext.Roles.FirstOrDefaultAsync(c => c.Name == RoleConstants.Customer).GetAwaiter().GetResult();
                    var userRole = new IdentityUserRole<Guid> { UserId = user.Id, RoleId = role.Id} ;
                    dbContext.UserRoles.AddAsync(userRole);
                       
                    Console.WriteLine($"Customer Role ID is {role.Id} ");
                    var isSaved = dbContext.SaveChangesAsync().GetAwaiter().GetResult();
                }

               
            }
        }
        private async static Task SeedRoles(AppDbContext dbContext)
        {
            if (!await dbContext.Roles.AnyAsync(c => c.Name == RoleConstants.Customer));
            {
                var role = RoleConstants.GetDefaultTemplateRoles().First(x => x.Name == RoleConstants.Customer);

                dbContext.Roles.Add(role);
                var roleResult = await dbContext.SaveChangesAsync();

                Console.WriteLine("Role seeded.");
                
            }
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
