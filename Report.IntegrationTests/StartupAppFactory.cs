using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Report.Api;
using Report.Api.Mappers;
using Report.Core.Repositories;
using Report.Core.Services;
using Report.Infra.Contexts;
using Report.Infra.Repositories;
using Report.Infra.Services;
using Report.IntegrationTests.Helpers;
using System;
using System.IO;
using System.Reflection;

namespace Report.IntegrationTests
{
    public class StartupAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(ConfigureServices);
            builder.ConfigureLogging((WebHostBuilderContext context, ILoggingBuilder loggingBuilder) =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole(opt => opt.IncludeScopes = true);
            });
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            SetupDatabase(services);
            SetupServices(services);
        }

        private void SetupDatabase(IServiceCollection services)
        {
            services.AddSingleton<DataContext>(new DataContext(
                new DbContextOptionsBuilder<DataContext>()
                    .UseInMemoryDatabase("ReportTest")
                    .Options
            ));

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DataContext>();
                var logger = scopedServices.GetRequiredService<ILogger<StartupAppFactory>>();

                db.Database.EnsureCreated();

                try
                {
                    Fakes.InitializeDbForTesting(db);

                    // And then to detach everything 
                    foreach (var entity in db.ChangeTracker.Entries())
                    {
                        entity.State = EntityState.Detached;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "An error occurred seeding the database with test data. Error: {Message}",
                        ex.Message
                    );
                }
            }
        }

        private void SetupServices(IServiceCollection services)
        {
            var config = GetConfiguration();

            services.AddControllers();

            services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            services.AddAutoMapper(
                typeof(AuthProfile),
                typeof(UserProfile),
                typeof(LogProfile)
            );

            services.AddHttpContextAccessor();

            services.AddSingleton<IConfiguration>(config);

            services.AddSingleton<IHashService, HashService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILogService, LogService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
        }

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(GetProjectPath())
                .AddJsonFile("appsettings.Development.json")
                .Build();
        }

        private string GetProjectPath()
        {
            var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;
            var projectName = startupAssembly.GetName().Name;
            var appBasePath = AppContext.BaseDirectory;
            var directoryInfo = new DirectoryInfo(appBasePath);

            do
            {
                directoryInfo = directoryInfo.Parent;
                if (directoryInfo.Exists)
                {
                    var projectPath = Path.Combine(directoryInfo.FullName, projectName, $"{ projectName }.csproj");
                    if (new FileInfo(projectPath).Exists)
                        return Path.Combine(directoryInfo.FullName, projectName);
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the app root { appBasePath }.");
        }
    }
}