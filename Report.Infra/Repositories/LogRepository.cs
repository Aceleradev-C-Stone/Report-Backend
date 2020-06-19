using Report.Domain.Models;
using Report.Domain.Repositories;
using Report.Infra.Contexts;

namespace Report.Infra.Repositories
{
    public class LogRepository : BaseRepository<Log>, ILogRepository
    {
        public LogRepository(DataContext context) : base(context) {}
    }
}