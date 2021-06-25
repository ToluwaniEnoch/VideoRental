using Api.Data;
using Api.Data.Entities.Account;
using Api.Models.Constants;
using Api.Models.Enums;
using Api.Models.Internals;
using Api.Models.Payloads;
using Api.Services;
using Api.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Helpers;
using Xunit;

namespace Test.Services.PriceCalculator
{
    public class PriceCalculatorTests : BaseTest
    {

        #region CalculatePrice
        [Fact]
        public async void CalculatePrice_ValidPayload_ReturnSuccess()
        {
            TestRef<Session> testRef = new TestRef<Session>();

            var collection = GetCollection().AddTransient(xy =>
            {
                var moqObj = new Mock<ISessionService>();
                moqObj.Setup(x => x.HasPermission(It.IsAny<string>())).Returns<string>(c => !string.IsNullOrEmpty(c));
                moqObj.Setup(x => x.GetSession()).Returns(testRef.GetRef);
                return moqObj.Object;
            }).BuildServiceProvider();

            var userManager = collection.GetService<UserManager<Persona>>();
            var roleManager = collection.GetService<RoleManager<Role>>();
            Assert.NotNull(roleManager);
            var user = new Persona("johnjacobs@localhost.com", "John", "Jacobs");
            if (userManager is not null)
            {
                var creation = await userManager.CreateAsync(user, "Password123#");
                Assert.True(creation.Succeeded);
            }
            var appDbContext = collection.GetService<AppDbContext>();
            Assert.NotNull(appDbContext);

            var role = new Role { Name = "Admin", Permissions = new List<string> { } };
            if (roleManager is not null && userManager is not null)
            {
                await roleManager.CreateAsync(role);
                await userManager.AddToRoleAsync(user, role.Name);
            }
            if (appDbContext is not null)
            {
                await appDbContext.SaveChangesAsync();
            }

            testRef.GetRef = () => new Session(user.Id, user.FirstName, user.Email, role.Name, role.Id);


            var loginService = collection.GetService<IAuthSetupService>();
            var rsp = loginService?.LoginAsync(new LoginPayload("johnjacobs@localhost.com", "Password123#")).GetAwaiter().GetResult();
            

            var collectionService = collection.GetService<IVideoService>();
            var video = new CreateVideoPayload { Title = "Hobbits", Type = VideoType.Regular, Genre = VideoGenre.Action };
            var register = collectionService.RegisterVideo(video).Result;
            await appDbContext.SaveChangesAsync();
            
            var service = collection.GetService<IPriceCalculatorService>();
            var result = service.CalculatePrice(new PriceCalculatorPayload { Title = "Hobbits", NumberOfDays = 2 }).Result;
            Assert.Equal(ResponseCodes.Success, result.Code);
            Assert.Equal(20M, result.Data.Cost);


        }
        #endregion
    }
}