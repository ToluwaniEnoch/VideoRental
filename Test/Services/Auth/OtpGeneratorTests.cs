using Api;
using Api.Authentication;
using Api.Data;
using Api.Models.Internals;
using Api.Services.Auth;
using Api.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace Test.Services.Auth
{
    public class OtpGeneratorTests
    {
        private IServiceCollection GetCollection()
        {
            IServiceCollection service = new ServiceCollection();
            service.AddDbContext<AppDbContext>(ctx =>
            {
                ctx.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });
            service.InitCoreServicesAndRepositories();
            service.AddLogging();
            service.AddScoped(x =>
            {
                var moqObj = new Mock<IJwtTokenGenerator>();
                moqObj.Setup(c => c.GenerateToken(It.IsAny<Session>(), It.IsAny<TimeSpan>())).Returns(Guid.NewGuid().ToString);
                return moqObj.Object;
            });

            service.AddScoped(cx =>
            {
                var moqObj = new Mock<ISessionService>();
                moqObj.Setup(x => x.HasPermission(It.IsAny<string>())).Returns<string>(c => !string.IsNullOrEmpty(c));
                moqObj.Setup(x => x.GetSession()).Returns(new Session(Guid.NewGuid(), null, null, null, Guid.NewGuid()));
                return moqObj.Object;
            });

            return service;
        }

        [Fact]
        public void Test_OTP_Generate()
        {
            var collection = GetCollection().BuildServiceProvider();
            var service = collection.GetService<IOtpGenerator>();
            Assert.NotNull(service);

            var otp = service.Generate("aRandomKey");
            Assert.NotEmpty(otp);
        }

        [Fact]
        public void Test_OTP_Verify_Failed()
        {
            var collection = GetCollection().BuildServiceProvider();
            var service = collection.GetService<IOtpGenerator>();
            Assert.NotNull(service);

            var otp = service.Generate("aRandomKey");

            Assert.NotEmpty(otp);

            Assert.False(service.Verify("aWrongKey", otp));
        }

        [Fact]
        public void Test_OTP_Verify_Success()
        {
            var collection = GetCollection().BuildServiceProvider();
            var service = collection.GetService<IOtpGenerator>();
            Assert.NotNull(service);

            var otp = service.Generate("aRandomKey");

            Assert.NotEmpty(otp);

            Assert.True(service.Verify("aRandomKey", otp));
        }
    }
}