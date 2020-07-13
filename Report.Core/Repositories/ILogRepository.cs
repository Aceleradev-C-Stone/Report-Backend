using System.Collections.Generic;
using System.Threading.Tasks;
using Report.Core.Models;

namespace Report.Core.Repositories
{
    public interface ILogRepository : IBaseRepository<Log>
    {
        Task<Log[]> GetAllByUserId(int userId);
        Task<Log[]> GetAllUnarchivedByUserId(int userId);
        Task<Log[]> GetAllArchivedByUserId(int userId);
    }
}