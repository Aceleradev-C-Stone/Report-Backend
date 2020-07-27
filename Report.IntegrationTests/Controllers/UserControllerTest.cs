using Newtonsoft.Json.Linq;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Enums;
using Report.Core.Models;
using Report.IntegrationTests.Comparers;
using Report.IntegrationTests.Helpers;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;
using Xunit.Priority;

namespace Report.IntegrationTests.Controllers
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class UserControllerTest
        : BaseControllerTest, IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public UserControllerTest(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact, Priority(1)]
        public async void Should_Return_Ok_When_Get_With_Manager()
        {
            // Arrange
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<UserResponse>().ToArray();
            var expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync("api/v1/user");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JArray)response.Data).ToObject<UserResponse[]>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new UserResponseListComparer());
        }

        [Fact, Priority(2)]
        public async void Should_Return_Forbidden_When_Get_Without_Authorization()
        {
            // Arrange
            var loginResponse = await LoginAsDeveloper(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.ForbiddenResponse();

            // Act
            var request = await _fixture.Client.GetAsync("api/v1/user");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(3)]
        public async void Should_Return_Unauthorized_When_Get_Without_Logged_User()
        {
            // Arrange
            RemoveClientAuthToken(_fixture.Client);

            // Act
            var request = await _fixture.Client.GetAsync("api/v1/user");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Theory, Priority(4)]
        [InlineData(2)]
        [InlineData(3)]
        public async void Should_Return_Ok_When_GetUserById_With_Owner(int userId)
        {
            // Arrange
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<UserResponse>().Find(x => x.Id == userId);
            var expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<UserResponse>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new UserResponseComparer());
        }

        [Theory, Priority(5)]
        [InlineData(2)]
        [InlineData(3)]
        public async void Should_Return_Ok_When_GetUserById_With_Manager(int userId)
        {
            // Arrange
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<UserResponse>().Find(x => x.Id == userId);
            var expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<UserResponse>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new UserResponseComparer());
        }

        [Fact, Priority(6)]
        public async void Should_Return_Forbidden_When_GetUserById_Without_Authorization()
        {
            // Arrange
            var userId = 3;
            var loggedUserId = userId - 1;
            var user = Fakes.Get<User>().Find(x => x.Id == loggedUserId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.ForbiddenResponse(
                "Não é possível obter informações de outro usuário");

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(7)]
        public async void Should_Return_Unauthorized_When_GetUserById_Without_Authorization()
        {
            // Arrange
            var userId = 3;
            RemoveClientAuthToken(_fixture.Client);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/user/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(8)]
        public async void Should_Return_NotFound_When_GetUserById_With_An_Incorrect_Id()
        {
            // Arrange
            var userId = 9999;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.NotFoundResponse("Usuário não encontrado");

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(9)]
        public async void Should_Return_Ok_When_Post_With_Manager()
        {
            // Arrange
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = Fakes.Get<CreateUserRequest>().First();
            requestData.Email = "some.test@email.com";

            // Act
            var request = await _fixture.Client.PostAsJsonAsync($"api/v1/user", requestData);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<UserResponse>();

            var data = Fakes.Get<UserResponse>().First();
            data.Id = (response.Data as UserResponse).Id;
            data.Email = requestData.Email;
            data.Role = EUserRole.DEVELOPER;
            data.CreatedAt = (response.Data as UserResponse).CreatedAt;
            var expected = Responses.OkResponse(null, data);

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new UserResponseComparer());
        }

        [Fact, Priority(10)]
        public async void Should_Return_Forbidden_When_Post_Without_Manager()
        {
            // Arrange
            var loginResponse = await LoginAsDeveloper(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = Fakes.Get<CreateUserRequest>().First();
            requestData.Email = "some.test@email.com";

            var expected = Responses.ForbiddenResponse();

            // Act
            var request = await _fixture.Client.PostAsJsonAsync($"api/v1/user", requestData);
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(11)]
        public async void Should_Return_Unauthorized_When_Post_Without_Logged_User()
        {
            // Arrange
            RemoveClientAuthToken(_fixture.Client);

            var requestData = Fakes.Get<CreateUserRequest>().First();
            requestData.Email = "some.test@email.com";

            // Act
            var request = await _fixture.Client.PostAsJsonAsync($"api/v1/user", requestData);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(12)]
        public async void Should_Return_Conflict_When_Post_With_Existing_Email()
        {
            // Arrange
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = Fakes.Get<CreateUserRequest>().First();

            var expected = Responses.ConflictResponse("Esse email já está sendo utilizado");

            // Act
            var request = await _fixture.Client.PostAsJsonAsync($"api/v1/user", requestData);
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Theory, Priority(13)]
        [InlineData(2)]
        [InlineData(3)]
        public async void Should_Return_Ok_When_Put_With_Owner(int userId)
        {
            // Arrange
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = new UpdateUserRequest();
            requestData.Name = "Some Test Name";

            var data = Fakes.Get<UserResponse>().Find(x => x.Id == userId);
            data.Email = requestData.Name;

            var expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.PutAsJsonAsync($"api/v1/user/{userId}", requestData);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<UserResponse>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new UserResponseComparer());
        }

        [Theory, Priority(14)]
        [InlineData(2)]
        [InlineData(3)]
        public async void Should_Return_Ok_When_Put_With_Manager(int userId)
        {
            // Arrange
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = new UpdateUserRequest();
            requestData.Name = "Some Test Name";

            var data = Fakes.Get<UserResponse>().Find(x => x.Id == userId);
            data.Email = requestData.Name;

            var expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.PutAsJsonAsync($"api/v1/user/{userId}", requestData);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<UserResponse>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new UserResponseComparer());
        }

        [Fact, Priority(15)]
        public async void Should_Return_Forbidden_When_Put_Without_Authorization()
        {
            // Arrange
            var userId = 3;
            var loggedUserId = userId - 1;
            var user = Fakes.Get<User>().Find(x => x.Id == loggedUserId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = new UpdateUserRequest();
            requestData.Name = "Some Test Name";

            var expected = Responses.ForbiddenResponse(
                "Não é possível atualizar informações de outro usuário");

            // Act
            var request = await _fixture.Client.PutAsJsonAsync($"api/v1/user/{userId}", requestData);
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(16)]
        public async void Should_Return_Unauthorized_When_Put_Without_Logged_User()
        {
            // Arrange
            var userId = 3;
            RemoveClientAuthToken(_fixture.Client);

            var requestData = new UpdateUserRequest();
            requestData.Name = "Some Test Name";

            // Act
            var request = await _fixture.Client.PutAsJsonAsync($"api/v1/user/{userId}", requestData);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(17)]
        public async void Should_Return_NotFound_When_Put_With_An_Incorrect_Id()
        {
            // Arrange
            var userId = 9999;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = new UpdateUserRequest();
            requestData.Name = "Some Test Name";

            var expected = Responses.NotFoundResponse("Usuário não encontrado");

            // Act
            var request = await _fixture.Client.PutAsJsonAsync($"api/v1/user/{userId}", requestData);
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Theory, Priority(60)]
        [InlineData(2)]
        [InlineData(3)]
        public async void Should_Return_Ok_When_Delete_With_Owner(int userId)
        {
            // Arrange
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.OkResponse("Usuário deletado");

            // Act
            var request = await _fixture.Client.DeleteAsync($"api/v1/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Theory, Priority(61)]
        [InlineData(4)]
        [InlineData(5)]
        public async void Should_Return_Ok_When_Delete_With_Manager(int userId)
        {
            // Arrange
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.OkResponse("Usuário deletado");

            // Act
            var request = await _fixture.Client.DeleteAsync($"api/v1/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(62)]
        public async void Should_Return_Forbidden_When_Delete_Without_Authorization()
        {
            // Arrange
            var userId = 7;
            var loggedUserId = userId - 1;
            var user = Fakes.Get<User>().Find(x => x.Id == loggedUserId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.ForbiddenResponse(
                "Não é possível deletar informações de outro usuário");

            // Act
            var request = await _fixture.Client.DeleteAsync($"api/v1/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(63)]
        public async void Should_Return_Unauthorized_When_Delete_Without_Logged_User()
        {
            // Arrange
            var userId = 3;
            RemoveClientAuthToken(_fixture.Client);

            // Act
            var request = await _fixture.Client.DeleteAsync($"api/v1/user/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(64)]
        public async void Should_Return_NotFound_When_Delete_With_An_Incorrect_Id()
        {
            // Arrange
            var userId = 9999;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.NotFoundResponse("Usuário não encontrado");

            // Act
            var request = await _fixture.Client.DeleteAsync($"api/v1/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }
    }
}