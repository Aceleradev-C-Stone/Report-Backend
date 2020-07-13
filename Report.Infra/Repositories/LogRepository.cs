using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Report.Core.Models;
using Report.Core.Repositories;
using Report.Infra.Contexts;

namespace Report.Infra.Repositories
{
    public class LogRepository : BaseRepository<Log>, ILogRepository
    {
        public LogRepository(DataContext context) : base(context) {}

        public override Task<Log[]> GetAll()
        {
            return _dbContext.Set<Log>()
                             .Where(log => log.Archived == false)
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

        public Task<Log[]> GetAllByUserId(int userId)
        {
            return _dbContext.Set<Log>()
                             .Where(log => log.UserId.Equals(userId))
                             .Include(log => log.User)
                             .AsNoTracking()
                             .ToArrayAsync();
        }

        public Task<Log[]> GetAllArchivedByUserId(int userId)
        {
            return _dbContext.Set<Log>()
                             .Where(log => log.UserId.Equals(userId))
                             .Where(log => log.Archived.Equals(true))
                             .Include(log => log.User)
                             .AsNoTracking()
                             .ToArrayAsync();
        }

        public Task<Log[]> GetAllUnarchivedByUserId(int userId)
        {
            return _dbContext.Set<Log>()
                             .Where(log => log.UserId.Equals(userId))
                             .Where(log => log.Archived.Equals(false))
                             .Include(log => log.User)
                             .AsNoTracking()
                             .ToArrayAsync();
        }
    }
}