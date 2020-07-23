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
    public class LogControllerTest
    {
        [Fact]
        public void Should_Return_Ok_When_Get()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor().Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
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
        public void Should_Return_BadRequest_When_Get_And_An_Error_Happens()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor().Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
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
        public void Should_Return_Ok_When_GetLogsByUserId_With_Owner()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_GetLogsByUserId_With_Manager()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_GetLogsByUserId_Without_Authorization()
        {
            // Arrange
            var userId = 1;
            var loggedUserId = userId - 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_GetLogsByUserId_And_An_Error_Happens()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_GetUnarchivedLogsByUserId_With_Owner()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetUnarchivedLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_GetUnarchivedLogsByUserId_With_Manager()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetUnarchivedLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_GetUnarchivedLogsByUserId_Without_Authorization()
        {
            // Arrange
            var userId = 1;
            var loggedUserId = userId - 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetUnarchivedLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_GetUnarchivedLogsByUserId_And_An_Error_Happens()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetUnarchivedLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_GetArchivedLogsByUserId_With_Owner()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetArchivedLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_GetArchivedLogsByUserId_With_Manager()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetArchivedLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_GetArchivedLogsByUserId_Without_Authorization()
        {
            // Arrange
            var userId = 1;
            var loggedUserId = userId - 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetArchivedLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_GetArchivedLogsByUserId_And_An_Error_Happens()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetArchivedLogsByUserId(userId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_GetLogById_With_Owner()
        {
            // Arrange
            var logId = 1;
            var userId = 9;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetLogById(logId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_GetLogById_With_Manager()
        {
            // Arrange
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetLogById(logId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_GetLogById_Without_Authorization()
        {
            // Arrange
            var logId = 1;
            var userId = 3;  // User with id 3 isn't owner of the log with id 1
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetLogById(logId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_GetLogById_And_An_Error_Happens()
        {
            // Arrange
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.GetLogById(logId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }
        
        [Fact]
        public void Should_Return_Ok_When_Post_With_Logged_User()
        {
            // Arrange
            var loggedUserId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);
            
            var request = fakes.Get<CreateLogRequest>().First();

            // Act
            var controller = new LogController(fakes.Mapper, logService);
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
        public void Should_Return_Ok_When_Post_With_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);
            
            var request = fakes.Get<CreateLogRequest>().First();

            // Act
            var controller = new LogController(fakes.Mapper, logService);
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
        public void Should_Return_Ok_When_Post_With_Manager_And_Specified_UserId()
        {
            // Arrange
            var userId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);
            
            var request = fakes.Get<CreateLogRequest>().First();
            request.UserId = userId;

            // Act
            var controller = new LogController(fakes.Mapper, logService);
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
        public void Should_Return_BadRequest_When_Post_And_An_Error_Happens()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);
            
            var request = fakes.Get<CreateLogRequest>().First();

            // Act
            var controller = new LogController(fakes.Mapper, logService);
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
        public void Should_Return_Ok_When_Put_With_Owner()
        {
            // Arrange
            var logId = 1;
            var userId = 9;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);
            
            var request = new UpdateLogRequest();

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Put(logId, request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_Put_With_Manager()
        {
            // Arrange
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);
            
            var request = new UpdateLogRequest();

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Put(logId, request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_Put_Without_Authorization()
        {
            // Arrange
            var logId = 1;
            var userId = 3;  // User with id 3 isn't owner of the log with id 1
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);
            
            var request = new UpdateLogRequest();

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Put(logId, request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_NotFound_When_Put_With_An_Incorrect_Id()
        {
            // Arrange
            var logId = 9999;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);
            
            var request = new UpdateLogRequest();

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Put(logId, request);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(404, result.StatusCode); // Not Found
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_Put_And_An_Error_Happens()
        {
            // Arrange
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);
            
            var request = new UpdateLogRequest();

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Put(logId, request);

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
            var logId = 1;
            var userId = 9;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Delete(logId);

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
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Delete(logId);

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
            var logId = 1;
            var userId = 3;  // User with id 3 isn't owner of the log with id 1
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Delete(logId);

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
            var logId = 9999;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Delete(logId);

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
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Delete(logId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(400, result.StatusCode); // Bad Request
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_Archive_With_Owner()
        {
            // Arrange
            var logId = 1;
            var userId = 9;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Archive(logId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Ok_When_Archive_With_Manager()
        {
            // Arrange
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Archive(logId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode); // Ok
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_Forbidden_When_Archive_Without_Authorization()
        {
            // Arrange
            var logId = 1;
            var userId = 3;  // User with id 3 isn't owner of the log with id 1
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, userId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Archive(logId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(403, result.StatusCode); // Forbidden
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_NotFound_When_Archive_With_An_Incorrect_Id()
        {
            // Arrange
            var logId = 9999;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Archive(logId);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ObjectResult>(actual.Result);

            var result = actual.Result as ObjectResult;
            Assert.NotNull(result.Value);
            Assert.Equal(404, result.StatusCode); // Not Found
            Assert.IsType<Response>(result.Value);
        }

        [Fact]
        public void Should_Return_BadRequest_When_Archive_And_An_Error_Happens()
        {
            // Arrange
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);
            var logService = new LogService(fakes.Mapper, fakeRepository, userService);

            // Act
            var controller = new LogController(fakes.Mapper, logService);
            var actual = controller.Archive(logId);

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