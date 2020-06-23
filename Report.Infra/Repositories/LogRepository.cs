using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Report.Domain.Models;
using Report.Domain.Repositories;
using Report.Infra.Contexts;

namespace Report.Infra.Repositories
{
    public class LogRepository : BaseRepository<Log>, ILogRepository
    {
        public LogRepository(DataContext context) : base(context) {}

        Task<Log[]> ILogRepository.GetAllByUserId(int userId)
        {
            return _dbContext.Set<Log>()
                             .AsNoTracking()
                             .Where(log => log.UserId.Equals(userId))
                             .ToArrayAsync();
        }
    }
}