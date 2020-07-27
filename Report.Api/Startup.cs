using System;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Report.Api.Mappers;
using Report.Core.Repositories;
using Report.Core.Services;
using Report.Infra.Contexts;
using Report.Infra.Repositories;
using Report.Infra.Services;

namespace Report.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            SetupDbContext(services);
            SetupControllers(services);
            SetupAuthentication(services);
            SetupServices(services);
            SetupSwagger(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(cfg =>
                cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "Record v1")
            );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(cfg => cfg
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void SetupDbContext(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(
                opt => opt.UseSqlServer(Configuration.GetConnectionString("Connection"))
            );
        }

        private void SetupControllers(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(opt => {
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
        }

        private void SetupAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters =
                    TokenService.GetTokenValidationParameters(Configuration);
            });
        }

        private void SetupServices(IServiceCollection services)
        {
            services.AddAutoMapper(
                typeof(AuthProfile),
                typeof(UserProfile),
                typeof(LogProfile)
            );
                
            services.AddHttpContextAccessor();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton<IHashService, HashService>();
            services.AddSingleton<ITokenService, TokenService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILogService, LogService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
        }

        private void SetupSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Record API",
                    Description = "API que centraliza logs de vários serviços",
                    Contact = new OpenApiContact
                    {
                        Name = "Felipe Vieira",
                        Email = "fvieiramacieldesouza58@gmail.com",
                        Url = new Uri("https://github.com/MrChampz"),
                    }
                });
            });
        }
    }
}
