using Api;
using Api.Authentication;
using Api.Data;
using Api.Models.Constants;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Reflection;
using Test.Helpers;

namespace Test
{
    public abstract class BaseTest
    {
        private static IServiceCollection InitializeCollection()
        {
            IServiceCollection service = new ServiceCollection();

            service.InitCoreServicesAndRepositories();
            service.ConfigureIdentity();
            service.AddLogging();

            service.AddMediatR(Assembly.GetExecutingAssembly());

            service.AddDistributedMemoryCache();

            service.AddTransient(xy =>
            {
                var mock = new Mock<IHostEnvironment>();
                mock.SetupGet(x => x.EnvironmentName).Returns("Development");
                return mock.Object;
            });

            SqlMapper.AddTypeHandler(new GuidAsStringHandler());

            return service;
        }

        protected IServiceCollection GetCollection()
        {
            var service = InitializeCollection();
            
            var dbName = Guid.NewGuid().ToString();

            service.AddDbContext<AppDbContext>(ctx =>
            {
                ctx.UseInMemoryDatabase(dbName);
            });
            service.AddTransient(xy =>
            {
                var moqObj = new Mock<IConfiguration>();
                var moqSection = new Mock<IConfigurationSection>();
                moqSection.SetupGet(x => x.Value).Returns(StringConstants.JwtKeyDefault);
                moqObj.Setup(x => x[StringConstants.JwtKey]).Returns("123456");                
                moqObj.Setup(x => x.GetSection(StringConstants.JwtKey)).Returns(moqSection.Object);                
                return moqObj.Object;
            });
            return service;
        }
    }
}