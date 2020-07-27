using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Enums;
using Report.IntegrationTests.Comparers;
using Report.IntegrationTests.Helpers;
using Xunit;
using Xunit.Priority;

namespace Report.IntegrationTests.Controllers
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class AuthControllerTest
        : BaseControllerTest, IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public AuthControllerTest(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact, Priority(18)]
        public async void Should_Return_Ok_When_Authenticate_With_Correct_Credentials()
        {
            // Arrange
            RemoveClientAuthToken(_fixture.Client);

            var data = Fakes.Get<LoginUserRequest>().First();

            var loginResponse = new LoginUserResponse();
            loginResponse.User = Fakes.Get<UserResponse>().First();

            // Act
            var request = await _fixture.Client.PostAsJsonAsync("api/v1/auth/login", data);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<LoginUserResponse>();
            var responseData = response.Data as LoginUserResponse;

            loginResponse.Token = responseData.Token;  // Should use the generated token for comparison
            loginResponse.ExpiresIn = responseData.ExpiresIn;
            var expected = Responses.OkResponse(null, loginResponse);

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LoginUserResponseComparer());
        }

        [Fact, Priority(19)]
        public async void Should_Return_Forbidden_When_Authenticate_With_Incorrect_Password()
        {
            // Arrange
            RemoveClientAuthToken(_fixture.Client);

            var data = Fakes.Get<LoginUserRequest>().First();
            data.Password = "SomeIncorrectPassword";  // Incorrect password
            var expected = Responses.ForbiddenResponse("Email ou senha incorretos");

            // Act
            var request = await _fixture.Client.PostAsJsonAsync("api/v1/auth/login", data);
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(20)]
        public async void Should_Return_NotFound_When_Authenticate_With_Incorrect_Email()
        {
            // Arrange
            RemoveClientAuthToken(_fixture.Client);

            var data = Fakes.Get<LoginUserRequest>().First();
            data.Email = "some.incorrect@email.com";  // Incorrect email
            var expected = Responses.NotFoundResponse("Usuário não encontrado");

            // Act
            var request = await _fixture.Client.PostAsJsonAsync("api/v1/auth/login", data);
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }
    
        [Fact, Priority(21)]
        public async void Should_Return_Ok_When_Register_With_Valid_Credentials()
        {
            // Arrange
            RemoveClientAuthToken(_fixture.Client);

            var data = Fakes.Get<RegisterUserRequest>().First();
            data.Email = "some@email.com"; // Email that doesn't exist

            // Act
            var request = await _fixture.Client.PostAsJsonAsync("api/v1/auth/register", data);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<UserResponse>();

            var user = Fakes.Get<UserResponse>().First();
            user.Id = (response.Data as UserResponse).Id;
            user.Email = data.Email;
            user.Role = EUserRole.DEVELOPER;
            user.CreatedAt = (response.Data as UserResponse).CreatedAt;
            
            var expected = Responses.OkResponse(null, user);

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new UserResponseComparer());
        }

        [Fact, Priority(22)]
        public async void Should_Return_Conflict_When_Register_With_Email_Already_In_Use()
        {
            // Arrange
            RemoveClientAuthToken(_fixture.Client);

            var data = Fakes.Get<RegisterUserRequest>().First();
            var expected = Responses.ConflictResponse("Esse email já está sendo utilizado");

            // Act
            var request = await _fixture.Client.PostAsJsonAsync("api/v1/auth/register", data);
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }
    }
}