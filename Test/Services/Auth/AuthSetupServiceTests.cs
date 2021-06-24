using Api.Data;
using Api.Data.Entities.Account;
using Api.Models.Constants;
using Api.Models.Internals;
using Api.Models.Payloads;
using Api.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.Helpers;
using Xunit;

namespace Test.Services.Auth
{
    public class AuthSetupServiceTests : BaseTest
    {
        [Fact]
        public void Test_Login_Invalid()
        {
            var collection = GetCollection().BuildServiceProvider();
            var service = collection.GetService<IAuthSetupService>();
            var rsp = service?.LoginAsync(new LoginPayload("dev@innovantics.com", "Password123#" )).Result;
            Assert.NotEqual(ResponseCodes.Success, rsp?.Code);
        }

        [Fact]
        public void Test_Login_Invalid_Password_Credential()
        {
            var collection = GetCollection().BuildServiceProvider();
            var userManager = collection.GetService<UserManager<Persona>>();
            _ = userManager?.CreateAsync(new Persona("dev@innovantics.com", "Jethro", "Daniel") {Email = "dev@innovantics.com" }, "Password123#").Result;
            var service = collection.GetService<IAuthSetupService>();
            var rsp = service?.LoginAsync(new LoginPayload("jethro.daniel@innovantics.com", "xy1234" )).Result;
            Assert.NotEqual(ResponseCodes.Success, rsp?.Code);
        }

        [Fact]
        public async Task Test_Login_Password_Successful()
        {
            var collection = GetCollection().BuildServiceProvider();
            var userManager = collection.GetService<UserManager<Persona>>();
            var roleManager = collection.GetService<RoleManager<Role>>();
            Assert.NotNull(roleManager);
            Assert.NotNull(userManager);
            Persona person = new Persona("dev@innovantics.com", "Aman", "Sulaiman"){PhoneNumber = "+2348064398491" };
            if (userManager is not null)
            {
                var result = await userManager.CreateAsync(person, "Password123#");
                Assert.True(result.Succeeded);
            }

            var appDbContext = collection.GetService<AppDbContext>();
            Assert.NotNull(appDbContext);

            var role = new Role { Name = "Admin", Permissions = new List<string> { } };
            if (roleManager is not null && userManager is not null)
            {
                await roleManager.CreateAsync(role);
                await userManager.AddToRoleAsync(person, role.Name);
            }
            if (appDbContext is not null)
            {
                await appDbContext.SaveChangesAsync();
            }
            var service = collection.GetService<IAuthSetupService>();
            var rsp = service?.LoginAsync(new LoginPayload ("dev@innovantics.com", "Password123#")).GetAwaiter().GetResult();
            Assert.Equal(ResponseCodes.Success, rsp?.Code);
        }

        [Fact]
        public async Task Test_SetPassword()
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
            var user = new Persona("dev@innovantics.com", "Jethro", "Daniel") { PhoneNumber = "+2348064398491" };
            var xy = await userManager.CreateAsync(user, "Password123#");
            Assert.True(xy.Succeeded);

            var appDbContext = collection.GetService<AppDbContext>();
            var role = new Role { Name = "Admin", Permissions = new List<string> { } };
            await roleManager.CreateAsync(role);
            await userManager.AddToRoleAsync(user, role.Name);
            await appDbContext.SaveChangesAsync();
            testRef.GetRef = () => new Session(user.Id, user.FirstName, user.Email, role.Name, Guid.NewGuid());
            var service = collection.GetService<IAuthSetupService>();
            var result = service.SetPasswordAsync(new ChangePasswordPayload ("Password123#", "Password101#") { ConfirmPassword = "Password101#" }).Result;
            Assert.Equal(ResponseCodes.Success, result.Code);
            var rsp = await service.LoginAsync(new LoginPayload("dev@innovantics.com", "Password101#"));
            Assert.Equal(ResponseCodes.Success, rsp.Code);
        }

        [Fact]
        public async Task Test_SetPassword_UseDefaultPasswordAsOld()
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
            var user = new Persona("dev@innovantics.com", "Jethro", "Daniel") { PhoneNumber = "+2348064398491" };
            var xy = await userManager.CreateAsync(user, "Password123#");
            Assert.True(xy.Succeeded);

            var appDbContext = collection.GetService<AppDbContext>();
            var role = new Role { Name = "Admin", Permissions = new List<string> { } };
            await roleManager.CreateAsync(role);
            await userManager.AddToRoleAsync(user, role.Name);

            await appDbContext.SaveChangesAsync();
            testRef.GetRef = () => new Session(user.Id, user.FirstName, user.Email, role.Name, Guid.NewGuid());
            var service = collection.GetService<IAuthSetupService>();
            var result = service.SetPasswordAsync(new ChangePasswordPayload("Password123#", "Password453#") { ConfirmPassword = "Password453#" }).Result;
            Assert.Equal(ResponseCodes.Success, result.Code);
            var rsp = service.LoginAsync(new LoginPayload ("dev@innovantics.com", "Password453#")).Result;
            Assert.Equal(ResponseCodes.Success, rsp.Code);
        }

        [Fact]
        public async Task Test_SetPassword_IncorrectOldPassword()
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
            var user = new Persona("dev@innovantics.com", "Jethro", "Daniel") { PhoneNumber = "+2348064398491" };
            var xy = await userManager.CreateAsync(user, "Password123#");
            Assert.True(xy.Succeeded);

            var appDbContext = collection.GetService<AppDbContext>();
            var role = new Role { Name = "Admin", Permissions = new List<string> { } };
            await appDbContext.Roles.AddAsync(role);

            await appDbContext.SaveChangesAsync();
            testRef.GetRef = () => new Session(user.Id, user.FirstName, user.Email, role.Name, Guid.NewGuid());
            var service = collection.GetService<IAuthSetupService>();
            var result = service.SetPasswordAsync(new ChangePasswordPayload("Password101#", "Password123#") { ConfirmPassword = "Password101#" }).Result;
            Assert.Equal(49, result.Code);
            Assert.Equal("Incorrect Old Password", result.Message);
        }
    }
}