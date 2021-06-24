using Api.Authentication;
using Api.Data.Entities.Account;
using Api.Models.Constants;
using Api.Models.Internals;
using Api.Models.Payloads;
using Api.Services.Auth;
using Api.Services.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace Test.Services.Auth
{
    public class AuthRecoverServiceTests : BaseTest
    {
        private IServiceCollection GetLocalCollection()
        {
            IServiceCollection serviceCollection = GetCollection();

            serviceCollection.AddScoped(x =>
            {
                var moqObj = new Mock<IJwtTokenGenerator>();
                moqObj.Setup(c => c.GenerateToken(It.IsAny<Session>(), It.IsAny<TimeSpan>())).Returns(Guid.NewGuid().ToString);
                return moqObj.Object;
            });

            serviceCollection.AddScoped(cx =>
            {
                var moqObj = new Mock<ISessionService>();
                moqObj.Setup(x => x.HasPermission(It.IsAny<string>())).Returns<string>(c => !string.IsNullOrEmpty(c));
                moqObj.Setup(x => x.GetSession()).Returns(new Session(Guid.NewGuid(), "Aman", "Aman", "Aman", Guid.NewGuid()));
                return moqObj.Object;
            });

            return serviceCollection;
        }

        [Fact]
        public void Test_Forgot_Password_Not_Found()
        {
            var collection = GetLocalCollection().BuildServiceProvider();
            var service = collection.GetService<IAuthRecoverService>();
            var userManager = collection.GetService<UserManager<Persona>>();
            _ = userManager?.CreateAsync(new Persona("dev@innovantics.com", "Dev", "Innovantics"), "1234567").Result;
            var result = service?.ForgotPasswordAsync(new ForgotPasswordPayload("dev@innovantics.com")).Result;
            Assert.NotEqual(ResponseCodes.Success, result?.Code);
        }

        [Fact]
        public void Test_Forgot_Password_Success()
        {
            var collection = GetLocalCollection().BuildServiceProvider();
            var service = collection.GetService<IAuthRecoverService>();
            var userManager = collection.GetService<UserManager<Persona>>();
            var r = userManager.CreateAsync(new Persona("dev@innovantics.com", "Jethro", "Daniel") { PhoneNumber = "+2348064398491" }, "Password123#").Result;
            Assert.True(r.Succeeded);
            var result = service.ForgotPasswordAsync(new ForgotPasswordPayload("dev@innovantics.com")).Result;
            Assert.Equal(ResponseCodes.Success, result.Code);
        }

        [Fact]
        public void Test_ResetPassword_InvalidToken()
        {
            var Otp = "123456";
            var collection = GetLocalCollection().AddTransient<IOtpGenerator>(xc =>
            {
                var mock = new Mock<IOtpGenerator>();
                mock.Setup(x => x.Generate(It.IsAny<string>(), It.IsAny<int>(), 6)).Returns(Guid.NewGuid().ToString);
                mock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 6)).Returns<string, string, int, int>((key, token, e, f) => token == Otp);
                return mock.Object;
            }).BuildServiceProvider();
            var service = collection.GetService<IAuthRecoverService>();
            var userManager = collection.GetService<UserManager<Persona>>();
            _ = userManager.CreateAsync(new Persona("jethro.daniel@innovantics.com", "Jethro", "Daniel") { PhoneNumber = "+2348064398491" }, "1234567").Result;

            var result = service.ResetPasswordAsync(new ResetPasswordPayload("dev@innovantics.com", "123457", "newPassword") { ConfirmPassword = "newPassword" }).Result;
            Assert.NotEqual(ResponseCodes.Success, result.Code);
        }

        [Fact]
        public void Test_ResetPassword_Success()
        {
            var Otp = "123456";
            var collection = GetLocalCollection().AddTransient<IOtpGenerator>(xc =>
            {
                var mock = new Mock<IOtpGenerator>();
                mock.Setup(x => x.Generate(It.IsAny<string>(), It.IsAny<int>(), 6)).Returns(Guid.NewGuid().ToString);
                mock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 6)).Returns<string, string, int, int>((key, token, e, f) => token == Otp);
                return mock.Object;
            }).BuildServiceProvider();
            var service = collection.GetService<IAuthRecoverService>()!;
            var userManager = collection.GetService<UserManager<Persona>>()!;
            var r = userManager.CreateAsync(new Persona("dev@innovantics.com", "Jethro", "Daniel") { PhoneNumber = "+2348064398491" }, "Password123#").Result;
            Assert.True(r.Succeeded);
            var result = service.ResetPasswordAsync(new ResetPasswordPayload("dev@innovantics.com", "123456", "newPassword") { ConfirmPassword = "newPassword" }).Result;
            Assert.Equal(ResponseCodes.Success, result.Code);
        }
    }
}