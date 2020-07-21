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
    public class AuthServiceTest
    {
        [Fact]
        public void Should_Return_User_And_Token_When_Authenticate_With_Correct_Credentials()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var tokenService = new TokenService(fakeConfig);
            var hashService = new HashService();

            var request = fakes.Get<LoginUserRequest>().First();
            var user = fakes.Get<User>().First();

            // TODO: Validate token 
            var response = new LoginUserResponse();
            response.User = fakes.Mapper.Map<UserResponse>(user);
            response.ExpiresIn = tokenService.GetExpirationInSeconds();

            // Act
            var service = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            var actual = service.Authenticate(request).Result;
            var data = actual.Data as LoginUserResponse;

            response.Token = data.Token;  // Should use the generated token for comparison
            var expected = Responses.OkResponse(null, response);

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LoginUserResponseComparer());
        }

        [Fact]
        public void Should_Return_Forbidden_When_Authenticate_With_Incorrect_Password()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var tokenService = new TokenService(fakeConfig);
            var hashService = new HashService();

            var request = fakes.Get<LoginUserRequest>().First();
            request.Password = "SomeIncorrectPassword";  // Incorrect password
            var expected = Responses.ForbiddenResponse("Email ou senha incorretos");

            // Act
            var service = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            var actual = service.Authenticate(request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_NotFound_When_Authenticate_With_Incorrect_Email()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var tokenService = new TokenService(fakeConfig);
            var hashService = new HashService();

            var request = fakes.Get<LoginUserRequest>().First();
            request.Email = "some@test.email.com";  // Email that doesn't exists in test data
            var expected = Responses.NotFoundResponse("Usuário não encontrado");

            // Act
            var service = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            var actual = service.Authenticate(request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_Authenticate_And_Exception_Is_Thrown()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepositoryException().Object;
            var tokenService = new TokenService(fakeConfig);
            var hashService = new HashService();

            var request = new LoginUserRequest();
            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            var actual = service.Authenticate(request).Result;

            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.Authenticate(request));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }
    
        [Fact]
        public void Should_Return_User_When_Register_With_Valid_Credentials()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var tokenService = new TokenService(fakeConfig);
            var hashService = new HashService();

            var mockUserRepository = fakes.FakeUserRepository();
            // Returns null when GetByEmail is called, so the service doesn't return Conflict.
            mockUserRepository.Setup(x => x.GetByEmail(It.IsAny<string>()))
                .Returns(Task.FromResult<User>(null));
            var fakeUserRepository = mockUserRepository.Object;

            var request = fakes.Get<RegisterUserRequest>().First();
            var user = fakes.Get<User>().First();
            
            var response = fakes.Mapper.Map<UserResponse>(user);
            response.Id = 999;  // Expected user id
            response.Role = EUserRole.DEVELOPER;  // By default, user is created as DEVELOPER

            // Act
            var service = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            var actual = service.Register(request).Result;

            response.CreatedAt = (actual.Data as UserResponse).CreatedAt;
            var expected = Responses.OkResponse(null, response);

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new UserResponseComparer());
        }

        [Fact]
        public void Should_Return_Conflict_When_Register_With_Email_Already_In_Use()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var tokenService = new TokenService(fakeConfig);
            var hashService = new HashService();

            var request = fakes.Get<RegisterUserRequest>().First();
            var expected = Responses.ConflictResponse("Esse email já está sendo utilizado");

            // Act
            var service = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            var actual = service.Register(request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_Register_And_Exception_Is_Thrown()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var fakeUserRepository = fakes.FakeUserRepositoryException().Object;
            var tokenService = new TokenService(fakeConfig);
            var hashService = new HashService();

            var request = fakes.Get<RegisterUserRequest>().First();
            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new AuthService(fakes.Mapper, fakeUserRepository, tokenService, hashService);
            var actual = service.Register(request).Result;

            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.Register(request));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }
    }
}