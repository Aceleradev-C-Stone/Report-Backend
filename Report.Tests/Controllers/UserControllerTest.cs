using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Report.Api.Controllers;
using Report.Api.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Models;
using Report.Infra.Services;
using Report.Tests.Helpers;
using Xunit;

namespace Report.Tests.Controllers
{
    public class UserControllerTest
    {
        [Fact]
        public void Should_Ok_When_Get_With_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Get();

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_Get_Without_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor().Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Get();

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_Get_And_An_Error_Happens()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Get();

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_GetUserById_With_Owner()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.GetUserById(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_GetUserById_With_Manager()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.GetUserById(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_GetUserById_Without_Authorization()
        {
            // Arrange
            var userId = 1;
            var loggedUserId = userId - 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.GetUserById(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_NotFound_When_GetUserById_With_An_Incorrect_Id()
        {
            // Arrange
            var userId = 9999;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.GetUserById(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(404, result.StatusCode); // Not Found
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_GetUserById_And_An_Error_Happens()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.GetUserById(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_Create_With_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            
            var request = fakes.Get<CreateUserRequest>().First();
            request.Email = "some.test@email.com";  // Email that doesn't exist in test data

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Post(request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_Create_Without_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor().Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            
            var request = fakes.Get<CreateUserRequest>().First();

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Post(request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Conflict_When_Create_With_Existing_Email()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            
            var request = fakes.Get<CreateUserRequest>().First();

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Post(request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(409, result.StatusCode); // Conflict
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_Create_And_An_Error_Happens()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            
            var request = fakes.Get<CreateUserRequest>().First();

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Post(request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_Update_With_Owner()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            
            var user = fakeRepository.GetById(userId).Result;
            var request = fakes.Mapper.Map<UpdateUserRequest>(user);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Put(user.Id, request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_Update_With_Manager()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            
            var user = fakeRepository.GetById(userId).Result;
            var request = fakes.Mapper.Map<UpdateUserRequest>(user);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Put(user.Id, request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_Update_Without_Authorization()
        {
            // Arrange
            var userId = 1;
            var loggedUserId = userId - 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            
            var user = fakeRepository.GetById(userId).Result;
            var request = fakes.Mapper.Map<UpdateUserRequest>(user);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Put(userId, request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
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
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            
            var request = fakes.Get<UpdateUserRequest>().First();

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Put(userId, request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(404, result.StatusCode); // Not Found
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_Update_And_An_Error_Happens()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);
            
            var user = fakes.Get<User>().First();
            var request = fakes.Get<UpdateUserRequest>().First();

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Put(user.Id, request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_Delete_With_Owner()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Delete(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_Delete_With_Manager()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Delete(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_Delete_Without_Authorization()
        {
            // Arrange
            var userId = 1;
            var loggedUserId = userId - 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Delete(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
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
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Delete(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(404, result.StatusCode); // Not Found
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_Delete_And_An_Error_Happens()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeRepository, hashService);

            // Act
            var controller = new UserController(fakes.Mapper, userService);
            var actual = controller.Delete(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }
    }
}