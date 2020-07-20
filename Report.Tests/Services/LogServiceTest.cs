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
    public class LogServiceTest
    {
        [Fact]
        public void Should_Return_All_Unarchived_Logs_When_GetAll()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor().Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var logs = fakeRepository.GetAll().Result;
            var response = fakes.Mapper.Map<LogResponse[]>(logs);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetAll().Result;
            var logResponses = actual.Data as LogResponse[];
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseListComparer());
            Array.ForEach(logResponses, log => Assert.False(log.Archived));
        }
    
        [Fact]
        public void Should_Return_BadRequest_When_GetAll_And_Exception_Is_Thrown()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor().Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetAll().Result;
            
            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.GetAll());
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }
    
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_All_User_Logs_When_GetLogsByUserId_With_Owner(int userId)
        {
            // Arrange
            var loggedUserId = userId;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var logs = fakeRepository.GetAllByUserId(userId).Result;
            var response = fakes.Mapper.Map<LogResponse[]>(logs);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetLogsByUserId(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseListComparer());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_All_User_Logs_When_GetLogsByUserId_With_Manager(int userId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var logs = fakeRepository.GetAllByUserId(userId).Result;
            var response = fakes.Mapper.Map<LogResponse[]>(logs);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetLogsByUserId(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseListComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Forbidden_When_GetLogsByUserId_Without_Authorization(int userId)
        {
            // Arrange
            var loggedUserId = userId - 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var expected = Responses.ForbiddenResponse(
                "Não é possível obter logs de outro usuário");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetLogsByUserId(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_GetLogsByUserId_And_Exception_Is_Thrown()
        {
            // Arrange
            var userId = 0;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetLogsByUserId(userId).Result;
            
            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.GetLogsByUserId(userId));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_All_User_Unarchived_Logs_When_GetUnarchivedLogsByUserId_With_Owner(int userId)
        {
            // Arrange
            var loggedUserId = userId;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var logs = fakeRepository.GetAllUnarchivedByUserId(userId).Result;
            var response = fakes.Mapper.Map<LogResponse[]>(logs);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetUnarchivedLogsByUserId(userId).Result;
            var logResponses = actual.Data as LogResponse[];
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseListComparer());
            Array.ForEach(logResponses, log => Assert.False(log.Archived));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_All_User_Unarchived_Logs_When_GetUnarchivedLogsByUserId_With_Manager(int userId)
        {
            // Arrange
            var loggedUserId = userId;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var logs = fakeRepository.GetAllUnarchivedByUserId(userId).Result;
            var response = fakes.Mapper.Map<LogResponse[]>(logs);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetUnarchivedLogsByUserId(userId).Result;
            var logResponses = actual.Data as LogResponse[];
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseListComparer());
            Array.ForEach(logResponses, log => Assert.False(log.Archived));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Forbidden_When_GetUnarchivedLogsByUserId_Without_Authorization(int userId)
        {
            // Arrange
            var loggedUserId = userId - 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var expected = Responses.ForbiddenResponse(
                "Não é possível obter logs de outro usuário");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetUnarchivedLogsByUserId(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_GetUnarchivedLogsByUserId_And_Exception_Is_Thrown()
        {
            // Arrange
            var userId = 0;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetUnarchivedLogsByUserId(userId).Result;
            
            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.GetUnarchivedLogsByUserId(userId));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_All_User_Archived_Logs_When_GetArchivedLogsByUserId_With_Owner(int userId)
        {
            // Arrange
            var loggedUserId = userId;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var logs = fakeRepository.GetAllArchivedByUserId(userId).Result;
            var response = fakes.Mapper.Map<LogResponse[]>(logs);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetArchivedLogsByUserId(userId).Result;
            var logResponses = actual.Data as LogResponse[];
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseListComparer());
            Array.ForEach(logResponses, log => Assert.True(log.Archived));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_All_User_Archived_Logs_When_GetArchivedLogsByUserId_With_Manager(int userId)
        {
            // Arrange
            var loggedUserId = userId;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var logs = fakeRepository.GetAllArchivedByUserId(userId).Result;
            var response = fakes.Mapper.Map<LogResponse[]>(logs);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetArchivedLogsByUserId(userId).Result;
            var logResponses = actual.Data as LogResponse[];
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseListComparer());
            Array.ForEach(logResponses, log => Assert.True(log.Archived));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Forbidden_When_GetArchivedLogsByUserId_Without_Authorization(int userId)
        {
            // Arrange
            var loggedUserId = userId - 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var expected = Responses.ForbiddenResponse(
                "Não é possível obter logs de outro usuário");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetArchivedLogsByUserId(userId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_GetArchivedLogsByUserId_And_Exception_Is_Thrown()
        {
            // Arrange
            var userId = 0;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetArchivedLogsByUserId(userId).Result;
            
            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.GetArchivedLogsByUserId(userId));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Theory]
        [InlineData(1, 9)]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        [InlineData(4, 4)]
        public void Should_Return_Log_When_GetLogById_With_Owner(int logId, int loggedUserId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var log = fakeRepository.GetById(logId).Result;
            var response = fakes.Mapper.Map<LogResponse>(log);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetLogById(logId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Log_When_GetLogById_With_Manager(int logId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var log = fakeRepository.GetById(logId).Result;
            var response = fakes.Mapper.Map<LogResponse>(log);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetLogById(logId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseComparer());
        }

        [Fact]
        public void Should_Return_Forbidden_When_GetLogById_Without_Authorization()
        {
            // Arrange
            var logId = 1;
            var loggedUserId = 0; // Doesn't exists user with id 0
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var expected = Responses.ForbiddenResponse(
                "Não é possível obter logs de outro usuário");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetLogById(logId).Result;
            
            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_GetLogById_And_Exception_Is_Thrown()
        {
            // Arrange
            var logId = 0;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepo = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepo, hashService);

            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.GetLogById(logId).Result;
            
            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.GetLogById(logId));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_Log_When_Create_With_Manager()
        {
            // Arrange
            var loggedManagerId = 0;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true, loggedManagerId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var request = fakes.Get<CreateLogRequest>().First();
            request.UserId = null;  // When UserId is not specified, the log is created with the id of logged MANAGER.
            var log = fakes.Get<Log>().First();

            var response = fakes.Mapper.Map<LogResponse>(log);
            response.Id = 999;  // Mocked id when creating a new log
            response.Archived = false;  // By default, log is created as unarchived
            response.UserId = loggedManagerId;  // As UserId were not specified, the log should be created with logged MANAGER id

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Create(request).Result;

            response.CreatedAt = (actual.Data as LogResponse).CreatedAt;
            var expected = Responses.OkResponse(null, response);

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseComparer());
        }

        [Fact]
        public void Should_Return_Log_When_Create_With_Manager_And_Specified_UserId()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var request = fakes.Get<CreateLogRequest>().First();
            var log = fakes.Get<Log>().First();

            var response = fakes.Mapper.Map<LogResponse>(log);
            response.Id = 999;  // Mocked id when creating a new log
            response.Archived = false;  // By default, log is created as unarchived
            response.UserId = request.UserId.Value; // UserId were specified, so it should be equal to the UserId in request

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Create(request).Result;

            response.CreatedAt = (actual.Data as LogResponse).CreatedAt;
            var expected = Responses.OkResponse(null, response);

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_Create_And_Exception_Is_Thrown()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var request = fakes.Get<CreateLogRequest>().First();
            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Create(request).Result;

            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.Create(request));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Theory]
        [InlineData(1, 9)]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        [InlineData(4, 4)]
        public void Should_Return_Log_When_Update_With_Owner(int logId, int loggedUserId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);
            
            var log = fakeRepository.GetById(logId).Result;
            log.Title = "Updated Title";

            var request = fakes.Mapper.Map<UpdateLogRequest>(log);
            var response = fakes.Mapper.Map<LogResponse>(log);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Update(logId, request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseComparer());
        }

        [Fact]
        public void Should_Return_User_When_Update_With_Manager()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var request = fakes.Get<UpdateLogRequest>().First();
            request.Title = "Updated Title";

            var log = fakes.Get<Log>().First();
            log.Title = request.Title;
            var response = fakes.Mapper.Map<LogResponse>(log);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Update(log.Id, request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Forbidden_When_Update_Without_Authorization(int logId)
        {
            // Arrange
            var fakes = new Fakes();
            var loggedUserId = 0; // Doesn't exist user with id 0
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var request = fakes.Get<UpdateLogRequest>().First();
            var expected = Responses.ForbiddenResponse(
                "Não é possível atualizar um log de outro usuário");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Update(logId, request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_NotFound_When_Update_With_An_Incorrect_Id()
        {
            // Arrange
            var logId = 9999;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var request = new UpdateLogRequest();
            var expected = Responses.NotFoundResponse("Log não encontrado");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Update(logId, request).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_Update_And_Exception_Is_Thrown()
        {
            // Arrange
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var request = new UpdateLogRequest();
            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Update(logId, request).Result;

            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.Update(logId, request));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Theory]
        [InlineData(1, 9)]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        [InlineData(4, 4)]
        public void Should_Return_Ok_When_Delete_With_Owner(int logId, int loggedUserId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var expected = Responses.OkResponse("Log deletado");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Delete(logId).Result;

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
        public void Should_Return_Ok_When_Delete_With_Manager(int logId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var expected = Responses.OkResponse("Log deletado");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Delete(logId).Result;

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
        public void Should_Return_Forbidden_When_Delete_Without_Authorization(int logId)
        {
            // Arrange
            var fakes = new Fakes();
            var loggedUserId = 0;  // Doesn't exist user with id 0
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var expected = Responses.ForbiddenResponse(
                "Não é possível deletar um log de outro usuário");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Delete(logId).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_NotFound_When_Delete_With_An_Incorrect_Id()
        {
            // Arrange
            var logId = 9999;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var expected = Responses.NotFoundResponse("Log não encontrado");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Delete(logId).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_Delete_And_Exception_Is_Thrown()
        {
            // Arrange
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Delete(logId).Result;

            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.Delete(logId));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Theory]
        [InlineData(1, 9)]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        [InlineData(4, 4)]
        public void Should_Return_Log_When_Archive_With_Owner(int logId, int loggedUserId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var log = fakeRepository.GetById(logId).Result;
            log.Archived = !log.Archived; // Should change log archiving state
            var response = fakes.Mapper.Map<LogResponse>(log);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Archive(logId).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Log_When_Archive_With_Manager(int logId)
        {
            // Arrange
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);
            
            var log = fakeRepository.GetById(logId).Result;
            log.Archived = !log.Archived; // Should change log archiving state
            var response = fakes.Mapper.Map<LogResponse>(log);
            var expected = Responses.OkResponse(null, response);

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Archive(logId).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new LogResponseComparer());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Return_Forbidden_When_Archive_Without_Authorization(int logId)
        {
            // Arrange
            var fakes = new Fakes();
            var loggedUserId = 0;  // Doesn't exist user with id 0
            var fakeHttp = fakes.FakeHttpContextAccessor(false, loggedUserId).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var expected = Responses.ForbiddenResponse(
                "Não é possível arquivar ou desarquivar um log de outro usuário");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Archive(logId).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_NotFound_When_Archive_With_An_Incorrect_Id()
        {
            // Arrange
            var logId = 9999;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepository().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var expected = Responses.NotFoundResponse("Log não encontrado");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Archive(logId).Result;

            // Assert
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }

        [Fact]
        public void Should_Return_BadRequest_When_Archive_And_Exception_Is_Thrown()
        {
            // Arrange
            var logId = 1;
            var fakes = new Fakes();
            var fakeHttp = fakes.FakeHttpContextAccessor(true).Object;
            var fakeRepository = fakes.FakeLogRepositoryException().Object;
            var fakeUserRepository = fakes.FakeUserRepository().Object;
            var hashService = new HashService();
            var userService = new UserService(fakes.Mapper, fakeHttp, fakeUserRepository, hashService);

            var expected = Responses.BadRequestResponse("Test Exception");

            // Act
            var service = new LogService(fakes.Mapper, fakeRepository, userService);
            var actual = service.Archive(logId).Result;

            // Assert
            Assert.ThrowsAnyAsync<Exception>(() => service.Archive(logId));
            Assert.IsType<Response>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual, new ResponseComparer());
        }
    }
}