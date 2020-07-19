using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Enums;
using Report.Core.Models;
using Report.Infra.Services;
using Report.Tests.Comparers;
using Report.Tests.Helpers;
using Xunit;

namespace Report.Tests.Services
{
    public class UserServiceTest
    {
        [Fact]
        public void Should_Return_All_Users_When_Get_With_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var users = fakeRepository.GetAll().Result;
            var response = fakes.Mapper.Map<UserResponse[]>(users);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Get().Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new UserResponseListComparer());
        }

        [Fact]
        public void Should_Return_Forbidden_When_Get_Without_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor().Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var expected = Responses.ForbiddenResponse();

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Get().Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }
    
        [Fact]
        public void Should_Return_BadRequest_When_Get_And_Exception_Is_Thrown()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();

            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Get().Result;
            
            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.Get());
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }
    
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_User_When_GetUserById_With_Owner(int userId)
        {
            // Arrange
            var fakes = new Fakes();
            var loggedUserId = userId;
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var user = fakeRepository.GetById(userId).Result;
            var response = fakes.Mapper.Map<UserResponse>(user);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.GetUserById(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new UserResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_User_When_GetUserById_With_Manager(int userId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var user = fakeRepository.GetById(userId).Result;
            var response = fakes.Mapper.Map<UserResponse>(user);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.GetUserById(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new UserResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Forbidden_When_GetUserById_Without_Authorization(int userId)
        {
            // Arrange
            var fakes = new Fakes();
            var loggedUserId = userId - 1;
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var expected = Responses.ForbiddenResponse(
                "Não é possível obter informações de outro usuário");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.GetUserById(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_NotFound_When_GetUserById_With_An_Incorrect_Id()
        {
            // Arrange
            var userId = 0;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var expected = Responses.NotFoundResponse("Usuário não encontrado");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.GetUserById(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_GetUserById_And_Exception_Is_Thrown()
        {
            // Arrange
            var userId = 0;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();

            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.GetUserById(userId).Result;
            
            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.GetUserById(userId));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_User_When_Create_With_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var hashService = new HashService();
            
            var mockRepository = fakes.FakeUserRepository();
            // Returns null when GetByEmail is called, so the service doesn't return Conflict.
            mockRepository.Setup(x => x.GetByEmail(It.IsAny<string>()))
                .Returns(Task.FromResult<User>(null));
            var fakeRepository = mockRepository.Object;

            var request = fakes.Get<CreateUserRequest>().First();
            var user = fakes.Get<User>().First();
            
            var response = fakes.Mapper.Map<UserResponse>(user);
            response.Id = 999;  // Mocked id when creating a new user
            response.Role = EUserRole.DEVELOPER;  // By default, user is created as DEVELOPER
            
            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Create(request).Result;

            response.CreatedAt = (actual.Data as UserResponse).CreatedAt;
            var expected = Responses.OkResponse(null, response);
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new UserResponseComparer());
        }

        [Fact]
        public void Should_Return_Forbidden_When_Create_Without_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor().Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var request = fakes.Get<CreateUserRequest>().First();
            var expected = Responses.ForbiddenResponse();

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Create(request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_Conflict_When_Create_With_Existing_Email()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var request = fakes.Get<CreateUserRequest>().First();
            var expected = Responses.ConflictResponse("Esse email já está sendo utilizado");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Create(request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_Create_And_Exception_Is_Thrown()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();

            var request = fakes.Get<CreateUserRequest>().First();
            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Create(request).Result;

            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.Create(request));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_User_When_Update_With_Owner(int userId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var user = fakeRepository.GetById(userId).Result;
            user.Name = "Updated Title";

            var request = fakes.Mapper.Map<UpdateUserRequest>(user);
            var response = fakes.Mapper.Map<UserResponse>(user);
            var expected = Responses.OkResponse(null, response);
            
            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Update(userId, request).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new UserResponseComparer());
        }

        [Fact]
        public void Should_Return_User_When_Update_With_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var request = fakes.Get<UpdateUserRequest>().First();
            request.Name = "Updated Title";

            var user = fakes.Get<User>().First();
            user.Name = request.Name;
            var response = fakes.Mapper.Map<UserResponse>(user);
            var expected = Responses.OkResponse(null, response);
            
            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Update(user.Id, request).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new UserResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Forbidden_When_Update_Without_Authorization(int userId)
        {
            // Arrange
            var fakes = new Fakes();
            var loggedUserId = userId - 1;
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var request = fakes.Get<UpdateUserRequest>().First();
            var expected = Responses.ForbiddenResponse(
                "Não é possível atualizar informações de outro usuário");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Update(userId, request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_NotFound_When_Update_With_An_Incorrect_Id()
        {
            // Arrange
            var userId = 999;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var request = new UpdateUserRequest();
            var expected = Responses.NotFoundResponse("Usuário não encontrado");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Update(userId, request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_Update_And_Exception_Is_Thrown()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();

            var request = new UpdateUserRequest();
            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Update(userId, request).Result;

            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.Update(userId, request));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Ok_When_Delete_With_Owner(int userId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var expected = Responses.OkResponse("Usuário deletado");
            
            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Delete(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Ok_When_Delete_With_Manager(int userId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var expected = Responses.OkResponse("Usuário deletado");
            
            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Delete(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Forbidden_When_Delete_Without_Authorization(int userId)
        {
            // Arrange
            var fakes = new Fakes();
            var loggedUserId = userId - 1;
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();

            var expected = Responses.ForbiddenResponse(
                "Não é possível deletar informações de outro usuário");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Delete(userId).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_NotFound_When_Delete_With_An_Incorrect_Id()
        {
            // Arrange
            var userId = 999;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            
            var expected = Responses.NotFoundResponse("Usuário não encontrado");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Delete(userId).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_Delete_And_Exception_Is_Thrown()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();

            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            var actual = service.Delete(userId).Result;

            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.Delete(userId));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

    }
}