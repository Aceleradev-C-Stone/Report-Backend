using System.Collections.Generic;
using System.Threading.Tasks;
using Report.Domain.Models;

namespace Report.Domain.Repositories
{
    public interface ILogRepository : IBaseRepository<Log>
    {
        Task<Log[]> GetAllByUserId(int userId);
    }
}