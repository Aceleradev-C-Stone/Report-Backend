using System.Threading.Tasks;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;

namespace Report.Core.Services
{
    public interface ILogService
    {
        Task<Response> GetAll();
        Task<Response> GetLogsByUserId(int userId);
        Task<Response> GetUnarchivedLogsByUserId(int userId);
        Task<Response> GetArchivedLogsByUserId(int userId);
        Task<Response> GetLogById(int logId);
        Task<Response> Create(CreateLogRequest request);
        Task<Response> Update(int logId, UpdateLogRequest request);
        Task<Response> Delete(int logId);
        Task<Response> Archive(int logId);
    }
}