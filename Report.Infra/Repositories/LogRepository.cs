using Report.Data.Context;
using Report.Domain.Models;

namespace Report.Data.Repositories
{
    public class LogRepository : BaseRepository<Log>, ILogRepository
    {
        public LogRepository(DataContext context) : base(context) {}
    }
}