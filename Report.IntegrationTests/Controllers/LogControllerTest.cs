using Newtonsoft.Json.Linq;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;
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
    public class LogControllerTest
        : BaseControllerTest, IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public LogControllerTest(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact, Priority(23)]
        public async void Should_Return_Ok_When_Get_With_Logged_User()
        {
            // Arrange
            var loginResponse = await LoginAsDesigner(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            // "Get" returns only unarchived logs
            var data = Fakes.Get<LogResponse>()
                .Where(x => x.Archived == false)
                .ToArray();
            Response expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync("api/v1/log");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JArray)response.Data).ToObject<LogResponse[]>();

            var responseStr = await request.Content.ReadAsStringAsync();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseListComparer());
        }

        [Fact, Priority(24)]
        public async void Should_Return_Unauthorized_When_Get_Without_Logged_User()
        {
            // Arrange
            RemoveClientAuthToken(_fixture.Client);

            // Act
            var request = await _fixture.Client.GetAsync("api/v1/log");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(25)]
        public async void Should_Return_Ok_When_GetLogsByUserId_With_Owner()
        {
            // Arrange
            var userId = 3;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<LogResponse>().FindAll(x => x.UserId == userId).ToArray();
            Response expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JArray)response.Data).ToObject<LogResponse[]>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseListComparer());
        }

        [Fact, Priority(26)]
        public async void Should_Return_Ok_When_GetLogsByUserId_With_Manager()
        {
            // Arrange
            var userId = 3;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<LogResponse>().FindAll(x => x.UserId == userId).ToArray();
            Response expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JArray)response.Data).ToObject<LogResponse[]>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseListComparer());
        }

        [Fact, Priority(27)]
        public async void Should_Return_Forbidden_When_GetLogsByUserId_Without_Authorization()
        {
            // Arrange
            var userId = 3;
            var loggedUserId = userId - 1;
            var user = Fakes.Get<User>().Find(x => x.Id == loggedUserId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            Response expected = Responses.ForbiddenResponse(
                "Não é possível obter logs de outro usuário");

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(28)]
        public async void Should_Return_Unauthorized_When_GetLogsByUserId_Without_Logged_User()
        {
            // Arrange
            var userId = 1;
            RemoveClientAuthToken(_fixture.Client);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/user/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(29)]
        public async void Should_Return_Ok_When_GetUnarchivedLogsByUserId_With_Owner()
        {
            // Arrange
            var userId = 3;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<LogResponse>()
                .Where(x => x.UserId == userId)
                .Where(x => x.Archived == false)
                .ToArray();
            Response expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/unarchived/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JArray)response.Data).ToObject<LogResponse[]>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseListComparer());
        }

        [Fact, Priority(30)]
        public async void Should_Return_Ok_When_GetUnarchivedLogsByUserId_With_Manager()
        {
            // Arrange
            var userId = 3;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<LogResponse>()
                .Where(x => x.UserId == userId)
                .Where(x => x.Archived == false)
                .ToArray();
            Response expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/unarchived/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JArray)response.Data).ToObject<LogResponse[]>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseListComparer());
        }

        [Fact, Priority(31)]
        public async void Should_Return_Forbidden_When_GetUnarchivedLogsByUserId_Without_Authorization()
        {
            // Arrange
            var userId = 3;
            var loggedUserId = userId - 1;
            var user = Fakes.Get<User>().Find(x => x.Id == loggedUserId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            Response expected = Responses.ForbiddenResponse(
                "Não é possível obter logs de outro usuário");

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/unarchived/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(32)]
        public async void Should_Return_Unauthorized_When_GetUnarchivedLogsByUserId_Without_Logged_User()
        {
            // Arrange
            var userId = 1;
            RemoveClientAuthToken(_fixture.Client);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/unarchived/user/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(33)]
        public async void Should_Return_Ok_When_GetArchivedLogsByUserId_With_Owner()
        {
            // Arrange
            var userId = 3;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<LogResponse>()
                .Where(x => x.UserId == userId)
                .Where(x => x.Archived == true)
                .ToArray();
            Response expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/archived/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JArray)response.Data).ToObject<LogResponse[]>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseListComparer());
        }

        [Fact, Priority(34)]
        public async void Should_Return_Ok_When_GetArchivedLogsByUserId_With_Manager()
        {
            // Arrange
            var userId = 3;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<LogResponse>()
                .Where(x => x.UserId == userId)
                .Where(x => x.Archived == true)
                .ToArray();
            Response expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/archived/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JArray)response.Data).ToObject<LogResponse[]>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseListComparer());
        }

        [Fact, Priority(35)]
        public async void Should_Return_Forbidden_When_GetArchivedLogsByUserId_Without_Authorization()
        {
            // Arrange
            var userId = 3;
            var loggedUserId = userId - 1;
            var user = Fakes.Get<User>().Find(x => x.Id == loggedUserId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            Response expected = Responses.ForbiddenResponse(
                "Não é possível obter logs de outro usuário");

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/archived/user/{userId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(36)]
        public async void Should_Return_Unauthorized_When_GetArchivedLogsByUserId_Without_Logged_User()
        {
            // Arrange
            var userId = 1;
            RemoveClientAuthToken(_fixture.Client);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/archived/user/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(37)]
        public async void Should_Return_Ok_When_GetLogById_With_Owner()
        {
            // Arrange
            var logId = 1;
            var userId = 9;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<LogResponse>().Find(x => x.Id == logId);
            Response expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/{logId}");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<LogResponse>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseComparer());
        }

        [Fact, Priority(38)]
        public async void Should_Return_Ok_When_GetLogById_With_Manager()
        {
            // Arrange
            var logId = 1;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<LogResponse>().Find(x => x.Id == logId);
            Response expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/{logId}");
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<LogResponse>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseComparer());
        }

        [Fact, Priority(39)]
        public async void Should_Return_Forbidden_When_GetLogById_Without_Authorization()
        {
            // Arrange
            var logId = 1;
            var userId = 3;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            Response expected = Responses.ForbiddenResponse(
                "Não é possível obter logs de outro usuário");

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/{logId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(40)]
        public async void Should_Return_Unauthorized_When_GetLogById_Without_Logged_User()
        {
            // Arrange
            var logId = 1;
            RemoveClientAuthToken(_fixture.Client);

            // Act
            var request = await _fixture.Client.GetAsync($"api/v1/log/{logId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(41)]
        public async void Should_Return_Ok_When_Post_With_Logged_User()
        {
            // Arrange
            var loginResponse = await LoginAsDeveloper(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = Fakes.Get<CreateLogRequest>().First();

            // Act
            var request = await _fixture.Client.PostAsJsonAsync($"api/v1/log", requestData);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<LogResponse>();

            var data = Fakes.Get<LogResponse>().First();
            data.Id = (response.Data as LogResponse).Id;
            data.UserId = loginResponse.User.Id;
            data.UserName = loginResponse.User.Name;
            data.Archived = false;
            data.CreatedAt = (response.Data as LogResponse).CreatedAt;
            var expected = Responses.OkResponse(null, data);

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseComparer());
        }

        [Fact, Priority(42)]
        public async void Should_Return_Ok_When_Post_With_Manager()
        {
            // Arrange
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = Fakes.Get<CreateLogRequest>().First();

            // Act
            var request = await _fixture.Client.PostAsJsonAsync($"api/v1/log", requestData);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<LogResponse>();

            var data = Fakes.Get<LogResponse>().First();
            data.Id = (response.Data as LogResponse).Id;
            data.UserId = loginResponse.User.Id;
            data.UserName = loginResponse.User.Name;
            data.Archived = false;
            data.CreatedAt = (response.Data as LogResponse).CreatedAt;
            var expected = Responses.OkResponse(null, data);

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseComparer());
        }

        [Fact, Priority(43)]
        public async void Should_Return_Ok_When_Post_With_Manager_And_Specified_UserId()
        {
            // Arrange
            var userId = 9;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = Fakes.Get<CreateLogRequest>().First();
            requestData.UserId = userId;

            // Act
            var request = await _fixture.Client.PostAsJsonAsync($"api/v1/log", requestData);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<LogResponse>();

            var data = Fakes.Get<LogResponse>().First();
            data.Id = (response.Data as LogResponse).Id;
            data.UserId = userId;
            data.UserName = (response.Data as LogResponse).UserName;
            data.Archived = false;
            data.CreatedAt = (response.Data as LogResponse).CreatedAt;
            var expected = Responses.OkResponse(null, data);

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseComparer());
        }

        [Fact, Priority(44)]
        public async void Should_Return_Unauthorized_When_Post_Without_Logged_User()
        {
            // Arrange
            RemoveClientAuthToken(_fixture.Client);

            var requestData = Fakes.Get<CreateLogRequest>().First();

            // Act
            var request = await _fixture.Client.PostAsJsonAsync($"api/v1/log", requestData);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(45)]
        public async void Should_Return_Ok_When_Put_With_Owner()
        {
            // Arrange
            var logId = 1;
            var userId = 9;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = Fakes.Get<UpdateLogRequest>().First();
            requestData.Title = "Some Test Title";

            var data = Fakes.Get<LogResponse>().First();
            data.Title = requestData.Title;

            var expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.PutAsJsonAsync($"api/v1/log/{logId}", requestData);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<LogResponse>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseComparer());
        }

        [Fact, Priority(46)]
        public async void Should_Return_Ok_When_Put_With_Manager()
        {
            // Arrange
            var logId = 20;
            var index = logId - 1;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = Fakes.Get<UpdateLogRequest>().ElementAt(index);
            requestData.Title = "Some Test Title";

            // Act
            var request = await _fixture.Client.PutAsJsonAsync($"api/v1/log/{logId}", requestData);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<LogResponse>();

            var data = Fakes.Get<LogResponse>().ElementAt(index);
            data.Title = requestData.Title;
            data.UserName = (response.Data as LogResponse).UserName;
            var expected = Responses.OkResponse(null, data);

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseComparer());
        }

        [Fact, Priority(47)]
        public async void Should_Return_Forbidden_When_Put_Without_Authorization()
        {
            // Arrange
            var logId = 1;
            var userId = 3;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = Fakes.Get<UpdateLogRequest>().First();
            requestData.Title = "Some Test Title";

            var expected = Responses.ForbiddenResponse(
                "Não é possível atualizar um log de outro usuário");

            // Act
            var request = await _fixture.Client.PutAsJsonAsync($"api/v1/log/{logId}", requestData);
            var response = await request.Content.ReadAsAsync<Response>();

            var responseStr = await request.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(48)]
        public async void Should_Return_Unauthorized_When_Put_Without_Logged_User()
        {
            // Arrange
            var logId = 3;
            RemoveClientAuthToken(_fixture.Client);

            var requestData = new UpdateLogRequest();
            requestData.Title = "Some Test Title";

            // Act
            var request = await _fixture.Client.PutAsJsonAsync($"api/v1/log/{logId}", requestData);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(49)]
        public async void Should_Return_NotFound_When_Put_With_An_Incorrect_Id()
        {
            // Arrange
            var logId = 9999;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var requestData = Fakes.Get<UpdateLogRequest>().First();
            requestData.Title = "Some Test Title";

            var expected = Responses.NotFoundResponse("Log não encontrado");

            // Act
            var request = await _fixture.Client.PutAsJsonAsync($"api/v1/log/{logId}", requestData);
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }
        
        [Fact, Priority(50)]
        public async void Should_Return_Ok_When_Delete_With_Owner()
        {
            // Arrange
            var logId = 4;
            var userId = 4;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.OkResponse("Log deletado");

            // Act
            var request = await _fixture.Client.DeleteAsync($"api/v1/log/{logId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(51)]
        public async void Should_Return_Ok_When_Delete_With_Manager()
        {
            // Arrange
            var logId = 2;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.OkResponse("Log deletado");

            // Act
            var request = await _fixture.Client.DeleteAsync($"api/v1/log/{logId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(52)]
        public async void Should_Return_Forbidden_When_Delete_Without_Authorization()
        {
            // Arrange
            var logId = 1;
            var userId = 3;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.ForbiddenResponse(
                "Não é possível deletar um log de outro usuário");

            // Act
            var request = await _fixture.Client.DeleteAsync($"api/v1/log/{logId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(53)]
        public async void Should_Return_Unauthorized_When_Delete_Without_Logged_User()
        {
            // Arrange
            var logId = 3;
            RemoveClientAuthToken(_fixture.Client);

            // Act
            var request = await _fixture.Client.DeleteAsync($"api/v1/log/{logId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(54)]
        public async void Should_Return_NotFound_When_Delete_With_An_Incorrect_Id()
        {
            // Arrange
            var logId = 9999;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.NotFoundResponse("Log não encontrado");

            // Act
            var request = await _fixture.Client.DeleteAsync($"api/v1/log/{logId}");
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }
        
        [Fact, Priority(55)]
        public async void Should_Return_Ok_When_Archive_With_Owner()
        {
            // Arrange
            var logId = 15;
            var userId = 6;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<LogResponse>().Find(x => x.Id == logId);
            data.Archived = !data.Archived;
            var expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.PatchAsync($"api/v1/log/archive/{logId}", null);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<LogResponse>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseComparer());
        }

        [Fact, Priority(56)]
        public async void Should_Return_Ok_When_Archive_With_Manager()
        {
            // Arrange
            var logId = 32;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var data = Fakes.Get<LogResponse>().Find(x => x.Id == logId);
            data.Archived = !data.Archived;
            var expected = Responses.OkResponse(null, data);

            // Act
            var request = await _fixture.Client.PatchAsync($"api/v1/log/archive/{logId}", null);
            var response = await request.Content.ReadAsAsync<Response>();
            response.Data = ((JObject)response.Data).ToObject<LogResponse>();

            // Assert
            Assert.True(request.IsSuccessStatusCode);
            Assert.Equal(expected, response, new LogResponseComparer());
        }

        [Fact, Priority(57)]
        public async void Should_Return_Forbidden_When_Archive_Without_Authorization()
        {
            // Arrange
            var logId = 1;
            var userId = 3;
            var user = Fakes.Get<User>().Find(x => x.Id == userId);
            var loginResponse = await LoginUser(_fixture.Client, user.Email);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.ForbiddenResponse(
                "Não é possível arquivar ou desarquivar um log de outro usuário");

            // Act
            var request = await _fixture.Client.PatchAsync($"api/v1/log/archive/{logId}", null);
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }

        [Fact, Priority(58)]
        public async void Should_Return_Unauthorized_When_Archive_Without_Logged_User()
        {
            // Arrange
            var logId = 3;
            RemoveClientAuthToken(_fixture.Client);

            // Act
            var request = await _fixture.Client.PatchAsync($"api/v1/log/archive/{logId}", null);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, request.StatusCode);
        }

        [Fact, Priority(59)]
        public async void Should_Return_NotFound_When_Archive_With_An_Incorrect_Id()
        {
            // Arrange
            var logId = 9999;
            var loginResponse = await LoginAsManager(_fixture.Client);
            SetClientAuthToken(_fixture.Client, loginResponse.Token);

            var expected = Responses.NotFoundResponse("Log não encontrado");

            // Act
            var request = await _fixture.Client.PatchAsync($"api/v1/log/archive/{logId}", null);
            var response = await request.Content.ReadAsAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
            Assert.Equal(expected, response, new ResponseComparer());
        }
    }
}
