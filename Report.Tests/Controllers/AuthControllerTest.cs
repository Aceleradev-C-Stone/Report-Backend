using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Report.Api.Controllers;
using Report.Api.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Infra.Services;
using Report.Tests.Helpers;
using Xunit;

namespace Report.Tests.Controllers
{
    public class AuthControllerTest
    {
        [Fact]
        public void Should_Return_Ok_When_Authenticate_With_Correct_Credentials()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var tokenService = new TokenService(fakeConfig);
            var authService = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            
            var request = fakes.Get<LoginUserRequest>().First();

            // Act
            var controller = new AuthController(fakes.Mapper, authService);
            var actual = controller.Authenticate(request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_Authenticate_With_Incorrect_Password()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var tokenService = new TokenService(fakeConfig);
            var authService = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            
            var request = fakes.Get<LoginUserRequest>().First();
            request.Password = "SomeIncorrectPassword";

            // Act
            var controller = new AuthController(fakes.Mapper, authService);
            var actual = controller.Authenticate(request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_NotFound_When_Authenticate_With_Incorrect_Email()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var tokenService = new TokenService(fakeConfig);
            var authService = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            
            var request = fakes.Get<LoginUserRequest>().First();
            request.Email = "some.incorrect@email.com";

            // Act
            var controller = new AuthController(fakes.Mapper, authService);
            var actual = controller.Authenticate(request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(404, result.StatusCode); // Not Found
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_Authenticate_And_An_Error_Happens()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();
            var tokenService = new TokenService(fakeConfig);
            var authService = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            
            var request = fakes.Get<LoginUserRequest>().First();

            // Act
            var controller = new AuthController(fakes.Mapper, authService);
            var actual = controller.Authenticate(request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }
    
        [Fact]
        public void Should_Return_Ok_When_Register_With_Valid_Credentials()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var tokenService = new TokenService(fakeConfig);
            var authService = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            
            var request = fakes.Get<RegisterUserRequest>().First();
            request.Email = "some@email.com"; // Email that doesn't exist

            // Act
            var controller = new AuthController(fakes.Mapper, authService);
            var actual = controller.Register(request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Conflict_When_Register_With_Email_Already_In_Use()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var tokenService = new TokenService(fakeConfig);
            var authService = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            
            var request = fakes.Get<RegisterUserRequest>().First();

            // Act
            var controller = new AuthController(fakes.Mapper, authService);
            var actual = controller.Register(request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(409, result.StatusCode); // Conflict
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_Register_And_An_Error_Happens()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepositoryException().Object;
            var hashService = new HashService();
            var tokenService = new TokenService(fakeConfig);
            var authService = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            
            var request = fakes.Get<RegisterUserRequest>().First();

            // Act
            var controller = new AuthController(fakes.Mapper, authService);
            var actual = controller.Register(request);

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