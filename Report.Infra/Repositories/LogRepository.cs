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

        public override Task<Log[]> GetAll()
        {
            return _dbContext.Set<Log>()
                             .Include(log => log.User)
                             .AsNoTracking()
                             .ToArrayAsync();
        }

        public override Task<Log> GetById(int id)
        {
            return _dbContext.Set<Log>()
                             .Include(log => log.User)
                             .AsNoTracking()
                             .FirstOrDefaultAsync(log => log.Id.Equals(id));
        }

        Task<Log[]> ILogRepository.GetAllByUserId(int userId)
        {
            return _dbContext.Set<Log>()
                             .Include(log => log.User)
                             .AsNoTracking()
                             .Where(log => log.UserId.Equals(userId))
                             .ToArrayAsync();
        }
    }
}