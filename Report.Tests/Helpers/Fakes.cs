using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Report.Api.Mappers;
using Report.Core.Dto.Requests;
using Report.Core.Enums;
using Report.Core.Extensions;
using Report.Core.Models;
using Report.Core.Repositories;

namespace Report.Tests.Helpers
{
    public class Fakes
    {
        private static DataFiles _data;

        public Fakes()
        {
            _data = new DataFiles();

            var configuration = new MapperConfiguration(cfg => 
            {
                cfg.AddProfile<AuthProfile>();
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<LogProfile>();
                cfg.CreateMap<User, UpdateUserRequest>();
                cfg.CreateMap<Log, UpdateLogRequest>();
                cfg.CreateMap<User, Api.Dto.Requests.UpdateUserRequest>();
            });

            this.Mapper = configuration.CreateMapper();
        }

        public List<T> Get<T>()
        {
            return _data.Get<T>();
        }

        public Mock<IUserRepository> FakeUserRepository()
        {
            var repository = new Mock<IUserRepository>();
            
            repository.Setup(x => x.GetAll())
                .Returns(Task.FromResult(Get<User>().ToArray()));

            repository.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns((int id) => {
                    var user = Get<User>().FirstOrDefault(u => u.Id == id);
                    return Task.FromResult(user);
                });

            repository.Setup(x => x.GetByEmail(It.IsAny<string>()))
                .Returns((string email) => {
                    var user = Get<User>().FirstOrDefault(u => u.Email == email);
                    return Task.FromResult(user);
                });

            repository.Setup(x => x.Add(It.IsAny<User>()))
                .Callback<User>(x => x.Id = 999);

            repository.Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(true));

            return repository;
        }

        public Mock<IUserRepository> FakeUserRepositoryException()
        {
            var repository = new Mock<IUserRepository>();
            
            repository.Setup(x => x.GetAll())
                .Callback(() => throw new Exception("Test Exception"));

            repository.Setup(x => x.GetById(It.IsAny<int>()))
                .Callback(() => throw new Exception("Test Exception"));

            repository.Setup(x => x.GetByEmail(It.IsAny<string>()))
                .Callback(() => throw new Exception("Test Exception"));

            repository.Setup(x => x.SaveChangesAsync())
                .Callback(() => throw new Exception("Test Exception"));

            return repository;
        }

        public Mock<ILogRepository> FakeLogRepository()
        {
            var repository = new Mock<ILogRepository>();

            repository.Setup(x => x.GetAll())
                .Returns(Task.FromResult(Get<Log>()
                    .Where(log => log.Archived == false)
                    .ToArray()));
            
            repository.Setup(x => x.GetAllByUserId(It.IsAny<int>()))
                .Returns((int userId) => Task.FromResult(Get<Log>()
                    .Where(log => log.UserId.Equals(userId))
                    .ToArray()));

            repository.Setup(x => x.GetAllUnarchivedByUserId(It.IsAny<int>()))
                .Returns((int userId) => Task.FromResult(Get<Log>()
                    .Where(log => log.UserId.Equals(userId))
                    .Where(log => log.Archived.Equals(false))
                    .ToArray()));

            repository.Setup(x => x.GetAllArchivedByUserId(It.IsAny<int>()))
                .Returns((int userId) => Task.FromResult(Get<Log>()
                    .Where(log => log.UserId.Equals(userId))
                    .Where(log => log.Archived.Equals(true))
                    .ToArray()));

            repository.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns((int id) => Task.FromResult(Get<Log>()
                    .FirstOrDefault(log => log.Id == id)));

            repository.Setup(x => x.Add(It.IsAny<Log>()))
                .Callback<Log>(log => log.Id = 999);

            repository.Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(true));

            return repository;
        }

        public Mock<ILogRepository> FakeLogRepositoryException()
        {
            var repository = new Mock<ILogRepository>();
            
            repository.Setup(x => x.GetAll())
                .Callback(() => throw new Exception("Test Exception"));

            repository.Setup(x => x.GetAllByUserId(It.IsAny<int>()))
                .Callback(() => throw new Exception("Test Exception"));
            
            repository.Setup(x => x.GetAllUnarchivedByUserId(It.IsAny<int>()))
                .Callback(() => throw new Exception("Test Exception"));
                
            repository.Setup(x => x.GetAllArchivedByUserId(It.IsAny<int>()))
                .Callback(() => throw new Exception("Test Exception"));

            repository.Setup(x => x.GetById(It.IsAny<int>()))
                .Callback(() => throw new Exception("Test Exception"));

            repository.Setup(x => x.SaveChangesAsync())
                .Callback(() => throw new Exception("Test Exception"));

            return repository;
        }

        public Mock<IHttpContextAccessor> FakeHttpContextAccessor(
            bool manager = false,
            int userId = 0)
        {
            var http = new Mock<IHttpContextAccessor>();
            
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, $"{ userId }"),
                new Claim(ClaimTypes.Role,
                    manager ? EUserRole.MANAGER.GetName() : EUserRole.DEVELOPER.GetName())
            }));

            http.Setup(x => x.HttpContext).Returns(context);

            return http;
        }

        public Mock<IConfiguration> FakeConfiguration()
        {
            var config = new Mock<IConfiguration>();
            var securitySection = new Mock<IConfigurationSection>();
            var tokenSection = new Mock<IConfigurationSection>();

            tokenSection.Setup(x => x.Value)
                .Returns("cmwSy38DAXQX4sgjE9qzyDydfnf5DnhjkHzNX7JFn8r3RpLeQCA3tdVbJXuN9FTr");

            securitySection.Setup(x => x.GetSection(It.Is<string>(s => s == "TokenSecret")))
                .Returns(tokenSection.Object);

            config.Setup(x => x.GetSection(It.Is<string>(s => s == "Security")))
                .Returns(securitySection.Object);

            return config;
        }

        public IMapper Mapper { get; }
    }
}