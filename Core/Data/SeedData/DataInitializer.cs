using Api.Data.Entities.Account;
using Api.Models.Constants;
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
        }

        private static void SeedUsers(UserManager<Persona> userManager, AppDbContext dbContext)
        {
            if (userManager.FindByEmailAsync("mercyRae@localhost.com").GetAwaiter().GetResult() == null)
            {
                Persona user = new Persona("mercyRae@localhost.com", "mercy", "Ra");
                
                Console.WriteLine("creating seed data");
                var result = userManager.CreateAsync(user, "Password123#").GetAwaiter().GetResult();

               

                if (result.Succeeded)
                {
                    var role = dbContext.Roles.FirstOrDefaultAsync(c => c.Name == RoleConstants.Admin).GetAwaiter().GetResult();
                    var userRole = new IdentityUserRole<Guid> { UserId = user.Id, RoleId = role.Id} ;
                    dbContext.UserRoles.AddAsync(userRole);
                       
                    Console.WriteLine($"Admin Role ID is {role.Id} ");
                    var isSaved = dbContext.SaveChangesAsync().GetAwaiter().GetResult();
                }

               
            }
        }
        private async static Task SeedRoles(AppDbContext dbContext)
        {
            if (!await dbContext.Roles.AnyAsync(c => c.Name == RoleConstants.Admin));
            {
                var role = RoleConstants.GetDefaultTemplateRoles().First(x => x.Name == RoleConstants.Admin);

                dbContext.Roles.Add(role);
                var roleResult = await dbContext.SaveChangesAsync();

                Console.WriteLine("Role seeded.");
                
            }
        }
    }
}
