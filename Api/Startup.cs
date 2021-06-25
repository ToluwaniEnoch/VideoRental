using Api.Data;
using Api.Data.Entities.Account;
using Api.Models.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Api
{
    public class Startup
    {
        private const string AllowOrigins = "AllowOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configure DI services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //DB Setup
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(StringConstants.ConnectionString));

            //Cors Setup
            var origins = Configuration[AllowOrigins];
            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy(AllowOrigins, builder =>
                    builder.WithOrigins(origins.Split(","))
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            //Infrastructure
            services.AddDistributedMemoryCache();
            services.AddHealthChecks();
            //Framework
            services.AddControllers();
            services.AddHttpContextAccessor();
            //App Services
            services.InitCoreServicesAndRepositories();
            //Documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = StringConstants.API_TITTLE,
                    Description = StringConstants.API_DESCRIBTION,
                    Contact = new OpenApiContact
                    {
                        Name = StringConstants.DEVELOPER_NAME,
                        Email = StringConstants.DEVELOPER_EMAIL,
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
        /// <summary>
        /// Configure middleware
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext dbContext)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(AllowOrigins);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            if (env.IsDevelopment() || env.IsStaging())
            {
                Api.Data.SeedData.DataInitializer.SeedData(dbContext);
            }
        }


        
        
    }
}