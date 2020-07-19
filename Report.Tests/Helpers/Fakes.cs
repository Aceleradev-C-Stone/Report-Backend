using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
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
        private Dictionary<Type, string> DataFileNames { get; } =
            new Dictionary<Type, string>();
        private string FileName<T>() { return DataFileNames[typeof(T)]; }

        private char Slash = Path.DirectorySeparatorChar;   

        public Fakes()
        {
            DataFileNames.Add(typeof(User), $"TestData{Slash}Users.json");
            DataFileNames.Add(typeof(CreateUserRequest), $"TestData{Slash}Users.json");
            DataFileNames.Add(typeof(UpdateUserRequest), $"TestData{Slash}Users.json");
            DataFileNames.Add(typeof(Log), $"TestData{Slash}Logs.json");
            DataFileNames.Add(typeof(CreateLogRequest), $"TestData{Slash}Logs.json");
            DataFileNames.Add(typeof(UpdateLogRequest), $"TestData{Slash}Logs.json");

            var configuration = new MapperConfiguration(cfg => 
            {
                cfg.AddProfile<AuthProfile>();
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<LogProfile>();
                cfg.CreateMap<User, UpdateUserRequest>();
            });

            this.Mapper = configuration.CreateMapper();
        }

        public List<T> Get<T>()
        {
            string content = File.ReadAllText(FileName<T>());
            return JsonConvert.DeserializeObject<List<T>>(content);
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

        public IMapper Mapper { get; }
    }
}