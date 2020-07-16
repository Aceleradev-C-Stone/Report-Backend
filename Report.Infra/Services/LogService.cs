using System;
using System.Threading.Tasks;
using AutoMapper;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Models;
using Report.Core.Repositories;
using Report.Core.Services;

namespace Report.Infra.Services
{
    public class LogService : BaseService, ILogService
    {
        private readonly IMapper _mapper;
        private readonly ILogRepository _repository;
        private readonly IUserService _userService;

        public LogService(
            IMapper mapper,
            ILogRepository repository,
            IUserService userService)
        {
            _mapper = mapper;
            _repository = repository;
            _userService = userService;
        }

        public async Task<Response> GetAll()
        {
            try
            {
                var logs = await _repository.GetAll();
                var response = _mapper.Map<LogResponse[]>(logs);
                return OkResponse(null, response);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        public async Task<Response> GetLogsByUserId(int userId)
        {
            try
            {
                if (!_userService.IsAuthenticated(userId))
                    return ForbiddenResponse(
                        "Não é possível obter logs de outro usuário");

                var logs = await _repository.GetAllByUserId(userId);
                var response = _mapper.Map<LogResponse[]>(logs);
                return OkResponse(null, response);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        public async Task<Response> GetUnarchivedLogsByUserId(int userId)
        {
             try
            {
                if (!_userService.IsAuthenticated(userId))
                    return ForbiddenResponse(
                        "Não é possível obter logs de outro usuário");
                
                var logs = await _repository.GetAllUnarchivedByUserId(userId);
                var response = _mapper.Map<LogResponse[]>(logs);
                return OkResponse(null, response);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        public async Task<Response> GetArchivedLogsByUserId(int userId)
        {
            try
            {
                if (!_userService.IsAuthenticated(userId))
                    return ForbiddenResponse(
                        "Não é possível obter logs de outro usuário");

                var logs = await _repository.GetAllArchivedByUserId(userId);
                var response = _mapper.Map<LogResponse[]>(logs);
                return OkResponse(null, response);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        public async Task<Response> GetLogById(int logId)
        {
            try
            {
                var log = await _repository.GetById(logId);

                if (!_userService.IsAuthenticated(log.UserId))
                    return ForbiddenResponse(
                        "Não é possível obter logs de outro usuário");

                var response = _mapper.Map<LogResponse>(log);
                return OkResponse(null, response);
            }
            catch (Exception ex)
            {
                return ForbiddenResponse(ex.Message);
            }
        }

        public async Task<Response> Create(CreateLogRequest request)
        {
            try
            {
                // If user role equals MANAGER, use id from request body,
                // otherwise, use logged user id
                int userId = _userService.GetLoggedUserId();
                if (_userService.IsLoggedUserManager() && request.UserId.HasValue)
                    userId =  request.UserId.Value;

                var log = MapCreateLogRequestToLog(userId, request);
                _repository.Add(log);

                if (await _repository.SaveChangesAsync())
                {
                    log = await _repository.GetById(log.Id);
                    var response = _mapper.Map<LogResponse>(log);
                    return OkResponse(null, response);
                }

                return BadRequestResponse("Erro desconhecido");
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        public async Task<Response> Update(int logId, UpdateLogRequest request)
        {
            try
            {
                var log = await _repository.GetById(logId);

                if (!_userService.IsAuthenticated(log.UserId))
                    return ForbiddenResponse(
                        "Não é possível atualizar um log de outro usuário");

                log = MapUpdateLogRequestToLog(log, request);
                _repository.Update(log);

                if (await _repository.SaveChangesAsync())
                {
                    var response = _mapper.Map<LogResponse>(log);
                    return OkResponse(null, response);
                }

                return BadRequestResponse("Erro desconhecido");
            }
            catch (NullReferenceException)
            {
                return NotFoundResponse("Log não encontrado");
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        public async Task<Response> Delete(int logId)
        {
            try
            {
                var log = await _repository.GetById(logId);

                if (!_userService.IsAuthenticated(log.UserId))
                    return ForbiddenResponse(
                        "Não é possível deletar um log de outro usuário");
                
                _repository.Delete(log);

                if (await _repository.SaveChangesAsync())
                    return OkResponse("Log deletado");
                
                return BadRequestResponse("Erro desconhecido");
            }
            catch (NullReferenceException)
            {
                return NotFoundResponse("Log não encontrado");
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        public async Task<Response> Archive(int logId)
        {
            try
            {
                var log = await _repository.GetById(logId);

                if (!_userService.IsAuthenticated(log.UserId))
                    return ForbiddenResponse(
                        "Não é possível arquivar ou desarquivar um log de outro usuário");

                log.Archived = !log.Archived;
                _repository.Update(log);

                if (await _repository.SaveChangesAsync())
                {
                    var response = _mapper.Map<LogResponse>(log);
                    return OkResponse(null, response);
                }
                
                return BadRequestResponse("Erro desconhecido");
            }
            catch (NullReferenceException)
            {
                return NotFoundResponse("Log não encontrado");
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        private Log MapCreateLogRequestToLog(int userId, CreateLogRequest request)
        {
            var log = _mapper.Map<Log>(request);
            log.UserId = userId;
            log.Archived = false;
            log.CreatedAt = DateTime.Now;
            return log;
        }

        private Log MapUpdateLogRequestToLog(Log log, UpdateLogRequest request)
        {
            log.Description = request.Description;
            log.Title       = request.Title;
            log.Details     = request.Details;
            log.Source      = request.Source;
            log.EventCount  = request.EventCount;
            log.Level       = request.Level;
            log.Channel     = request.Channel;
            return log;
        }
    }
}